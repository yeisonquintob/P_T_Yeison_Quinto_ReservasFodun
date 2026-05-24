using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class SedeConfiguration : IEntityTypeConfiguration<Sede>
{
    public void Configure(EntityTypeBuilder<Sede> builder)
    {
        builder.ToTable("Sedes");

        builder.HasKey(x => x.IdSede);

        builder.Property(x => x.NombreSede)
            .HasColumnName("Nombre")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.NombreCorto)
            .HasMaxLength(100);

        builder.Property(x => x.Direccion)
            .HasMaxLength(250);

        builder.Property(x => x.Descripcion)
            .HasMaxLength(1000);

        builder.Property(x => x.CapacidadTotal)
            .IsRequired();

        builder.Property(x => x.Activo)
            .IsRequired();

        builder.HasOne(x => x.TipoSede)
            .WithMany(x => x.Sedes)
            .HasForeignKey(x => x.IdTipoSede)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Municipio)
            .WithMany(x => x.Sedes)
            .HasForeignKey(x => x.IdMunicipio)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
