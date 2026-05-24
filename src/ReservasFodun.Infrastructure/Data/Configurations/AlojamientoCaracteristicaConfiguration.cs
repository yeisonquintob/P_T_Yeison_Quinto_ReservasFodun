using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class AlojamientoCaracteristicaConfiguration : IEntityTypeConfiguration<AlojamientoCaracteristica>
{
    public void Configure(EntityTypeBuilder<AlojamientoCaracteristica> builder)
    {
        builder.ToTable("AlojamientoCaracteristicas");

        builder.HasKey(x => x.IdAlojamientoCaracteristica);

        builder.HasOne(x => x.Alojamiento)
            .WithMany(x => x.AlojamientoCaracteristicas)
            .HasForeignKey(x => x.IdAlojamiento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Caracteristica)
            .WithMany(x => x.AlojamientoCaracteristicas)
            .HasForeignKey(x => x.IdCaracteristica)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
