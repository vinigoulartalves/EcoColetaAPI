using EcoColeta.Api.Models;

namespace EcoColeta.Api.Repositories;

public interface IUsuarioSistemaRepository
{
    Task<UsuarioSistema?> ObterPorEmailAsync(string email);
}
