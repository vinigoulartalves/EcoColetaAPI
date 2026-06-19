using EcoColeta.Api.Models;
using EcoColeta.Api.Services;
using EcoColeta.Api.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoColeta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        [FromQuery] FiltroPaginacaoRequest paginacao,
        [FromQuery] bool? resolvido,
        [FromQuery] NivelAlerta? nivel)
    {
        var resultado = await _service.ListarAsync(
            paginacao.ObterPaginaValida(),
            paginacao.ObterTamanhoPaginaValido(),
            resolvido,
            nivel);

        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var resultado = await _service.ObterPorIdAsync(id);
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
