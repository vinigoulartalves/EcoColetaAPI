namespace EcoColeta.Api.Models;

public class RegistroResiduo
{
    public int Id { get; set; }
    public int PontoColetaId { get; set; }
    public TipoResiduo TipoResiduo { get; set; }
    public decimal PesoKg { get; set; }
    public string Origem { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public DateTime RegistradoEm { get; set; } = DateTime.UtcNow;

    public PontoColeta PontoColeta { get; set; } = null!;
}
