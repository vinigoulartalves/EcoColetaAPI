using EcoColeta.Api.Data;
using EcoColeta.Api.Exceptions;
using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.ViewModels.Requests;
using EcoColeta.Api.ViewModels.Responses;
using Microsoft.EntityFrameworkCore;

namespace EcoColeta.Api.Services;

public interface IRegistroResiduoService
{
    Task<PagedResponse<RegistroResiduoResponse>> ListarAsync(int pagina, int tamanhoPagina, int? pontoColetaId, TipoResiduo? tipoResiduo);
    Task<RegistroResiduoResponse> RegistrarAsync(RegistrarResiduoRequest request);
}

public class RegistroResiduoService : IRegistroResiduoService
{
    private readonly IRegistroResiduoRepository _registroRepository;
    private readonly AppDbContext _context;
    private readonly IAlertaColetaService _alertaService;

    public RegistroResiduoService(
        IRegistroResiduoRepository registroRepository,
        AppDbContext context,
        IAlertaColetaService alertaService)
    {
        _registroRepository = registroRepository;
        _context = context;
        _alertaService = alertaService;
    }

    public async Task<PagedResponse<RegistroResiduoResponse>> ListarAsync(int pagina, int tamanhoPagina, int? pontoColetaId, TipoResiduo? tipoResiduo)
    {
        var resultado = await _registroRepository.ListarAsync(pagina, tamanhoPagina, pontoColetaId, tipoResiduo);
        return MapeadorResponse.ParaPaginacao(resultado, MapeadorResponse.ParaResponse);
    }

    public async Task<RegistroResiduoResponse> RegistrarAsync(RegistrarResiduoRequest request)
    {
        var ponto = await _context.PontosColeta.FirstOrDefaultAsync(p => p.Id == request.PontoColetaId)
            ?? throw new NotFoundException($"Ponto de coleta com id {request.PontoColetaId} não encontrado.");

        if (!ponto.Ativo)
            throw new BusinessException("Não é possível registrar descarte em ponto de coleta inativo.");

        if (ponto.TipoResiduoAceito != request.TipoResiduo)
            throw new BusinessException($"Este ponto aceita apenas resíduos do tipo {ponto.TipoResiduoAceito}.");

        var registro = new RegistroResiduo
        {
            PontoColetaId = request.PontoColetaId,
            TipoResiduo = request.TipoResiduo,
            PesoKg = request.PesoKg,
            Origem = request.Origem,
            Observacao = request.Observacao,
            RegistradoEm = DateTime.UtcNow
        };

        ponto.OcupacaoAtualKg += request.PesoKg;

        _context.RegistrosResiduos.Add(registro);
        await _context.SaveChangesAsync();

        await _alertaService.VerificarEGerarAlertaAsync(ponto);

        registro.PontoColeta = ponto;
        return MapeadorResponse.ParaResponse(registro);
    }
}
