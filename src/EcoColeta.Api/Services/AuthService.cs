using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcoColeta.Api.Configurations;
using EcoColeta.Api.Exceptions;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.ViewModels.Requests;
using EcoColeta.Api.ViewModels.Responses;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EcoColeta.Api.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly IUsuarioSistemaRepository _usuarioRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUsuarioSistemaRepository usuarioRepository, IOptions<JwtSettings> jwtSettings)
    {
        _usuarioRepository = usuarioRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email)
            ?? throw new UnauthorizedAppException("E-mail ou senha inválidos.");

        if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            throw new UnauthorizedAppException("E-mail ou senha inválidos.");

        var expiraEm = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiracaoMinutos);
        var token = GerarToken(usuario, expiraEm);

        return new LoginResponse
        {
            Token = token,
            ExpiraEm = expiraEm,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Role = usuario.Role
        };
    }

    private string GerarToken(Models.UsuarioSistema usuario, DateTime expiraEm)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.ChaveSecreta));
        var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Emissor,
            audience: _jwtSettings.Audiencia,
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
