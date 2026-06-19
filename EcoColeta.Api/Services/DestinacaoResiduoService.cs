using EcoColeta.Api.Exceptions;
using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.ViewModels.Requests;
using EcoColeta.Api.ViewModels.Responses;

namespace EcoColeta.Api.Services;

public interface IDestinacaoResiduoService
{
    Task<PaginacaoResponse<DestinacaoResiduoResponse>> ListarAsync(int pagina, int tamanhoPagina, bool? reciclavel);
    Task<DestinacaoResiduoResponse> ObterPorIdAsync(int id);
    Task<DestinacaoResiduoResponse> ObterPorTipoAsync(TipoResiduo tipoResiduo);
    Task<DestinacaoResiduoResponse> CriarAsync(CriarDestinacaoResiduoRequest request);
    Task<DestinacaoResiduoResponse> AtualizarAsync(int id, AtualizarDestinacaoResiduoRequest request);
    Task ExcluirAsync(int id);
}

public class DestinacaoResiduoService : IDestinacaoResiduoService
{
    private readonly IDestinacaoResiduoRepository _repository;

    public DestinacaoResiduoService(IDestinacaoResiduoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginacaoResponse<DestinacaoResiduoResponse>> ListarAsync(int pagina, int tamanhoPagina, bool? reciclavel)
    {
        var resultado = await _repository.ListarAsync(pagina, tamanhoPagina, reciclavel);
        return MapeadorResponse.ParaPaginacao(resultado, MapeadorResponse.ParaResponse);
    }

    public async Task<DestinacaoResiduoResponse> ObterPorIdAsync(int id)
    {
        var destinacao = await _repository.ObterPorIdAsync(id)
            ?? throw new NotFoundException($"Destinação com id {id} não encontrada.");

        return MapeadorResponse.ParaResponse(destinacao);
    }

    public async Task<DestinacaoResiduoResponse> ObterPorTipoAsync(TipoResiduo tipoResiduo)
    {
        var destinacao = await _repository.ObterPorTipoAsync(tipoResiduo)
            ?? throw new NotFoundException($"Destinação para o tipo {tipoResiduo} não encontrada.");

        return MapeadorResponse.ParaResponse(destinacao);
    }

    public async Task<DestinacaoResiduoResponse> CriarAsync(CriarDestinacaoResiduoRequest request)
    {
        var existente = await _repository.ObterPorTipoAsync(request.TipoResiduo);
        if (existente is not null)
            throw new BusinessException($"Já existe destinação cadastrada para o tipo {request.TipoResiduo}.");

        var destinacao = new DestinacaoResiduo
        {
            TipoResiduo = request.TipoResiduo,
            Descricao = request.Descricao,
            InstrucoesDescarte = request.InstrucoesDescarte,
            Reciclavel = request.Reciclavel,
            RiscoAmbiental = request.RiscoAmbiental
        };

        var criada = await _repository.CriarAsync(destinacao);
        return MapeadorResponse.ParaResponse(criada);
    }

    public async Task<DestinacaoResiduoResponse> AtualizarAsync(int id, AtualizarDestinacaoResiduoRequest request)
    {
        var destinacao = await _repository.ObterPorIdAsync(id)
            ?? throw new NotFoundException($"Destinação com id {id} não encontrada.");

        destinacao.Descricao = request.Descricao;
        destinacao.InstrucoesDescarte = request.InstrucoesDescarte;
        destinacao.Reciclavel = request.Reciclavel;
        destinacao.RiscoAmbiental = request.RiscoAmbiental;

        var atualizada = await _repository.AtualizarAsync(destinacao);
        return MapeadorResponse.ParaResponse(atualizada);
    }

    public async Task ExcluirAsync(int id)
    {
        var destinacao = await _repository.ObterPorIdAsync(id)
            ?? throw new NotFoundException($"Destinação com id {id} não encontrada.");

        await _repository.ExcluirAsync(destinacao);
    }
}
