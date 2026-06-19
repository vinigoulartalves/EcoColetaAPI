using EcoColeta.Api.Data;
using EcoColeta.Api.Models;
using EcoColeta.Api.ViewModels.Responses;
using Microsoft.EntityFrameworkCore;

namespace EcoColeta.Api.Repositories;

public class PontoColetaRepository : IPontoColetaRepository
{
    private readonly EcoColetaDbContext _context;

    public PontoColetaRepository(EcoColetaDbContext context)
    {
        _context = context;
    }

    public async Task<PaginacaoResponse<PontoColeta>> ListarAsync(int pagina, int tamanhoPagina, string? cidade, TipoResiduo? tipoResiduo, bool? ativo)
    {
        var query = _context.PontosColeta.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(cidade))
            query = query.Where(p => p.Cidade.Contains(cidade));

        if (tipoResiduo.HasValue)
            query = query.Where(p => p.TipoResiduoAceito == tipoResiduo.Value);

        if (ativo.HasValue)
            query = query.Where(p => p.Ativo == ativo.Value);

        var totalItens = await query.CountAsync();
        var itens = await query
            .OrderBy(p => p.Nome)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new PaginacaoResponse<PontoColeta>
        {
            Itens = itens,
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalItens = totalItens,
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina)
        };
    }

    public async Task<PontoColeta?> ObterPorIdAsync(int id)
    {
        return await _context.PontosColeta.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PontoColeta> CriarAsync(PontoColeta pontoColeta)
    {
        _context.PontosColeta.Add(pontoColeta);
        await _context.SaveChangesAsync();
        return pontoColeta;
    }

    public async Task<PontoColeta> AtualizarAsync(PontoColeta pontoColeta)
    {
        _context.PontosColeta.Update(pontoColeta);
        await _context.SaveChangesAsync();
        return pontoColeta;
    }

    public async Task<IEnumerable<PontoColeta>> ListarTodosAtivosAsync()
    {
        return await _context.PontosColeta.AsNoTracking().Where(p => p.Ativo).ToListAsync();
    }
}
