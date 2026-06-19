using System.Net.Http.Json;
using System.Text.Json;
using EcoColeta.Tests.Factories;

namespace EcoColeta.Tests.Controllers;

public class RegistrosResiduosAlertaTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegistrosResiduosAlertaTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegistrarAcimaDe80PorCento_DeveGerarAlerta()
    {
        // Arrange
        var token = await _client.ObterTokenAdminAsync();
        _client.DefinirTokenBearer(token);

        var registro = new
        {
            pontoColetaId = 1,
            tipoResiduo = 1,
            pesoKg = 81m,
            origem = "Teste Automatizado"
        };

        // Act
        var registroResponse = await _client.PostAsJsonAsync("/api/registros-residuos", registro);
        registroResponse.EnsureSuccessStatusCode();

        var alertasResponse = await _client.GetAsync("/api/alertas-coleta?resolvido=false&pontoColetaId=1");

        // Assert
        alertasResponse.EnsureSuccessStatusCode();
        var content = await alertasResponse.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);
        var totalItens = document.RootElement.GetProperty("totalItens").GetInt32();
        Assert.True(totalItens >= 1, "Esperava pelo menos um alerta gerado após ocupação acima de 80%.");
    }
}
