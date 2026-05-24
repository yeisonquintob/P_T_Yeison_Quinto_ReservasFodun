using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReservasFodun.Domain.Entities;

namespace ReservasFodun.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Departamento> Departamentos { get; set; }
    public DbSet<Municipio> Municipios { get; set; }
    public DbSet<TipoSede> TiposSede { get; set; }
    public DbSet<Sede> Sedes { get; set; }

    public DbSet<TipoAlojamiento> TiposAlojamiento { get; set; }
    public DbSet<Alojamiento> Alojamientos { get; set; }
    public DbSet<Caracteristica> Caracteristicas { get; set; }
    public DbSet<AlojamientoCaracteristica> AlojamientoCaracteristicas { get; set; }

    public DbSet<Temporada> Temporadas { get; set; }
    public DbSet<Tarifa> Tarifas { get; set; }

    public DbSet<EstadoReserva> EstadosReserva { get; set; }
    public DbSet<ServicioAdicional> ServiciosAdicionales { get; set; }
    public DbSet<Festivo> Festivos { get; set; }
    public DbSet<PreguntaSecreta> PreguntasSecretas { get; set; }

    public DbSet<PerfilUsuario> PerfilUsuario { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<ReservaDetalle> ReservaDetalles { get; set; }
    public DbSet<ReservaServicioAdicional> ReservaServiciosAdicionales { get; set; }
    public DbSet<Pago> Pagos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}