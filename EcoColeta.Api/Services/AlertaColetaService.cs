using EcoColeta.Api.Data;
using EcoColeta.Api.Exceptions;
using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.ViewModels.Responses;
using Microsoft.EntityFrameworkCore;

namespace EcoColeta.Api.Services;

public interface IAlertaColetaService
{
    Task<PagedResponse<AlertaColetaResponse>> ListarAsync(int pagina, int tamanhoPagina, bool? resolvido, NivelAlerta? nivel, int? pontoColetaId);
    Task<AlertaColetaResponse> ResolverAsync(int id);
    Task<int> RecalcularAlertasAsync();
    Task VerificarEGerarAlertaAsync(PontoColeta ponto);
}

public class AlertaColetaService : IAlertaColetaService
{
    private readonly IAlertaColetaRepository _repository;
    private readonly AppDbContext _context;

    public AlertaColetaService(IAlertaColetaRepository repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<PagedResponse<AlertaColetaResponse>> ListarAsync(int pagina, int tamanhoPagina, bool? resolvido, NivelAlerta? nivel, int? pontoColetaId)
    {
        var resultado = await _repository.ListarAsync(pagina, tamanhoPagina, resolvido, nivel, pontoColetaId);
        return MapeadorResponse.ParaPaginacao(resultado, MapeadorResponse.ParaResponse);
    }

    public async Task<AlertaColetaResponse> ResolverAsync(int id)
    {
        var alerta = await _repository.ObterPorIdParaAtualizacaoAsync(id)
            ?? throw new NotFoundException($"Alerta com id {id} não encontrado.");

        if (alerta.Resolvido)
            throw new BusinessException("Este alerta já está resolvido.");

        alerta.Resolvido = true;
        alerta.ResolvidoEm = DateTime.UtcNow;

        var atualizado = await _repository.AtualizarAsync(alerta);
        atualizado.PontoColeta = await _context.PontosColeta
            .AsNoTracking()
            .FirstAsync(p => p.Id == atualizado.PontoColetaId);

        return MapeadorResponse.ParaResponse(atualizado);
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
