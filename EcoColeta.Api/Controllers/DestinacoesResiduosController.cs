using EcoColeta.Api.Models;
using EcoColeta.Api.Services;
using EcoColeta.Api.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoColeta.Api.Controllers;

[ApiController]
[Route("api/destinacoes-residuos")]
public class DestinacoesResiduosController : ControllerBase
{
    private readonly IDestinacaoResiduoService _service;

    public DestinacoesResiduosController(IDestinacaoResiduoService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar([FromQuery] ParametrosPaginacao paginacao)
    {
        var resultado = await _service.ListarAsync(
            paginacao.ObterPaginaValida(),
            paginacao.ObterTamanhoPaginaValido());

        return Ok(resultado);
    }

    [HttpGet("{tipoResiduo}")]
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resultado = await _service.CriarAsync(request);
        return CreatedAtAction(nameof(ObterPorTipo), new { tipoResiduo = resultado.TipoResiduo }, resultado);
    }
}
