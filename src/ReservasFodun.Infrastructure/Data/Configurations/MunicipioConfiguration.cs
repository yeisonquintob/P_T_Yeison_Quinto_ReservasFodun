using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class MunicipioConfiguration : IEntityTypeConfiguration<Municipio>
{
    public void Configure(EntityTypeBuilder<Municipio> builder)
    {
        builder.ToTable("Municipios");

        builder.HasKey(x => x.IdMunicipio);

        builder.Property(x => x.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Activo)
            .IsRequired();

        builder.HasOne(x => x.Departamento)
            .WithMany(x => x.Municipios)
            .HasForeignKey(x => x.IdDepartamento)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
