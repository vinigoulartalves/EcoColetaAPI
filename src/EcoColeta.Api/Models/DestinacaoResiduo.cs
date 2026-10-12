namespace EcoColeta.Api.Models;

public class DestinacaoResiduo
{
    public int Id { get; set; }
    public TipoResiduo TipoResiduo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string InstrucoesDescarte { get; set; } = string.Empty;
    public bool Reciclavel { get; set; }
    public string RiscoAmbiental { get; set; } = string.Empty;
}
