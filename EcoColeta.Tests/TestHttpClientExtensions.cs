using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace EcoColeta.Tests;

public static class TestHttpClientExtensions
{
    public static async Task<string> ObterTokenAdminAsync(this HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = TestConstants.AdminEmail,
            senha = TestConstants.AdminSenha
        });

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);
        return document.RootElement.GetProperty("token").GetString()
            ?? throw new InvalidOperationException("Token não retornado no login.");
    }

    public static void DefinirTokenBearer(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
