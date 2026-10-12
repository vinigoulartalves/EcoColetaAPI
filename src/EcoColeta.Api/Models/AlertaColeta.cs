namespace EcoColeta.Api.Models;

public class AlertaColeta
{
    public int Id { get; set; }
    public int PontoColetaId { get; set; }
    public NivelAlerta Nivel { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public bool Resolvido { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvidoEm { get; set; }

    public PontoColeta PontoColeta { get; set; } = null!;
}
