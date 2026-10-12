namespace EcoColeta.Api.ViewModels.Responses;

public class RegistroResiduoResponse
{
    public int Id { get; set; }
    public int PontoColetaId { get; set; }
    public string NomePontoColeta { get; set; } = string.Empty;
    public string TipoResiduo { get; set; } = string.Empty;
    public decimal PesoKg { get; set; }
    public string Origem { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public DateTime RegistradoEm { get; set; }
}
