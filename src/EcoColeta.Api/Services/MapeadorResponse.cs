using EcoColeta.Api.Models;
using EcoColeta.Api.ViewModels.Responses;

namespace EcoColeta.Api.Services;

public static class MapeadorResponse
{
    public static PontoColetaResponse ParaResponse(PontoColeta ponto)
    {
        var percentual = ponto.CapacidadeMaximaKg > 0
            ? Math.Round(ponto.OcupacaoAtualKg / ponto.CapacidadeMaximaKg * 100, 2)
            : 0;

        return new PontoColetaResponse
        {
            Id = ponto.Id,
            Nome = ponto.Nome,
            Endereco = ponto.Endereco,
            Bairro = ponto.Bairro,
            Cidade = ponto.Cidade,
            Latitude = ponto.Latitude,
            Longitude = ponto.Longitude,
            TipoResiduoAceito = ponto.TipoResiduoAceito.ToString(),
            CapacidadeMaximaKg = ponto.CapacidadeMaximaKg,
            OcupacaoAtualKg = ponto.OcupacaoAtualKg,
            PercentualOcupacao = percentual,
            Ativo = ponto.Ativo,
            CriadoEm = ponto.CriadoEm
        };
    }

    public static RegistroResiduoResponse ParaResponse(RegistroResiduo registro)
    {
        return new RegistroResiduoResponse
        {
            Id = registro.Id,
            PontoColetaId = registro.PontoColetaId,
            NomePontoColeta = registro.PontoColeta?.Nome ?? string.Empty,
            TipoResiduo = registro.TipoResiduo.ToString(),
            PesoKg = registro.PesoKg,
            Origem = registro.Origem,
            Observacao = registro.Observacao,
            RegistradoEm = registro.RegistradoEm
        };
    }

    public static AlertaColetaResponse ParaResponse(AlertaColeta alerta)
    {
        return new AlertaColetaResponse
        {
            Id = alerta.Id,
            PontoColetaId = alerta.PontoColetaId,
            NomePontoColeta = alerta.PontoColeta?.Nome ?? string.Empty,
            Nivel = alerta.Nivel.ToString(),
            Mensagem = alerta.Mensagem,
            Resolvido = alerta.Resolvido,
            CriadoEm = alerta.CriadoEm,
            ResolvidoEm = alerta.ResolvidoEm
        };
    }

    public static DestinacaoResiduoResponse ParaResponse(DestinacaoResiduo destinacao)
    {
        return new DestinacaoResiduoResponse
        {
            Id = destinacao.Id,
            TipoResiduo = destinacao.TipoResiduo.ToString(),
            Descricao = destinacao.Descricao,
            InstrucoesDescarte = destinacao.InstrucoesDescarte,
            Reciclavel = destinacao.Reciclavel,
            RiscoAmbiental = destinacao.RiscoAmbiental
        };
    }

    public static PagedResponse<TResponse> ParaPaginacao<TEntity, TResponse>(
        PagedResponse<TEntity> paginacao,
        Func<TEntity, TResponse> mapeador)
    {
        return new PagedResponse<TResponse>
        {
            Itens = paginacao.Itens.Select(mapeador),
            PaginaAtual = paginacao.PaginaAtual,
            TamanhoPagina = paginacao.TamanhoPagina,
            TotalItens = paginacao.TotalItens,
            TotalPaginas = paginacao.TotalPaginas
        };
    }
}
