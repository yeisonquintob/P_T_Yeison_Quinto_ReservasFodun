using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class PreguntaSecretaConfiguration : IEntityTypeConfiguration<PreguntaSecreta>
{
    public void Configure(EntityTypeBuilder<PreguntaSecreta> builder)
    {
        builder.ToTable("PreguntasSecretas");

        builder.HasKey(x => x.IdPreguntaSecreta);

        builder.Property(x => x.Pregunta)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Activo)
            .IsRequired();
    }
}
