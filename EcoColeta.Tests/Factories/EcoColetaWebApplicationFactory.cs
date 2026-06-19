using EcoColeta.Api.Data;
using EcoColeta.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EcoColeta.Tests.Factories;

public class EcoColetaWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<EcoColetaDbContext>));
            services.AddDbContext<EcoColetaDbContext>(options =>
                options.UseInMemoryDatabase("EcoColetaTestDb"));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<EcoColetaDbContext>();
            context.Database.EnsureCreated();
            SeedTestData(context);
        });
    }

    private static void SeedTestData(EcoColetaDbContext context)
    {
        if (context.PontosColeta.Any()) return;

        context.UsuariosSistema.Add(new UsuarioSistema
        {
            Nome = "Admin Teste",
            Email = "admin@teste.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin"
        });

        context.PontosColeta.Add(new PontoColeta
        {
            Nome = "Ponto Teste",
            Endereco = "Rua Teste, 1",
            Bairro = "Centro",
            Cidade = "TestCity",
            Latitude = -23.5,
            Longitude = -46.6,
            TipoResiduoAceito = TipoResiduo.Plastico,
            CapacidadeMaximaKg = 100,
            OcupacaoAtualKg = 0,
            Ativo = true
        });

        context.DestinacoesResiduos.Add(new DestinacaoResiduo
        {
            TipoResiduo = TipoResiduo.Plastico,
            Descricao = "Plástico teste",
            InstrucoesDescarte = "Separar e lavar",
            Reciclavel = true,
            RiscoAmbiental = "Baixo"
        });

        context.SaveChanges();
    }
}
