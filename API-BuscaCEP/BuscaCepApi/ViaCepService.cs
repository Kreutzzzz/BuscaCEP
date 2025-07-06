using System.Text.Json;
using BuscaCepApi.Models;

// Implementa��o do contrato IViaCepService, focada em interagir com a API externa do ViaCEP.
public class ViaCepService : IViaCepService
{
    private readonly HttpClient _httpClient;

    public ViaCepService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        // Define a URL base para todas as requisi��es feitas por esta inst�ncia do HttpClient.
        _httpClient.BaseAddress = new Uri("https://viacep.com.br/ws/");
    }

    public async Task<EnderecoResponse?> BuscarPorCepAsync(string cep)
    {
        var response = await _httpClient.GetAsync($"{cep}/json/");
        // Delega o processamento e a deserializa��o da resposta para um m�todo centralizado.
        return await ProcessarResposta<EnderecoResponse>(response);
    }

    public async Task<List<EnderecoResponse>?> BuscarPorEnderecoAsync(string uf, string municipio, string logradouro)
    {
        // Formata os par�metros para serem seguros para uma URL, tratando espa�os e caracteres especiais.
        var url = $"{Uri.EscapeDataString(uf)}/{Uri.EscapeDataString(municipio)}/{Uri.EscapeDataString(logradouro)}/json/";
        var response = await _httpClient.GetAsync(url);
        return await ProcessarResposta<List<EnderecoResponse>>(response);
    }

    // M�todo gen�rico privado que centraliza a l�gica de tratamento da resposta do ViaCEP.
    // Isso evita duplica��o de c�digo e torna a manuten��o mais simples.
    private async Task<T?> ProcessarResposta<T>(HttpResponseMessage response) where T : class
    {
        // Primeira verifica��o: falhas na requisi��o HTTP (ex: 400, 404, 500).
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        // Leitura defensiva como string para evitar exce��es com respostas inesperadas.
        var responseString = await response.Content.ReadAsStringAsync();

        // Verifica��o da regra de neg�cio do ViaCEP, que retorna a flag "erro".
        if (string.IsNullOrWhiteSpace(responseString) || responseString.Contains("\"erro\": \"true\""))
        {
            return null;
        }

        // Tenta deserializar a string para o tipo T. Se falhar (JSON malformado), retorna nulo.
        try
        {
            return JsonSerializer.Deserialize<T>(responseString);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}