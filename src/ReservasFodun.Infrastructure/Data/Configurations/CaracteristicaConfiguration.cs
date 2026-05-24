using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class CaracteristicaConfiguration : IEntityTypeConfiguration<Caracteristica>
{
    public void Configure(EntityTypeBuilder<Caracteristica> builder)
    {
        builder.ToTable("Caracteristicas");

        builder.HasKey(x => x.IdCaracteristica);

        builder.Property(x => x.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasMaxLength(500);

        builder.Property(x => x.Activo)
            .IsRequired();
    }
}
