using System.Net;
using System.Text.Json;
using EcoColeta.Tests.Factories;

namespace EcoColeta.Tests.Controllers;

public class HealthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public HealthControllerTests(CustomWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Readiness_DeveInformarBancoEAmbiente()
    {
        var response = await _client.GetAsync("/health/ready");
        response.EnsureSuccessStatusCode();
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Healthy", json.RootElement.GetProperty("status").GetString());
        Assert.Equal("Connected", json.RootElement.GetProperty("database").GetString());
        Assert.Equal("Testing", json.RootElement.GetProperty("environment").GetString());
    }

    [Fact]
    public async Task Liveness_DeveRetornar200SemAutenticacao()
        => Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/health/live")).StatusCode);

    [Fact]
    public async Task Swagger_Desabilitado_NaoDeveExporEspecificacao()
        => Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync("/swagger/v1/swagger.json")).StatusCode);
}
