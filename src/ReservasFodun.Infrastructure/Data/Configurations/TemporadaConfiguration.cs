using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class TemporadaConfiguration : IEntityTypeConfiguration<Temporada>
{
    public void Configure(EntityTypeBuilder<Temporada> builder)
    {
        builder.ToTable("Temporadas");

        builder.HasKey(x => x.IdTemporada);

        builder.Property(x => x.IdTemporada)
            .HasColumnName("IdTemporada");

        builder.Property(x => x.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasColumnName("Descripcion")
            .HasMaxLength(500);

        builder.Property(x => x.EsAlta)
            .HasColumnName("EsTemporadaAlta")
            .IsRequired();

        builder.Property(x => x.EsEspecial)
            .HasColumnName("EsTarifaEspecial")
            .IsRequired();

        builder.Property(x => x.Activo)
            .HasColumnName("Activo")
            .IsRequired();
    }
}
