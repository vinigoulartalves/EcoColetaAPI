using EcoColeta.Api.Models;
using EcoColeta.Api.ViewModels.Responses;

namespace EcoColeta.Api.Repositories;

public interface IRegistroResiduoRepository
{
    Task<PagedResponse<RegistroResiduo>> ListarAsync(int pagina, int tamanhoPagina, int? pontoColetaId, TipoResiduo? tipoResiduo);
    Task<RegistroResiduo?> ObterPorIdAsync(int id);
    Task<RegistroResiduo> CriarAsync(RegistroResiduo registro);
    Task<decimal> ObterTotalPesoAsync();
    Task<Dictionary<TipoResiduo, decimal>> ObterPesoPorTipoAsync();
}
