using EcoColeta.Api.Models;
using EcoColeta.Api.Services;
using EcoColeta.Api.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoColeta.Api.Controllers;

[ApiController]
[Route("api/alertas-coleta")]
public class AlertasColetaController : ControllerBase
{
    private readonly IAlertaColetaService _service;

    public AlertasColetaController(IAlertaColetaService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar(
        [FromQuery] ParametrosPaginacao paginacao,
        [FromQuery] bool? resolvido,
        [FromQuery] NivelAlerta? nivel,
        [FromQuery] int? pontoColetaId)
    {
        var resultado = await _service.ListarAsync(
            paginacao.ObterPaginaValida(),
            paginacao.ObterTamanhoPaginaValido(),
            resolvido,
            nivel,
            pontoColetaId);

        return Ok(resultado);
    }

    [HttpPatch("{id:int}/resolver")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Resolver(int id)
    {
        var resultado = await _service.ResolverAsync(id);
        return Ok(resultado);
    }

    [HttpPost("recalcular")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Recalcular()
    {
        var alertasGerados = await _service.RecalcularAlertasAsync();
        return Ok(new { mensagem = "Recálculo concluído.", alertasGerados });
    }
}
