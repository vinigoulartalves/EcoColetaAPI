using EcoColeta.Api.Models;
using EcoColeta.Api.Services;
using EcoColeta.Api.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoColeta.Api.Controllers;

[ApiController]
[Route("api/registros-residuos")]
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
        [FromQuery] ParametrosPaginacao paginacao,
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

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarResiduoRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resultado = await _service.RegistrarAsync(request);
        return Created(string.Empty, resultado);
    }
}
