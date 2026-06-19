using EcoColeta.Api.Models;
using EcoColeta.Api.Services;
using EcoColeta.Api.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoColeta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DestinacoesResiduosController : ControllerBase
{
    private readonly IDestinacaoResiduoService _service;

    public DestinacoesResiduosController(IDestinacaoResiduoService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar(
        [FromQuery] FiltroPaginacaoRequest paginacao,
        [FromQuery] bool? reciclavel)
    {
        var resultado = await _service.ListarAsync(
            paginacao.ObterPaginaValida(),
            paginacao.ObterTamanhoPaginaValido(),
            reciclavel);

        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var resultado = await _service.ObterPorIdAsync(id);
        return Ok(resultado);
    }

    [HttpGet("tipo/{tipoResiduo}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObterPorTipo(TipoResiduo tipoResiduo)
    {
        var resultado = await _service.ObterPorTipoAsync(tipoResiduo);
        return Ok(resultado);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Criar([FromBody] CriarDestinacaoResiduoRequest request)
    {
        var resultado = await _service.CriarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarDestinacaoResiduoRequest request)
    {
        var resultado = await _service.AtualizarAsync(id, request);
        return Ok(resultado);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _service.ExcluirAsync(id);
        return NoContent();
    }
}
