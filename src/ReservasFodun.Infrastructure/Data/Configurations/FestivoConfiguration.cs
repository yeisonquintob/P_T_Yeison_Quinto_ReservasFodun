using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class FestivoConfiguration : IEntityTypeConfiguration<Festivo>
{
    public void Configure(EntityTypeBuilder<Festivo> builder)
    {
        builder.ToTable("Festivos");

        builder.HasKey(x => x.IdFestivo);

        builder.Property(x => x.Fecha)
            .IsRequired();

        builder.Property(x => x.Nombre)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Activo)
            .IsRequired();
    }
}
