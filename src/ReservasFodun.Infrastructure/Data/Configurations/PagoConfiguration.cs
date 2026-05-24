using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.ToTable("Pagos");

        builder.HasKey(x => x.IdPago);

        builder.Property(x => x.FechaPago)
            .IsRequired();

        builder.Property(x => x.ValorPagado)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.MetodoPago)
            .HasMaxLength(100);

        builder.Property(x => x.ReferenciaPago)
            .HasMaxLength(150);

        builder.Property(x => x.EstadoPago)
            .HasMaxLength(50);

        builder.Property(x => x.Observaciones)
            .HasMaxLength(500);

        builder.Property(x => x.UsuarioCreacion)
            .HasMaxLength(150);

        builder.Property(x => x.FechaCreacion)
            .IsRequired();

        builder.HasOne(x => x.Reserva)
            .WithMany(x => x.Pagos)
            .HasForeignKey(x => x.IdReserva)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
