using EcoColeta.Api.Repositories;
using EcoColeta.Api.ViewModels.Responses;

namespace EcoColeta.Api.Services;

public interface IRelatorioImpactoService
{
    Task<RelatorioImpactoResponse> ObterResumoAsync();
}

public class RelatorioImpactoService : IRelatorioImpactoService
{
    private readonly IRegistroResiduoRepository _registroRepository;
    private readonly IPontoColetaRepository _pontoRepository;
    private readonly IAlertaColetaRepository _alertaRepository;

    public RelatorioImpactoService(
        IRegistroResiduoRepository registroRepository,
        IPontoColetaRepository pontoRepository,
        IAlertaColetaRepository alertaRepository)
    {
        _registroRepository = registroRepository;
        _pontoRepository = pontoRepository;
        _alertaRepository = alertaRepository;
    }

    public async Task<RelatorioImpactoResponse> ObterResumoAsync()
    {
        var totalPeso = await _registroRepository.ObterTotalPesoAsync();
        var pesoPorTipo = await _registroRepository.ObterPesoPorTipoAsync();
        var pontosAtivos = await _pontoRepository.ContarAtivosAsync();
        var alertasAbertos = await _alertaRepository.ContarNaoResolvidosAsync();

        return new RelatorioImpactoResponse
        {
            TotalResiduosRegistradosKg = totalPeso,
            ResiduosPorTipo = pesoPorTipo.ToDictionary(k => k.Key.ToString(), v => v.Value),
            PontosColetaAtivos = pontosAtivos,
            AlertasAbertos = alertasAbertos,
            EstimativaImpactoPositivoKg = Math.Round(totalPeso * 0.75m, 2),
            GeradoEm = DateTime.UtcNow
        };
    }
}
