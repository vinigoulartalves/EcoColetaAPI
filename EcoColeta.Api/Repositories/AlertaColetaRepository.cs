using EcoColeta.Api.Data;
using EcoColeta.Api.Models;
using EcoColeta.Api.ViewModels.Responses;
using Microsoft.EntityFrameworkCore;

namespace EcoColeta.Api.Repositories;

public class AlertaColetaRepository : IAlertaColetaRepository
{
    private readonly EcoColetaDbContext _context;

    public AlertaColetaRepository(EcoColetaDbContext context)
    {
        _context = context;
    }

    public async Task<PaginacaoResponse<AlertaColeta>> ListarAsync(int pagina, int tamanhoPagina, bool? resolvido, NivelAlerta? nivel)
    {
        var query = _context.AlertasColeta
            .AsNoTracking()
            .Include(a => a.PontoColeta)
            .AsQueryable();

        if (resolvido.HasValue)
            query = query.Where(a => a.Resolvido == resolvido.Value);

        if (nivel.HasValue)
            query = query.Where(a => a.Nivel == nivel.Value);

        var totalItens = await query.CountAsync();
        var itens = await query
            .OrderByDescending(a => a.CriadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new PaginacaoResponse<AlertaColeta>
        {
            Itens = itens,
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalItens = totalItens,
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina)
        };
    }

    public async Task<AlertaColeta?> ObterPorIdAsync(int id)
    {
        return await _context.AlertasColeta
            .AsNoTracking()
            .Include(a => a.PontoColeta)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<AlertaColeta> CriarAsync(AlertaColeta alerta)
    {
        _context.AlertasColeta.Add(alerta);
        await _context.SaveChangesAsync();
        return alerta;
    }

    public async Task<AlertaColeta> AtualizarAsync(AlertaColeta alerta)
    {
        _context.AlertasColeta.Update(alerta);
        await _context.SaveChangesAsync();
        return alerta;
    }

    public async Task<IEnumerable<AlertaColeta>> ListarNaoResolvidosPorPontoAsync(int pontoColetaId)
    {
        return await _context.AlertasColeta
            .Where(a => a.PontoColetaId == pontoColetaId && !a.Resolvido)
            .ToListAsync();
    }

    public async Task<int> ContarNaoResolvidosAsync()
    {
        return await _context.AlertasColeta.AsNoTracking().CountAsync(a => !a.Resolvido);
    }

    public async Task<int> ContarCriticosNaoResolvidosAsync()
    {
        return await _context.AlertasColeta
            .AsNoTracking()
            .CountAsync(a => !a.Resolvido && a.Nivel == NivelAlerta.Critico);
    }

    public async Task<int> ContarPontosEmAlertaAsync()
    {
        return await _context.AlertasColeta
            .AsNoTracking()
            .Where(a => !a.Resolvido)
            .Select(a => a.PontoColetaId)
            .Distinct()
            .CountAsync();
    }
}
