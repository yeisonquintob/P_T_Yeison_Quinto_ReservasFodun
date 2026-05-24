using Microsoft.EntityFrameworkCore;
using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Domain.Entities;
using ReservasFodun.Infrastructure.Data;

namespace ReservasFodun.Infrastructure.Repositories;

public class TipoAlojamientoRepository : ITipoAlojamientoRepository
{
    private readonly ApplicationDbContext _context;

    public TipoAlojamientoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TipoAlojamientoAdminDto>> ObtenerTiposAlojamientoAdminAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.TiposAlojamiento
            .AsNoTracking()
            .Select(x => new TipoAlojamientoAdminDto
            {
                IdTipoAlojamiento = x.IdTipoAlojamiento,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                Activo = x.Activo,
                TotalAlojamientos = _context.Alojamientos
                    .Count(a => a.IdTipoAlojamiento == x.IdTipoAlojamiento)
            })
            .OrderByDescending(x => x.Activo)
            .ThenBy(x => x.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<TipoAlojamientoAdminDto?> ObtenerTipoAlojamientoAdminPorIdAsync(
        int idTipoAlojamiento,
        CancellationToken cancellationToken = default)
    {
        return await _context.TiposAlojamiento
            .AsNoTracking()
            .Where(x => x.IdTipoAlojamiento == idTipoAlojamiento)
            .Select(x => new TipoAlojamientoAdminDto
            {
                IdTipoAlojamiento = x.IdTipoAlojamiento,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                Activo = x.Activo,
                TotalAlojamientos = _context.Alojamientos
                    .Count(a => a.IdTipoAlojamiento == x.IdTipoAlojamiento)
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CrearTipoAlojamientoAsync(
        TipoAlojamientoFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        var tipoAlojamiento = new TipoAlojamiento
        {
            Nombre = request.Nombre.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim(),
            Activo = request.Activo
        };

        _context.TiposAlojamiento.Add(tipoAlojamiento);

        await _context.SaveChangesAsync(cancellationToken);

        return tipoAlojamiento.IdTipoAlojamiento;
    }

    public async Task<bool> ActualizarTipoAlojamientoAsync(
        TipoAlojamientoFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        var tipoAlojamiento = await _context.TiposAlojamiento
            .FirstOrDefaultAsync(x => x.IdTipoAlojamiento == request.IdTipoAlojamiento, cancellationToken);

        if (tipoAlojamiento is null)
            return false;

        tipoAlojamiento.Nombre = request.Nombre.Trim();
        tipoAlojamiento.Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim();
        tipoAlojamiento.Activo = request.Activo;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> InactivarTipoAlojamientoAsync(
        int idTipoAlojamiento,
        CancellationToken cancellationToken = default)
    {
        var tipoAlojamiento = await _context.TiposAlojamiento
            .FirstOrDefaultAsync(x => x.IdTipoAlojamiento == idTipoAlojamiento, cancellationToken);

        if (tipoAlojamiento is null)
            return false;

        tipoAlojamiento.Activo = false;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ActivarTipoAlojamientoAsync(
        int idTipoAlojamiento,
        CancellationToken cancellationToken = default)
    {
        var tipoAlojamiento = await _context.TiposAlojamiento
            .FirstOrDefaultAsync(x => x.IdTipoAlojamiento == idTipoAlojamiento, cancellationToken);

        if (tipoAlojamiento is null)
            return false;

        tipoAlojamiento.Activo = true;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
