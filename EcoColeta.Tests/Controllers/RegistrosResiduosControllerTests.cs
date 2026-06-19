using System.Net;
using EcoColeta.Tests.Factories;

namespace EcoColeta.Tests.Controllers;

public class RegistrosResiduosControllerTests : IClassFixture<EcoColetaWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegistrosResiduosControllerTests(EcoColetaWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Listar_DeveRetornarStatus200()
    {
        var response = await _client.GetAsync("/api/registros-residuos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
