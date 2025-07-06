using System.Text.Json.Serialization;

namespace BuscaCepApi.Models;


// Representa a resposta resumida de um endere�o, contendo apenas os campos
// necess�rios para a listagem inicial no frontend.

public class EnderecoSummaryResponse
{
    [JsonPropertyName("cep")]
    public string? Cep { get; set; }

    [JsonPropertyName("logradouro")]
    public string? Logradouro { get; set; }

    [JsonPropertyName("complemento")]
    public string? Complemento { get; set; }

    [JsonPropertyName("bairro")]
    public string? Bairro { get; set; }

    [JsonPropertyName("localidade")]
    public string? Localidade { get; set; }

    [JsonPropertyName("uf")]
    public string? Uf { get; set; }
}