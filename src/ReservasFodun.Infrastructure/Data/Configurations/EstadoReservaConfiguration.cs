using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class EstadoReservaConfiguration : IEntityTypeConfiguration<EstadoReserva>
{
    public void Configure(EntityTypeBuilder<EstadoReserva> builder)
    {
        builder.ToTable("EstadosReserva");

        builder.HasKey(x => x.IdEstadoReserva);

        builder.Property(x => x.IdEstadoReserva)
            .HasColumnName("IdEstadoReserva");

        builder.Property(x => x.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.BloqueaDisponibilidad)
            .HasColumnName("BloqueoDisponibilidad")
            .IsRequired();
    }
}