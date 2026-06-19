using EcoColeta.Api.Data;
using EcoColeta.Api.Models;
using EcoColeta.Api.ViewModels.Responses;
using Microsoft.EntityFrameworkCore;

namespace EcoColeta.Api.Repositories;

public class DestinacaoResiduoRepository : IDestinacaoResiduoRepository
{
    private readonly EcoColetaDbContext _context;

    public DestinacaoResiduoRepository(EcoColetaDbContext context)
    {
        _context = context;
    }

    public async Task<PaginacaoResponse<DestinacaoResiduo>> ListarAsync(int pagina, int tamanhoPagina, bool? reciclavel)
    {
        var query = _context.DestinacoesResiduos.AsNoTracking().AsQueryable();

        if (reciclavel.HasValue)
            query = query.Where(d => d.Reciclavel == reciclavel.Value);

        var totalItens = await query.CountAsync();
        var itens = await query
            .OrderBy(d => d.TipoResiduo)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new PaginacaoResponse<DestinacaoResiduo>
        {
            Itens = itens,
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalItens = totalItens,
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina)
        };
    }

    public async Task<DestinacaoResiduo?> ObterPorIdAsync(int id)
    {
        return await _context.DestinacoesResiduos.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<DestinacaoResiduo?> ObterPorTipoAsync(TipoResiduo tipoResiduo)
    {
        return await _context.DestinacoesResiduos.AsNoTracking().FirstOrDefaultAsync(d => d.TipoResiduo == tipoResiduo);
    }

    public async Task<DestinacaoResiduo> CriarAsync(DestinacaoResiduo destinacao)
    {
        _context.DestinacoesResiduos.Add(destinacao);
        await _context.SaveChangesAsync();
        return destinacao;
    }

    public async Task<DestinacaoResiduo> AtualizarAsync(DestinacaoResiduo destinacao)
    {
        _context.DestinacoesResiduos.Update(destinacao);
        await _context.SaveChangesAsync();
        return destinacao;
    }

    public async Task ExcluirAsync(DestinacaoResiduo destinacao)
    {
        _context.DestinacoesResiduos.Remove(destinacao);
        await _context.SaveChangesAsync();
    }
}
