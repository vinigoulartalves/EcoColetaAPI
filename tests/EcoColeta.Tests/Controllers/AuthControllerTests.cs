using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EcoColeta.Tests.Factories;

namespace EcoColeta.Tests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LoginValido_DeveRetornarToken()
    {
        // Arrange
        var request = "/api/auth/login";
        var payload = new { email = TestConstants.AdminEmail, senha = TestConstants.AdminSenha };

        // Act
        var response = await _client.PostAsJsonAsync(request, payload);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);
        var token = document.RootElement.GetProperty("token").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task EndpointProtegido_SemToken_DeveRetornar401()
    {
        // Arrange
        var request = "/api/pontos-coleta";
        var payload = new
        {
            nome = "Ponto Sem Auth",
            endereco = "Rua A, 1",
            bairro = "Centro",
            cidade = "TestCity",
            latitude = -23.5,
            longitude = -46.6,
            tipoResiduoAceito = 1,
            capacidadeMaximaKg = 200m
        };

        // Act
        var response = await _client.PostAsJsonAsync(request, payload);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
