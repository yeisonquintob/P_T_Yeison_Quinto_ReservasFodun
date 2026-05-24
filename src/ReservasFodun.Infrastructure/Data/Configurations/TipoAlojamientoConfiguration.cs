using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class TipoAlojamientoConfiguration : IEntityTypeConfiguration<TipoAlojamiento>
{
    public void Configure(EntityTypeBuilder<TipoAlojamiento> builder)
    {
        builder.ToTable("TiposAlojamiento");

        builder.HasKey(x => x.IdTipoAlojamiento);

        builder.Property(x => x.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasMaxLength(500);

        builder.Property(x => x.Activo)
            .IsRequired();
    }
}
