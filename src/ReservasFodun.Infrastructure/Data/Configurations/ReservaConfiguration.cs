using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
{
    public void Configure(EntityTypeBuilder<Reserva> builder)
    {
        builder.ToTable("Reservas");

        builder.HasKey(x => x.IdReserva);

        builder.Property(x => x.IdReserva)
            .HasColumnName("IdReserva");

        builder.Property(x => x.CodigoReserva)
            .HasColumnName("CodigoReserva")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.IdUsuario)
            .HasColumnName("UserId")
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.IdSede)
            .HasColumnName("IdSede")
            .IsRequired();

        builder.Property(x => x.IdEstadoReserva)
            .HasColumnName("IdEstadoReserva")
            .IsRequired();

        builder.Property(x => x.FechaLlegada)
            .HasColumnName("FechaLlegada")
            .IsRequired();

        builder.Property(x => x.FechaSalida)
            .HasColumnName("FechaSalida")
            .IsRequired();

        builder.Property(x => x.NumeroNoches)
            .HasColumnName("NumeroNoches")
            .IsRequired();

        builder.Property(x => x.NumeroPersonas)
            .HasColumnName("NumeroPersonas")
            .IsRequired();

        builder.Property(x => x.NumeroHabitaciones)
            .HasColumnName("NumeroHabitaciones")
            .IsRequired();

        builder.Property(x => x.ValorSubtotal)
            .HasColumnName("ValorSubtotal")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.ValorTotal)
            .HasColumnName("ValorTotal")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Observaciones)
            .HasColumnName("Observaciones")
            .HasMaxLength(1000);

        builder.Property(x => x.FechaCreacion)
            .HasColumnName("FechaCreacion")
            .IsRequired();

        builder.Property(x => x.FechaModificacion)
            .HasColumnName("FechaModificacion");

        builder.HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(x => x.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Sede)
            .WithMany("Reservas")
            .HasForeignKey(x => x.IdSede)
            .HasPrincipalKey(x => x.IdSede)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.EstadoReserva)
            .WithMany("Reservas")
            .HasForeignKey(x => x.IdEstadoReserva)
            .HasPrincipalKey(x => x.IdEstadoReserva)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
