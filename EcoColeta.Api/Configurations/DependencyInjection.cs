using System.Text;
using EcoColeta.Api.Data;
using EcoColeta.Api.Repositories;
using EcoColeta.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace EcoColeta.Api.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddEcoColetaServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddDbContext<EcoColetaDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPontoColetaRepository, PontoColetaRepository>();
        services.AddScoped<IRegistroResiduoRepository, RegistroResiduoRepository>();
        services.AddScoped<IAlertaColetaRepository, AlertaColetaRepository>();
        services.AddScoped<IDestinacaoResiduoRepository, DestinacaoResiduoRepository>();
        services.AddScoped<IUsuarioSistemaRepository, UsuarioSistemaRepository>();

        services.AddScoped<IPontoColetaService, PontoColetaService>();
        services.AddScoped<IRegistroResiduoService, RegistroResiduoService>();
        services.AddScoped<IAlertaColetaService, AlertaColetaService>();
        services.AddScoped<IDestinacaoResiduoService, DestinacaoResiduoService>();
        services.AddScoped<IRelatorioImpactoService, RelatorioImpactoService>();
        services.AddScoped<IAuthService, AuthService>();

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("Configuração JWT não encontrada.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Emissor,
                    ValidAudience = jwtSettings.Audiencia,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.ChaveSecreta)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "EcoColeta API",
                Version = "v1",
                Description = "API RESTful para gestão de resíduos e reciclagem (ESG)."
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Informe o token JWT no formato: Bearer {seu_token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
