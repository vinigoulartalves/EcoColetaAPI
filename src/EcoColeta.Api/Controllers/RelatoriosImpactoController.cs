using EcoColeta.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoColeta.Api.Controllers;

[ApiController]
[Route("api/relatorios-impacto")]
public class RelatoriosImpactoController : ControllerBase
{
    private readonly IRelatorioImpactoService _service;

    public RelatoriosImpactoController(IRelatorioImpactoService service)
    {
        _service = service;
    }

    [HttpGet("resumo")]
    [AllowAnonymous]
    public async Task<IActionResult> ObterResumo()
    {
        var resultado = await _service.ObterResumoAsync();
        return Ok(resultado);
    }
}
