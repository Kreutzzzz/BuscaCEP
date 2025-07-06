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
			return BadRequest(new { message = "CEP inv�lido. Deve conter 8 d�gitos." });
		}

		try
		{
			var endereco = await _viaCepService.BuscarPorCepAsync(cepLimpo);

			if (endereco == null)
			{
				return NotFound(new { message = "CEP n�o encontrado ou inv�lido." });
			}

			return Ok(endereco);
		}
		catch (Exception)
		{
			// Catch gen�rico para falhas inesperadas no servi�o (ex: rede).
			return StatusCode(500, new { message = "Ocorreu um erro ao consultar o servi�o externo." });
		}
	}

	[HttpGet("BuscaEndereco")]
	public async Task<IActionResult> BuscaEndereco([FromQuery] string uf, [FromQuery] string municipio, [FromQuery] string logradouro)
	{
		if (string.IsNullOrWhiteSpace(uf) || string.IsNullOrWhiteSpace(municipio) || string.IsNullOrWhiteSpace(logradouro))
		{
			return BadRequest(new { message = "Os par�metros uf, municipio e logradouro s�o obrigat�rios." });
		}

		try
		{
			var enderecos = await _viaCepService.BuscarPorEnderecoAsync(uf, municipio, logradouro);

			if (enderecos == null || !enderecos.Any())
			{
				return NotFound(new { message = "Nenhum endere�o encontrado para os crit�rios fornecidos." });
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
			return StatusCode(500, new { message = "Ocorreu um erro ao consultar o servi�o externo." });
		}
	}
}