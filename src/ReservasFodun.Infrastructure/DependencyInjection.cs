using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Application.Interfaces.Services;
using ReservasFodun.Infrastructure.Email;
using ReservasFodun.Infrastructure.Repositories;

namespace ReservasFodun.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ============================================================
        // Configuración SMTP
        // ============================================================
        services.Configure<SmtpSettings>(
            configuration.GetSection("SmtpSettings"));

        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IEmailSender, IdentityEmailSender>();

        // ============================================================
        // Repositorios
        // ============================================================
        services.AddScoped<ISedeRepository, SedeRepository>();
        services.AddScoped<IAlojamientoRepository, AlojamientoRepository>();
        services.AddScoped<ITipoAlojamientoRepository, TipoAlojamientoRepository>();
        services.AddScoped<ITarifaRepository, TarifaRepository>();
        services.AddScoped<ITemporadaRepository, TemporadaRepository>();
        services.AddScoped<IReservaRepository, ReservaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        services.AddScoped<IServicioAdicionalRepository, ServicioAdicionalRepository>();

        return services;
    }
}



