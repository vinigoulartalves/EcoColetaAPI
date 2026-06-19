using EcoColeta.Api.Exceptions;
using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.ViewModels.Requests;
using EcoColeta.Api.ViewModels.Responses;

namespace EcoColeta.Api.Services;

public interface IPontoColetaService
{
    Task<PagedResponse<PontoColetaResponse>> ListarAsync(int pagina, int tamanhoPagina, string? cidade, string? bairro, TipoResiduo? tipoResiduo, bool? ativo);
    Task<PontoColetaResponse> ObterPorIdAsync(int id);
    Task<PontoColetaResponse> CriarAsync(CriarPontoColetaRequest request);
    Task<PontoColetaResponse> AtualizarAsync(int id, AtualizarPontoColetaRequest request);
}

public class PontoColetaService : IPontoColetaService
{
    private readonly IPontoColetaRepository _repository;

    public PontoColetaService(IPontoColetaRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResponse<PontoColetaResponse>> ListarAsync(int pagina, int tamanhoPagina, string? cidade, string? bairro, TipoResiduo? tipoResiduo, bool? ativo)
    {
        var resultado = await _repository.ListarAsync(pagina, tamanhoPagina, cidade, bairro, tipoResiduo, ativo);
        return MapeadorResponse.ParaPaginacao(resultado, MapeadorResponse.ParaResponse);
    }

    public async Task<PontoColetaResponse> ObterPorIdAsync(int id)
    {
        var ponto = await _repository.ObterPorIdAsync(id)
            ?? throw new NotFoundException($"Ponto de coleta com id {id} não encontrado.");

        return MapeadorResponse.ParaResponse(ponto);
    }

    public async Task<PontoColetaResponse> CriarAsync(CriarPontoColetaRequest request)
    {
        var ponto = new PontoColeta
        {
            Nome = request.Nome,
            Endereco = request.Endereco,
            Bairro = request.Bairro,
            Cidade = request.Cidade,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            TipoResiduoAceito = request.TipoResiduoAceito,
            CapacidadeMaximaKg = request.CapacidadeMaximaKg,
            OcupacaoAtualKg = 0,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        var criado = await _repository.CriarAsync(ponto);
        return MapeadorResponse.ParaResponse(criado);
    }

    public async Task<PontoColetaResponse> AtualizarAsync(int id, AtualizarPontoColetaRequest request)
    {
        var ponto = await _repository.ObterPorIdAsync(id)
            ?? throw new NotFoundException($"Ponto de coleta com id {id} não encontrado.");

        var pontoRastreado = new PontoColeta
        {
            Id = ponto.Id,
            Nome = request.Nome,
            Endereco = request.Endereco,
            Bairro = request.Bairro,
            Cidade = request.Cidade,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            TipoResiduoAceito = request.TipoResiduoAceito,
            CapacidadeMaximaKg = request.CapacidadeMaximaKg,
            OcupacaoAtualKg = ponto.OcupacaoAtualKg,
            Ativo = request.Ativo,
            CriadoEm = ponto.CriadoEm
        };

        if (pontoRastreado.OcupacaoAtualKg > pontoRastreado.CapacidadeMaximaKg)
            throw new BusinessException("A capacidade máxima não pode ser menor que a ocupação atual.");

        var atualizado = await _repository.AtualizarAsync(pontoRastreado);
        return MapeadorResponse.ParaResponse(atualizado);
    }
}
