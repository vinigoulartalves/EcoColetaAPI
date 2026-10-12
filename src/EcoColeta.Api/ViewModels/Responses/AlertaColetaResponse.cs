namespace EcoColeta.Api.ViewModels.Responses;

public class AlertaColetaResponse
{
    public int Id { get; set; }
    public int PontoColetaId { get; set; }
    public string NomePontoColeta { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public bool Resolvido { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? ResolvidoEm { get; set; }
}
