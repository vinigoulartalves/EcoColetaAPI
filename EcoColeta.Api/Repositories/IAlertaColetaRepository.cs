using EcoColeta.Api.Models;
using EcoColeta.Api.ViewModels.Responses;

namespace EcoColeta.Api.Repositories;

public interface IAlertaColetaRepository
{
    Task<PaginacaoResponse<AlertaColeta>> ListarAsync(int pagina, int tamanhoPagina, bool? resolvido, NivelAlerta? nivel);
    Task<AlertaColeta?> ObterPorIdAsync(int id);
    Task<AlertaColeta> CriarAsync(AlertaColeta alerta);
    Task<AlertaColeta> AtualizarAsync(AlertaColeta alerta);
    Task<IEnumerable<AlertaColeta>> ListarNaoResolvidosPorPontoAsync(int pontoColetaId);
    Task<int> ContarNaoResolvidosAsync();
    Task<int> ContarCriticosNaoResolvidosAsync();
    Task<int> ContarPontosEmAlertaAsync();
}
