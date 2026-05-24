using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class ServicioAdicionalConfiguration : IEntityTypeConfiguration<ServicioAdicional>
{
    public void Configure(EntityTypeBuilder<ServicioAdicional> builder)
    {
        builder.ToTable("ServiciosAdicionales");

        builder.HasKey(x => x.IdServicioAdicional);

        builder.Property(x => x.IdServicioAdicional)
            .HasColumnName("IdServicio");

        builder.Property(x => x.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasColumnName("Descripcion")
            .HasMaxLength(500);

        builder.Property(x => x.Valor)
            .HasColumnName("Valor")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.PorPersona)
            .HasColumnName("PorPersona")
            .IsRequired();

        builder.Property(x => x.PorNoche)
            .HasColumnName("PorNoche")
            .IsRequired();

        builder.Property(x => x.Activo)
            .HasColumnName("Activo")
            .IsRequired();
    }
}
