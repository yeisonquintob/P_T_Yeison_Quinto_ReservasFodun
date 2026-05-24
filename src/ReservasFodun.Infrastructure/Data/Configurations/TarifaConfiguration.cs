using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class TarifaConfiguration : IEntityTypeConfiguration<Tarifa>
{
    public void Configure(EntityTypeBuilder<Tarifa> builder)
    {
        builder.ToTable("Tarifas");

        builder.HasKey(x => x.IdTarifa);

        builder.Property(x => x.IdTarifa)
            .HasColumnName("IdTarifa");

        builder.Property(x => x.IdSede)
            .HasColumnName("IdSede");

        builder.Property(x => x.IdAlojamiento)
            .HasColumnName("IdAlojamiento");

        builder.Property(x => x.IdTemporada)
            .HasColumnName("IdTemporada")
            .IsRequired();

        builder.Property(x => x.NumeroHabitacionesTarifa)
            .HasColumnName("NumeroHabitacionesTarifa");

        builder.Property(x => x.PersonasIncluidas)
            .HasColumnName("PersonasIncluidas")
            .IsRequired();

        builder.Property(x => x.TarifaBase)
            .HasColumnName("TarifaBase")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ValorPersonaAdicional)
            .HasColumnName("ValorPersonaAdicional")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.EsTarifaEspecial)
            .HasColumnName("EsTarifaEspecial")
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasColumnName("Descripcion")
            .HasMaxLength(300);

        builder.Property(x => x.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        builder.HasOne(x => x.Sede)
            .WithMany(x => x.Tarifas)
            .HasForeignKey(x => x.IdSede)
            .HasPrincipalKey(x => x.IdSede)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Alojamiento)
            .WithMany(x => x.Tarifas)
            .HasForeignKey(x => x.IdAlojamiento)
            .HasPrincipalKey(x => x.IdAlojamiento)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Temporada)
            .WithMany(x => x.Tarifas)
            .HasForeignKey(x => x.IdTemporada)
            .HasPrincipalKey(x => x.IdTemporada)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
