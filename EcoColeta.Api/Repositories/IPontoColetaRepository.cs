using EcoColeta.Api.Models;
using EcoColeta.Api.ViewModels.Responses;

namespace EcoColeta.Api.Repositories;

public interface IPontoColetaRepository
{
    Task<PaginacaoResponse<PontoColeta>> ListarAsync(int pagina, int tamanhoPagina, string? cidade, TipoResiduo? tipoResiduo, bool? ativo);
    Task<PontoColeta?> ObterPorIdAsync(int id);
    Task<PontoColeta> CriarAsync(PontoColeta pontoColeta);
    Task<PontoColeta> AtualizarAsync(PontoColeta pontoColeta);
    Task<IEnumerable<PontoColeta>> ListarTodosAtivosAsync();
}
