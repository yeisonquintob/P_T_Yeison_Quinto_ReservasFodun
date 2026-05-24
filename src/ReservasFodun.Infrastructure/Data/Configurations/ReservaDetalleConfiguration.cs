using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class ReservaDetalleConfiguration : IEntityTypeConfiguration<ReservaDetalle>
{
    public void Configure(EntityTypeBuilder<ReservaDetalle> builder)
    {
        builder.ToTable("ReservaDetalles");

        builder.HasKey(x => x.IdReservaDetalle);

        builder.Property(x => x.NumeroPersonas)
            .IsRequired();

        builder.Property(x => x.NumeroHabitaciones)
            .IsRequired();

        builder.Property(x => x.ValorUnitario)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ValorTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(x => x.Reserva)
            .WithMany(x => x.ReservaDetalles)
            .HasForeignKey(x => x.IdReserva)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Alojamiento)
            .WithMany(x => x.ReservaDetalles)
            .HasForeignKey(x => x.IdAlojamiento)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
