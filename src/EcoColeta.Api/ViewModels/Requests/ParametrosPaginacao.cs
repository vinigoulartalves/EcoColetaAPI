namespace EcoColeta.Api.ViewModels.Requests;

public class ParametrosPaginacao
{
    private const int TamanhoPaginaMaximo = 50;

    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;

    public int ObterPaginaValida() => Pagina < 1 ? 1 : Pagina;

    public int ObterTamanhoPaginaValido()
    {
        if (TamanhoPagina < 1) return 10;
        return TamanhoPagina > TamanhoPaginaMaximo ? TamanhoPaginaMaximo : TamanhoPagina;
    }
}
