using EcoColeta.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoColeta.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (context.Database.IsRelational())
            await context.Database.MigrateAsync();
        else
            await context.Database.EnsureCreatedAsync();

        if (await context.UsuariosSistema.AnyAsync())
            return;

        context.UsuariosSistema.Add(new UsuarioSistema
        {
            Nome = "Administrador",
            Email = "admin@ecocoleta.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin"
        });

        if (!await context.DestinacoesResiduos.AnyAsync())
        {
            context.DestinacoesResiduos.AddRange(
                new DestinacaoResiduo
                {
                    TipoResiduo = TipoResiduo.Plastico,
                    Descricao = "Plásticos recicláveis",
                    InstrucoesDescarte = "Lavar e secar antes do descarte. Separar por tipo (PET, PEAD).",
                    Reciclavel = true,
                    RiscoAmbiental = "Baixo quando reciclado corretamente"
                },
                new DestinacaoResiduo
                {
                    TipoResiduo = TipoResiduo.Papel,
                    Descricao = "Papéis e papelões",
                    InstrucoesDescarte = "Manter seco e livre de gordura. Dobrar para reduzir volume.",
                    Reciclavel = true,
                    RiscoAmbiental = "Baixo"
                },
                new DestinacaoResiduo
                {
                    TipoResiduo = TipoResiduo.Vidro,
                    Descricao = "Vidros em geral",
                    InstrucoesDescarte = "Separar por cor quando possível. Evitar quebras.",
                    Reciclavel = true,
                    RiscoAmbiental = "Baixo"
                },
                new DestinacaoResiduo
                {
                    TipoResiduo = TipoResiduo.Metal,
                    Descricao = "Metais ferrosos e não ferrosos",
                    InstrucoesDescarte = "Separar alumínio de ferro. Esvaziar latas.",
                    Reciclavel = true,
                    RiscoAmbiental = "Baixo"
                },
                new DestinacaoResiduo
                {
                    TipoResiduo = TipoResiduo.Organico,
                    Descricao = "Resíduos orgânicos",
                    InstrucoesDescarte = "Destinar à compostagem. Evitar mistura com recicláveis.",
                    Reciclavel = true,
                    RiscoAmbiental = "Médio — gera metano em aterros"
                },
                new DestinacaoResiduo
                {
                    TipoResiduo = TipoResiduo.Eletronico,
                    Descricao = "Equipamentos eletrônicos",
                    InstrucoesDescarte = "Levar a pontos de coleta especializados. Não descartar no lixo comum.",
                    Reciclavel = true,
                    RiscoAmbiental = "Alto — metais pesados e substâncias tóxicas"
                },
                new DestinacaoResiduo
                {
                    TipoResiduo = TipoResiduo.PilhasBaterias,
                    Descricao = "Pilhas e baterias",
                    InstrucoesDescarte = "Armazenar em recipiente seguro. Levar a coleta específica.",
                    Reciclavel = true,
                    RiscoAmbiental = "Alto — contaminação de solo e água"
                },
                new DestinacaoResiduo
                {
                    TipoResiduo = TipoResiduo.OleoCozinha,
                    Descricao = "Óleo de cozinha usado",
                    InstrucoesDescarte = "Armazenar em garrafa PET. Nunca despejar na pia ou no solo.",
                    Reciclavel = true,
                    RiscoAmbiental = "Alto — contamina corpos d'água"
                },
                new DestinacaoResiduo
                {
                    TipoResiduo = TipoResiduo.Outros,
                    Descricao = "Outros resíduos",
                    InstrucoesDescarte = "Consultar orientação local de descarte.",
                    Reciclavel = false,
                    RiscoAmbiental = "Variável"
                }
            );
        }

        if (!await context.PontosColeta.AnyAsync())
        {
            context.PontosColeta.AddRange(
                new PontoColeta
                {
                    Nome = "EcoPonto Centro",
                    Endereco = "Rua das Flores, 100",
                    Bairro = "Centro",
                    Cidade = "São Paulo",
                    Latitude = -23.5505,
                    Longitude = -46.6333,
                    TipoResiduoAceito = TipoResiduo.Plastico,
                    CapacidadeMaximaKg = 500,
                    OcupacaoAtualKg = 0
                },
                new PontoColeta
                {
                    Nome = "EcoPonto Jardins",
                    Endereco = "Av. Paulista, 500",
                    Bairro = "Jardins",
                    Cidade = "São Paulo",
                    Latitude = -23.5614,
                    Longitude = -46.6559,
                    TipoResiduoAceito = TipoResiduo.Papel,
                    CapacidadeMaximaKg = 300,
                    OcupacaoAtualKg = 0
                },
                new PontoColeta
                {
                    Nome = "EcoPonto Vila Mariana",
                    Endereco = "Rua Domingos de Morais, 200",
                    Bairro = "Vila Mariana",
                    Cidade = "São Paulo",
                    Latitude = -23.5890,
                    Longitude = -46.6342,
                    TipoResiduoAceito = TipoResiduo.Vidro,
                    CapacidadeMaximaKg = 400,
                    OcupacaoAtualKg = 0
                }
            );
        }

        await context.SaveChangesAsync();
    }
}
