namespace EcoColeta.Api.ViewModels.Responses;

public class DestinacaoResiduoResponse
{
    public int Id { get; set; }
    public string TipoResiduo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string InstrucoesDescarte { get; set; } = string.Empty;
    public bool Reciclavel { get; set; }
    public string RiscoAmbiental { get; set; } = string.Empty;
}
