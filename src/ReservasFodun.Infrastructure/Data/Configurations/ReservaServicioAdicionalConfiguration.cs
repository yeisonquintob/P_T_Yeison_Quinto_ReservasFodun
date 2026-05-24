using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class ReservaServicioAdicionalConfiguration : IEntityTypeConfiguration<ReservaServicioAdicional>
{
    public void Configure(EntityTypeBuilder<ReservaServicioAdicional> builder)
    {
        builder.ToTable("ReservaServiciosAdicionales");

        builder.HasKey(x => x.IdReservaServicioAdicional);

        builder.Property(x => x.Cantidad)
            .IsRequired();

        builder.Property(x => x.ValorUnitario)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ValorTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(x => x.Reserva)
            .WithMany(x => x.ReservaServiciosAdicionales)
            .HasForeignKey(x => x.IdReserva)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ServicioAdicional)
            .WithMany(x => x.ReservaServiciosAdicionales)
            .HasForeignKey(x => x.IdServicioAdicional)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
