using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class TipoSedeConfiguration : IEntityTypeConfiguration<TipoSede>
{
    public void Configure(EntityTypeBuilder<TipoSede> builder)
    {
        builder.ToTable("TiposSede");

        builder.HasKey(x => x.IdTipoSede);

        builder.Property(x => x.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasMaxLength(500);

        builder.Property(x => x.Activo)
            .IsRequired();
    }
}
