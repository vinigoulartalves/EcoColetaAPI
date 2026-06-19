using EcoColeta.Api.Data;
using EcoColeta.Api.Exceptions;
using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.ViewModels.Responses;
using Microsoft.EntityFrameworkCore;

namespace EcoColeta.Api.Services;

public interface IAlertaColetaService
{
    Task<PaginacaoResponse<AlertaColetaResponse>> ListarAsync(int pagina, int tamanhoPagina, bool? resolvido, NivelAlerta? nivel);
    Task<AlertaColetaResponse> ObterPorIdAsync(int id);
    Task<int> RecalcularAlertasAsync();
    Task VerificarEGerarAlertaAsync(PontoColeta ponto);
}

public class AlertaColetaService : IAlertaColetaService
{
    private readonly IAlertaColetaRepository _repository;
    private readonly EcoColetaDbContext _context;

    public AlertaColetaService(IAlertaColetaRepository repository, EcoColetaDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<PaginacaoResponse<AlertaColetaResponse>> ListarAsync(int pagina, int tamanhoPagina, bool? resolvido, NivelAlerta? nivel)
    {
        var resultado = await _repository.ListarAsync(pagina, tamanhoPagina, resolvido, nivel);
        return MapeadorResponse.ParaPaginacao(resultado, MapeadorResponse.ParaResponse);
    }

    public async Task<AlertaColetaResponse> ObterPorIdAsync(int id)
    {
        var alerta = await _repository.ObterPorIdAsync(id)
            ?? throw new NotFoundException($"Alerta com id {id} não encontrado.");

        return MapeadorResponse.ParaResponse(alerta);
    }

    public async Task<int> RecalcularAlertasAsync()
    {
        var pontos = await _context.PontosColeta.Where(p => p.Ativo).ToListAsync();
        var alertasGerados = 0;

        foreach (var ponto in pontos)
        {
            var gerou = await VerificarEGerarAlertaInternoAsync(ponto);
            if (gerou) alertasGerados++;
        }

        return alertasGerados;
    }

    public async Task VerificarEGerarAlertaAsync(PontoColeta ponto)
    {
        await VerificarEGerarAlertaInternoAsync(ponto);
    }

    private async Task<bool> VerificarEGerarAlertaInternoAsync(PontoColeta ponto)
    {
        if (ponto.CapacidadeMaximaKg <= 0)
            return false;

        var percentual = ponto.OcupacaoAtualKg / ponto.CapacidadeMaximaKg * 100;

        if (percentual < 80)
        {
            await ResolverAlertasNaoResolvidosAsync(ponto.Id);
            return false;
        }

        var nivel = percentual >= 100 ? NivelAlerta.Critico : NivelAlerta.Atencao;
        var alertasExistentes = await _repository.ListarNaoResolvidosPorPontoAsync(ponto.Id);

        if (alertasExistentes.Any(a => a.Nivel == nivel))
            return false;

        foreach (var alertaAntigo in alertasExistentes.Where(a => a.Nivel != nivel))
        {
            alertaAntigo.Resolvido = true;
            alertaAntigo.ResolvidoEm = DateTime.UtcNow;
            await _repository.AtualizarAsync(alertaAntigo);
        }

        var mensagem = nivel == NivelAlerta.Critico
            ? $"Ponto '{ponto.Nome}' atingiu capacidade crítica ({percentual:F1}%). Coleta urgente necessária."
            : $"Ponto '{ponto.Nome}' próximo do limite ({percentual:F1}%). Programar coleta em breve.";

        await _repository.CriarAsync(new AlertaColeta
        {
            PontoColetaId = ponto.Id,
            Nivel = nivel,
            Mensagem = mensagem,
            Resolvido = false,
            CriadoEm = DateTime.UtcNow
        });

        return true;
    }

    private async Task ResolverAlertasNaoResolvidosAsync(int pontoColetaId)
    {
        var alertas = await _repository.ListarNaoResolvidosPorPontoAsync(pontoColetaId);
        foreach (var alerta in alertas)
        {
            alerta.Resolvido = true;
            alerta.ResolvidoEm = DateTime.UtcNow;
            await _repository.AtualizarAsync(alerta);
        }
    }
}
