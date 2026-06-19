using EcoColeta.Api.Models;
using EcoColeta.Api.Services;
using EcoColeta.Api.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoColeta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrosResiduosController : ControllerBase
{
    private readonly IRegistroResiduoService _service;

    public RegistrosResiduosController(IRegistroResiduoService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar(
        [FromQuery] FiltroPaginacaoRequest paginacao,
        [FromQuery] int? pontoColetaId,
        [FromQuery] TipoResiduo? tipoResiduo)
    {
        var resultado = await _service.ListarAsync(
            paginacao.ObterPaginaValida(),
            paginacao.ObterTamanhoPaginaValido(),
            pontoColetaId,
            tipoResiduo);

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
    public async Task<IActionResult> Registrar([FromBody] CriarRegistroResiduoRequest request)
    {
        var resultado = await _service.RegistrarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
    }
}
