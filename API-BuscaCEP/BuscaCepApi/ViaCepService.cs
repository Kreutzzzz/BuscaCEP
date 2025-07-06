using System.Text.Json;
using BuscaCepApi.Models;

// Implementação do contrato IViaCepService, focada em interagir com a API externa do ViaCEP.
public class ViaCepService : IViaCepService
{
    private readonly HttpClient _httpClient;

    public ViaCepService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        // Define a URL base para todas as requisições feitas por esta instância do HttpClient.
        _httpClient.BaseAddress = new Uri("https://viacep.com.br/ws/");
    }

    public async Task<EnderecoResponse?> BuscarPorCepAsync(string cep)
    {
        var response = await _httpClient.GetAsync($"{cep}/json/");
        // Delega o processamento e a deserialização da resposta para um método centralizado.
        return await ProcessarResposta<EnderecoResponse>(response);
    }

    public async Task<List<EnderecoResponse>?> BuscarPorEnderecoAsync(string uf, string municipio, string logradouro)
    {
        // Formata os parâmetros para serem seguros para uma URL, tratando espaços e caracteres especiais.
        var url = $"{Uri.EscapeDataString(uf)}/{Uri.EscapeDataString(municipio)}/{Uri.EscapeDataString(logradouro)}/json/";
        var response = await _httpClient.GetAsync(url);
        return await ProcessarResposta<List<EnderecoResponse>>(response);
    }

    // Método genérico privado que centraliza a lógica de tratamento da resposta do ViaCEP.
    // Isso evita duplicação de código e torna a manutenção mais simples.
    private async Task<T?> ProcessarResposta<T>(HttpResponseMessage response) where T : class
    {
        // Primeira verificação: falhas na requisição HTTP (ex: 400, 404, 500).
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        // Leitura defensiva como string para evitar exceções com respostas inesperadas.
        var responseString = await response.Content.ReadAsStringAsync();

        // Verificação da regra de negócio do ViaCEP, que retorna a flag "erro".
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