using BuscaCepApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BuscaCepApi.Controllers;

[ApiController]
[Route("api")]
public class EnderecoController : ControllerBase
{
	private readonly IViaCepService _viaCepService;

	public EnderecoController(IViaCepService viaCepService)
	{
		_viaCepService = viaCepService;
	}

	[HttpGet("BuscaCep/{cep}")]
	public async Task<IActionResult> BuscaCep(string cep)
	{
		var cepLimpo = new string(cep.Where(char.IsDigit).ToArray());
		if (cepLimpo.Length != 8)
		{
			return BadRequest(new { message = "CEP inválido. Deve conter 8 dígitos." });
		}

		try
		{
			var endereco = await _viaCepService.BuscarPorCepAsync(cepLimpo);

			if (endereco == null)
			{
				return NotFound(new { message = "CEP não encontrado ou inválido." });
			}

			return Ok(endereco);
		}
		catch (Exception)
		{
			// Catch genérico para falhas inesperadas no serviço (ex: rede).
			return StatusCode(500, new { message = "Ocorreu um erro ao consultar o serviço externo." });
		}
	}

	[HttpGet("BuscaEndereco")]
	public async Task<IActionResult> BuscaEndereco([FromQuery] string uf, [FromQuery] string municipio, [FromQuery] string logradouro)
	{
		if (string.IsNullOrWhiteSpace(uf) || string.IsNullOrWhiteSpace(municipio) || string.IsNullOrWhiteSpace(logradouro))
		{
			return BadRequest(new { message = "Os parâmetros uf, municipio e logradouro são obrigatórios." });
		}

		try
		{
			var enderecos = await _viaCepService.BuscarPorEnderecoAsync(uf, municipio, logradouro);

			if (enderecos == null || !enderecos.Any())
			{
				return NotFound(new { message = "Nenhum endereço encontrado para os critérios fornecidos." });
			}

			var summaryResponse = enderecos.Select(end => new EnderecoSummaryResponse
			{
				Cep = end.Cep,
				Logradouro = end.Logradouro,
				Complemento = end.Complemento,
				Bairro = end.Bairro,
				Localidade = end.Localidade,
				Uf = end.Uf
			}).ToList();

			return Ok(summaryResponse);
		}
		catch (Exception)
		{
			return StatusCode(500, new { message = "Ocorreu um erro ao consultar o serviço externo." });
		}
	}
}