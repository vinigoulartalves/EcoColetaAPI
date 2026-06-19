namespace EcoColeta.Api.ViewModels.Responses;

public class RelatorioImpactoResponse
{
    public decimal TotalResiduosRegistradosKg { get; set; }
    public int TotalPontosColeta { get; set; }
    public int PontosAtivos { get; set; }
    public int PontosEmAlerta { get; set; }
    public int AlertasCriticos { get; set; }
    public decimal EstimativaImpactoPositivoKg { get; set; }
    public Dictionary<string, decimal> ResiduosPorTipo { get; set; } = new();
    public DateTime GeradoEm { get; set; }
}
