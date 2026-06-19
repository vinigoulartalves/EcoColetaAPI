using EcoColeta.Api.Exceptions;
using EcoColeta.Api.Models;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.ViewModels.Requests;
using EcoColeta.Api.ViewModels.Responses;

namespace EcoColeta.Api.Services;

public interface IDestinacaoResiduoService
{
    Task<PagedResponse<DestinacaoResiduoResponse>> ListarAsync(int pagina, int tamanhoPagina);
    Task<DestinacaoResiduoResponse> ObterPorTipoAsync(TipoResiduo tipoResiduo);
    Task<DestinacaoResiduoResponse> CriarAsync(CriarDestinacaoResiduoRequest request);
}

public class DestinacaoResiduoService : IDestinacaoResiduoService
{
    private readonly IDestinacaoResiduoRepository _repository;

    public DestinacaoResiduoService(IDestinacaoResiduoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResponse<DestinacaoResiduoResponse>> ListarAsync(int pagina, int tamanhoPagina)
    {
        var resultado = await _repository.ListarAsync(pagina, tamanhoPagina, reciclavel: null);
        return MapeadorResponse.ParaPaginacao(resultado, MapeadorResponse.ParaResponse);
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
}
