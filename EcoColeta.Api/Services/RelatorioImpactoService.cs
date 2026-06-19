using EcoColeta.Api.Repositories;
using EcoColeta.Api.ViewModels.Responses;

namespace EcoColeta.Api.Services;

public interface IRelatorioImpactoService
{
    Task<RelatorioImpactoResponse> GerarAsync();
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

    public async Task<RelatorioImpactoResponse> GerarAsync()
    {
        var totalPeso = await _registroRepository.ObterTotalPesoAsync();
        var pesoPorTipo = await _registroRepository.ObterPesoPorTipoAsync();
        var pontos = (await _pontoRepository.ListarTodosAtivosAsync()).ToList();
        var pontosEmAlerta = await _alertaRepository.ContarPontosEmAlertaAsync();
        var alertasCriticos = await _alertaRepository.ContarCriticosNaoResolvidosAsync();

        var estimativaImpacto = totalPeso * 0.75m;

        return new RelatorioImpactoResponse
        {
            TotalResiduosRegistradosKg = totalPeso,
            TotalPontosColeta = pontos.Count,
            PontosAtivos = pontos.Count(p => p.Ativo),
            PontosEmAlerta = pontosEmAlerta,
            AlertasCriticos = alertasCriticos,
            EstimativaImpactoPositivoKg = Math.Round(estimativaImpacto, 2),
            ResiduosPorTipo = pesoPorTipo.ToDictionary(k => k.Key.ToString(), v => v.Value),
            GeradoEm = DateTime.UtcNow
        };
    }
}
