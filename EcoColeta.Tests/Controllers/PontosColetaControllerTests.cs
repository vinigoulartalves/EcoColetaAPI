using System.Net;
using EcoColeta.Tests.Factories;

namespace EcoColeta.Tests.Controllers;

public class PontosColetaControllerTests : IClassFixture<EcoColetaWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PontosColetaControllerTests(EcoColetaWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Listar_DeveRetornarStatus200()
    {
        var response = await _client.GetAsync("/api/pontos-coleta");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
