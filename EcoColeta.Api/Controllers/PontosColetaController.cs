using EcoColeta.Api.Models;
using EcoColeta.Api.Services;
using EcoColeta.Api.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoColeta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PontosColetaController : ControllerBase
{
    private readonly IPontoColetaService _service;

    public PontosColetaController(IPontoColetaService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar(
        [FromQuery] FiltroPaginacaoRequest paginacao,
        [FromQuery] string? cidade,
        [FromQuery] TipoResiduo? tipoResiduo,
        [FromQuery] bool? ativo)
    {
        var resultado = await _service.ListarAsync(
            paginacao.ObterPaginaValida(),
            paginacao.ObterTamanhoPaginaValido(),
            cidade,
            tipoResiduo,
            ativo);

        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var resultado = await _service.ObterPorIdAsync(id);
        return Ok(resultado);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Criar([FromBody] CriarPontoColetaRequest request)
    {
        var resultado = await _service.CriarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarPontoColetaRequest request)
    {
        var resultado = await _service.AtualizarAsync(id, request);
        return Ok(resultado);
    }
}
