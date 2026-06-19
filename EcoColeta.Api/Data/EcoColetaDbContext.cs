using EcoColeta.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoColeta.Api.Data;

public class EcoColetaDbContext : DbContext
{
    public EcoColetaDbContext(DbContextOptions<EcoColetaDbContext> options) : base(options)
    {
    }

    public DbSet<PontoColeta> PontosColeta => Set<PontoColeta>();
    public DbSet<RegistroResiduo> RegistrosResiduos => Set<RegistroResiduo>();
    public DbSet<AlertaColeta> AlertasColeta => Set<AlertaColeta>();
    public DbSet<DestinacaoResiduo> DestinacoesResiduos => Set<DestinacaoResiduo>();
    public DbSet<UsuarioSistema> UsuariosSistema => Set<UsuarioSistema>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PontoColeta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Endereco).HasMaxLength(250).IsRequired();
            entity.Property(e => e.Bairro).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Cidade).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CapacidadeMaximaKg).HasPrecision(10, 2);
            entity.Property(e => e.OcupacaoAtualKg).HasPrecision(10, 2);
            entity.HasIndex(e => e.Cidade);
            entity.HasIndex(e => e.TipoResiduoAceito);
        });

        modelBuilder.Entity<RegistroResiduo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PesoKg).HasPrecision(10, 2);
            entity.Property(e => e.Origem).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Observacao).HasMaxLength(500);
            entity.HasOne(e => e.PontoColeta)
                .WithMany(p => p.Registros)
                .HasForeignKey(e => e.PontoColetaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AlertaColeta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Mensagem).HasMaxLength(500).IsRequired();
            entity.HasOne(e => e.PontoColeta)
                .WithMany(p => p.Alertas)
                .HasForeignKey(e => e.PontoColetaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DestinacaoResiduo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descricao).HasMaxLength(250).IsRequired();
            entity.Property(e => e.InstrucoesDescarte).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.RiscoAmbiental).HasMaxLength(250).IsRequired();
            entity.HasIndex(e => e.TipoResiduo).IsUnique();
        });

        modelBuilder.Entity<UsuarioSistema>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.SenhaHash).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}
