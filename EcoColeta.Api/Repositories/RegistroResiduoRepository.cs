using EcoColeta.Api.Data;
using EcoColeta.Api.Models;
using EcoColeta.Api.ViewModels.Responses;
using Microsoft.EntityFrameworkCore;

namespace EcoColeta.Api.Repositories;

public class RegistroResiduoRepository : IRegistroResiduoRepository
{
    private readonly AppDbContext _context;

    public RegistroResiduoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<RegistroResiduo>> ListarAsync(int pagina, int tamanhoPagina, int? pontoColetaId, TipoResiduo? tipoResiduo)
    {
        var query = _context.RegistrosResiduos
            .AsNoTracking()
            .Include(r => r.PontoColeta)
            .AsQueryable();

        if (pontoColetaId.HasValue)
            query = query.Where(r => r.PontoColetaId == pontoColetaId.Value);

        if (tipoResiduo.HasValue)
            query = query.Where(r => r.TipoResiduo == tipoResiduo.Value);

        var totalItens = await query.CountAsync();
        var itens = await query
            .OrderByDescending(r => r.RegistradoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new PagedResponse<RegistroResiduo>
        {
            Itens = itens,
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalItens = totalItens,
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina)
        };
    }

    public async Task<RegistroResiduo?> ObterPorIdAsync(int id)
    {
        return await _context.RegistrosResiduos
            .AsNoTracking()
            .Include(r => r.PontoColeta)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<RegistroResiduo> CriarAsync(RegistroResiduo registro)
    {
        _context.RegistrosResiduos.Add(registro);
        await _context.SaveChangesAsync();
        return registro;
    }

    public async Task<decimal> ObterTotalPesoAsync()
    {
        return await _context.RegistrosResiduos.AsNoTracking().SumAsync(r => r.PesoKg);
    }

    public async Task<Dictionary<TipoResiduo, decimal>> ObterPesoPorTipoAsync()
    {
        return await _context.RegistrosResiduos
            .AsNoTracking()
            .GroupBy(r => r.TipoResiduo)
            .Select(g => new { Tipo = g.Key, Peso = g.Sum(r => r.PesoKg) })
            .ToDictionaryAsync(x => x.Tipo, x => x.Peso);
    }
}
