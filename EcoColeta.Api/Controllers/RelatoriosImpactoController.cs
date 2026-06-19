using EcoColeta.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoColeta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RelatoriosImpactoController : ControllerBase
{
    private readonly IRelatorioImpactoService _service;

    public RelatoriosImpactoController(IRelatorioImpactoService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Gerar()
    {
        var resultado = await _service.GerarAsync();
        return Ok(resultado);
    }

    [HttpPost("gerar")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GerarConsolidado()
    {
        var resultado = await _service.GerarAsync();
        return Ok(resultado);
    }
}
