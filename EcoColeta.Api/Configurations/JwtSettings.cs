namespace EcoColeta.Api.Configurations;

public class JwtSettings
{
    public const string SectionName = "Jwt";
    public string ChaveSecreta { get; set; } = string.Empty;
    public string Emissor { get; set; } = string.Empty;
    public string Audiencia { get; set; } = string.Empty;
    public int ExpiracaoMinutos { get; set; } = 60;
}
