using Microsoft.Extensions.DependencyInjection;
using ReservasFodun.Application.Interfaces.Services;
using ReservasFodun.Application.Services;

namespace ReservasFodun.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISedeService, SedeService>();
        services.AddScoped<IAlojamientoService, AlojamientoService>();
        services.AddScoped<IDisponibilidadService, DisponibilidadService>();
        services.AddScoped<ITarifaService, TarifaService>();
        services.AddScoped<IReservaService, ReservaService>();
        services.AddScoped<IUsuarioService, UsuarioService>();

        services.AddScoped<ITipoAlojamientoService, TipoAlojamientoService>();

        services.AddScoped<ITemporadaService, TemporadaService>();

        services.AddScoped<IServicioAdicionalService, ServicioAdicionalService>();

        return services;
    }
}




