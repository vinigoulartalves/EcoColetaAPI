namespace EcoColeta.Api.ViewModels.Responses;

public class PontoColetaResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string TipoResiduoAceito { get; set; } = string.Empty;
    public decimal CapacidadeMaximaKg { get; set; }
    public decimal OcupacaoAtualKg { get; set; }
    public decimal PercentualOcupacao { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
}
