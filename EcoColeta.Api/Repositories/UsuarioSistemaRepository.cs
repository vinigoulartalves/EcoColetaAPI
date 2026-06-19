using EcoColeta.Api.Data;
using EcoColeta.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoColeta.Api.Repositories;

public class UsuarioSistemaRepository : IUsuarioSistemaRepository
{
    private readonly AppDbContext _context;

    public UsuarioSistemaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UsuarioSistema?> ObterPorEmailAsync(string email)
    {
        return await _context.UsuariosSistema.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
    }
}
