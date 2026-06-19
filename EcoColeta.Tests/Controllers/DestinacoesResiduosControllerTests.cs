using System.Net;
using EcoColeta.Tests.Factories;

namespace EcoColeta.Tests.Controllers;

public class DestinacoesResiduosControllerTests : IClassFixture<EcoColetaWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DestinacoesResiduosControllerTests(EcoColetaWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Listar_DeveRetornarStatus200()
    {
        var response = await _client.GetAsync("/api/DestinacoesResiduos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
