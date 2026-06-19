namespace EcoColeta.Api.ViewModels.Responses;

public class PaginacaoResponse<T>
{
    public IEnumerable<T> Itens { get; set; } = Enumerable.Empty<T>();
    public int PaginaAtual { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalItens { get; set; }
    public int TotalPaginas { get; set; }
}
