using EcoColeta.Api.Models;
using EcoColeta.Api.ViewModels.Responses;

namespace EcoColeta.Api.Repositories;

public interface IDestinacaoResiduoRepository
{
    Task<PaginacaoResponse<DestinacaoResiduo>> ListarAsync(int pagina, int tamanhoPagina, bool? reciclavel);
    Task<DestinacaoResiduo?> ObterPorIdAsync(int id);
    Task<DestinacaoResiduo?> ObterPorTipoAsync(TipoResiduo tipoResiduo);
    Task<DestinacaoResiduo> CriarAsync(DestinacaoResiduo destinacao);
    Task<DestinacaoResiduo> AtualizarAsync(DestinacaoResiduo destinacao);
    Task ExcluirAsync(DestinacaoResiduo destinacao);
}
