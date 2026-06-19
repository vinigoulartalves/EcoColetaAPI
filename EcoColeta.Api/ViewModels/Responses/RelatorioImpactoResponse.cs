namespace EcoColeta.Api.ViewModels.Responses;

public class RelatorioImpactoResponse
{
    public decimal TotalResiduosRegistradosKg { get; set; }
    public Dictionary<string, decimal> ResiduosPorTipo { get; set; } = new();
    public int PontosColetaAtivos { get; set; }
    public int AlertasAbertos { get; set; }
    public decimal EstimativaImpactoPositivoKg { get; set; }
    public DateTime GeradoEm { get; set; }
}
