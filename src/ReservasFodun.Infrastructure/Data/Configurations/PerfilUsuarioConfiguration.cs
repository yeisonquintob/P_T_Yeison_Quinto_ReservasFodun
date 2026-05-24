using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data.Configurations;

public class PerfilUsuarioConfiguration : IEntityTypeConfiguration<PerfilUsuario>
{
    public void Configure(EntityTypeBuilder<PerfilUsuario> builder)
    {
        builder.ToTable("PerfilUsuario");

        builder.HasKey(x => x.IdPerfilUsuario);

        builder.Property(x => x.IdPerfilUsuario)
            .HasColumnName("IdPerfil");

        builder.Property(x => x.IdUsuario)
            .HasColumnName("UserId")
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.TipoDocumento)
            .HasColumnName("TipoDocumento")
            .HasMaxLength(10);

        builder.Property(x => x.NumeroDocumento)
            .HasColumnName("NroDocumento")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Nombres)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Apellidos)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Telefono)
            .HasColumnName("Celular")
            .HasMaxLength(30);

        builder.Property(x => x.Direccion)
            .HasColumnName("DireccionResidencia")
            .HasMaxLength(250);

        builder.Property(x => x.Activo)
            .IsRequired();

        builder.Property(x => x.FechaCreacion)
            .IsRequired();

        builder.Property(x => x.FechaModificacion);

        builder.HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(x => x.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);
    }
}