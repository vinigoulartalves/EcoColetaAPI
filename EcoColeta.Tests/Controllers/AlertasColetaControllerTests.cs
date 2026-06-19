using System.Net;
using EcoColeta.Tests.Factories;

namespace EcoColeta.Tests.Controllers;

public class AlertasColetaControllerTests : IClassFixture<EcoColetaWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AlertasColetaControllerTests(EcoColetaWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Listar_DeveRetornarStatus200()
    {
        var response = await _client.GetAsync("/api/alertas-coleta");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
