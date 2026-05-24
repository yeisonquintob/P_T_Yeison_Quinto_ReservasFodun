using Microsoft.EntityFrameworkCore;
using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Domain.Entities;
using ReservasFodun.Infrastructure.Data;

namespace ReservasFodun.Infrastructure.Repositories;

public class ServicioAdicionalRepository : IServicioAdicionalRepository
{
    private readonly ApplicationDbContext _context;

    public ServicioAdicionalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ServicioAdicionalAdminDto>> ObtenerServiciosAdicionalesAdminAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.ServiciosAdicionales
            .AsNoTracking()
            .Select(x => new ServicioAdicionalAdminDto
            {
                IdServicioAdicional = x.IdServicioAdicional,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                Valor = x.Valor,
                PorPersona = x.PorPersona,
                PorNoche = x.PorNoche,
                Activo = x.Activo
            })
            .OrderByDescending(x => x.Activo)
            .ThenBy(x => x.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<ServicioAdicionalAdminDto?> ObtenerServicioAdicionalAdminPorIdAsync(
        int idServicioAdicional,
        CancellationToken cancellationToken = default)
    {
        return await _context.ServiciosAdicionales
            .AsNoTracking()
            .Where(x => x.IdServicioAdicional == idServicioAdicional)
            .Select(x => new ServicioAdicionalAdminDto
            {
                IdServicioAdicional = x.IdServicioAdicional,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                Valor = x.Valor,
                PorPersona = x.PorPersona,
                PorNoche = x.PorNoche,
                Activo = x.Activo
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CrearServicioAdicionalAsync(
        ServicioAdicionalFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        var servicio = new ServicioAdicional
        {
            Nombre = request.Nombre.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim(),
            Valor = request.Valor,
            PorPersona = request.PorPersona,
            PorNoche = request.PorNoche,
            Activo = request.Activo
        };

        _context.ServiciosAdicionales.Add(servicio);

        await _context.SaveChangesAsync(cancellationToken);

        return servicio.IdServicioAdicional;
    }

    public async Task<bool> ActualizarServicioAdicionalAsync(
        ServicioAdicionalFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        var servicio = await _context.ServiciosAdicionales
            .FirstOrDefaultAsync(x => x.IdServicioAdicional == request.IdServicioAdicional, cancellationToken);

        if (servicio is null)
            return false;

        servicio.Nombre = request.Nombre.Trim();
        servicio.Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim();
        servicio.Valor = request.Valor;
        servicio.PorPersona = request.PorPersona;
        servicio.PorNoche = request.PorNoche;
        servicio.Activo = request.Activo;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> InactivarServicioAdicionalAsync(
        int idServicioAdicional,
        CancellationToken cancellationToken = default)
    {
        var servicio = await _context.ServiciosAdicionales
            .FirstOrDefaultAsync(x => x.IdServicioAdicional == idServicioAdicional, cancellationToken);

        if (servicio is null)
            return false;

        servicio.Activo = false;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ActivarServicioAdicionalAsync(
        int idServicioAdicional,
        CancellationToken cancellationToken = default)
    {
        var servicio = await _context.ServiciosAdicionales
            .FirstOrDefaultAsync(x => x.IdServicioAdicional == idServicioAdicional, cancellationToken);

        if (servicio is null)
            return false;

        servicio.Activo = true;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
