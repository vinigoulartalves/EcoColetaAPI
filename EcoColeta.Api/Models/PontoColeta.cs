namespace EcoColeta.Api.Models;

public class PontoColeta
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TipoResiduo TipoResiduoAceito { get; set; }
    public decimal CapacidadeMaximaKg { get; set; }
    public decimal OcupacaoAtualKg { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<RegistroResiduo> Registros { get; set; } = new List<RegistroResiduo>();
    public ICollection<AlertaColeta> Alertas { get; set; } = new List<AlertaColeta>();
}
