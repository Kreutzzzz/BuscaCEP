using BuscaCepApi.Models;

public interface IViaCepService
{
    Task<EnderecoResponse?> BuscarPorCepAsync(string cep);
    Task<List<EnderecoResponse>?> BuscarPorEnderecoAsync(string uf, string municipio, string logradouro);
}