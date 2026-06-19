using System.Net;
using EcoColeta.Tests.Factories;

namespace EcoColeta.Tests.Controllers;

public class RelatoriosImpactoControllerTests : IClassFixture<EcoColetaWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RelatoriosImpactoControllerTests(EcoColetaWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ObterResumo_DeveRetornarStatus200()
    {
        var response = await _client.GetAsync("/api/relatorios-impacto/resumo");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
