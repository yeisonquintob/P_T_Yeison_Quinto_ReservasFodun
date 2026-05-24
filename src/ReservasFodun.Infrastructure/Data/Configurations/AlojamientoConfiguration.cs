using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class AlojamientoConfiguration : IEntityTypeConfiguration<Alojamiento>
{
    public void Configure(EntityTypeBuilder<Alojamiento> builder)
    {
        builder.ToTable("Alojamientos");

        builder.HasKey(x => x.IdAlojamiento);

        builder.Property(x => x.IdAlojamiento)
            .HasColumnName("IdAlojamiento");

        builder.Property(x => x.IdSede)
            .HasColumnName("IdSede")
            .IsRequired();

        builder.Property(x => x.IdTipoAlojamiento)
            .HasColumnName("IdTipoAlojamiento")
            .IsRequired();

        builder.Property(x => x.NumeroAlojamiento)
            .HasColumnName("Numero")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.NombreAlojamiento)
            .HasColumnName("Nombre")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasColumnName("Descripcion")
            .HasMaxLength(1000);

        builder.Property(x => x.NumeroHabitaciones)
            .HasColumnName("NumeroHabitaciones")
            .IsRequired();

        builder.Property(x => x.CapacidadMaxima)
            .HasColumnName("CapacidadMaxima")
            .IsRequired();

        builder.Property(x => x.NumeroHabitacionesTarifa)
            .HasColumnName("NumeroHabitacionesTarifa");

        builder.Property(x => x.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        builder.HasOne(x => x.Sede)
            .WithMany(x => x.Alojamientos)
            .HasForeignKey(x => x.IdSede)
            .HasPrincipalKey(x => x.IdSede)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TipoAlojamiento)
            .WithMany(x => x.Alojamientos)
            .HasForeignKey(x => x.IdTipoAlojamiento)
            .HasPrincipalKey(x => x.IdTipoAlojamiento)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
