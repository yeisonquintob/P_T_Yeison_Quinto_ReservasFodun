using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.DTOs.Alojamientos;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Domain.Entities;
using ReservasFodun.Infrastructure.Data;

namespace ReservasFodun.Infrastructure.Repositories;

public class AlojamientoRepository : IAlojamientoRepository
{
    private readonly ApplicationDbContext _context;

    public AlojamientoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AlojamientoDto>> ObtenerAlojamientosPorSedeAsync(
        int idSede,
        CancellationToken cancellationToken = default)
    {
        return await _context.Alojamientos
            .AsNoTracking()
            .Include(x => x.Sede)
            .Include(x => x.TipoAlojamiento)
            .Where(x => x.IdSede == idSede && x.Activo)
            .Select(x => new AlojamientoDto
            {
                IdAlojamiento = x.IdAlojamiento,
                IdSede = x.IdSede,
                NombreSede = x.Sede != null ? x.Sede.NombreSede : string.Empty,
                NumeroAlojamiento = x.NumeroAlojamiento,
                NombreAlojamiento = x.NombreAlojamiento,
                TipoAlojamiento = x.TipoAlojamiento != null ? x.TipoAlojamiento.Nombre : null,
                Descripcion = x.Descripcion,
                NumeroHabitaciones = x.NumeroHabitaciones,
                CapacidadMaxima = x.CapacidadMaxima,
                Activo = x.Activo
            })
            .OrderBy(x => x.NombreAlojamiento)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AlojamientoDisponibleDto>> ConsultarDisponibilidadPorFechaAsync(
        DateTime fechaLlegada,
        DateTime fechaSalida,
        int? idSede,
        CancellationToken cancellationToken = default)
    {
        var sql = @"
            EXEC sp_ConsultarAlojamientosDisponiblesPorFecha
                @p_FechaLlegada,
                @p_FechaSalida,
                @p_IdSede";

        var parametros = new[]
        {
            new SqlParameter("@p_FechaLlegada", fechaLlegada),
            new SqlParameter("@p_FechaSalida", fechaSalida),
            new SqlParameter("@p_IdSede", (object?)idSede ?? DBNull.Value)
        };

        return await _context.Database
            .SqlQueryRaw<AlojamientoDisponibleDto>(sql, parametros)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AlojamientoDisponibleDto>> ConsultarDisponibilidadPorFechaPersonasAsync(
        DateTime fechaLlegada,
        DateTime fechaSalida,
        int numeroPersonas,
        int? idSede,
        CancellationToken cancellationToken = default)
    {
        var sql = @"
            EXEC sp_ConsultarAlojamientosDisponiblesPorFechaPersonas
                @p_FechaLlegada,
                @p_FechaSalida,
                @p_NumeroPersonas,
                @p_IdSede";

        var parametros = new[]
        {
            new SqlParameter("@p_FechaLlegada", fechaLlegada),
            new SqlParameter("@p_FechaSalida", fechaSalida),
            new SqlParameter("@p_NumeroPersonas", numeroPersonas),
            new SqlParameter("@p_IdSede", (object?)idSede ?? DBNull.Value)
        };

        return await _context.Database
            .SqlQueryRaw<AlojamientoDisponibleDto>(sql, parametros)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AlojamientoAdminDto>> ObtenerAlojamientosAdminAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Alojamientos
            .AsNoTracking()
            .Include(x => x.Sede)
            .Include(x => x.TipoAlojamiento)
            .Select(x => new AlojamientoAdminDto
            {
                IdAlojamiento = x.IdAlojamiento,
                IdSede = x.IdSede,
                IdTipoAlojamiento = x.IdTipoAlojamiento,
                NumeroAlojamiento = x.NumeroAlojamiento,
                NombreAlojamiento = x.NombreAlojamiento,
                Descripcion = x.Descripcion,
                NumeroHabitaciones = x.NumeroHabitaciones,
                CapacidadMaxima = x.CapacidadMaxima,
                NumeroHabitacionesTarifa = x.NumeroHabitacionesTarifa,
                Activo = x.Activo,
                NombreSede = x.Sede != null ? x.Sede.NombreSede : null,
                TipoAlojamiento = x.TipoAlojamiento != null ? x.TipoAlojamiento.Nombre : null
            })
            .OrderByDescending(x => x.Activo)
            .ThenBy(x => x.NombreSede)
            .ThenBy(x => x.NombreAlojamiento)
            .ToListAsync(cancellationToken);
    }

    public async Task<AlojamientoAdminDto?> ObtenerAlojamientoAdminPorIdAsync(
        int idAlojamiento,
        CancellationToken cancellationToken = default)
    {
        return await _context.Alojamientos
            .AsNoTracking()
            .Include(x => x.Sede)
            .Include(x => x.TipoAlojamiento)
            .Where(x => x.IdAlojamiento == idAlojamiento)
            .Select(x => new AlojamientoAdminDto
            {
                IdAlojamiento = x.IdAlojamiento,
                IdSede = x.IdSede,
                IdTipoAlojamiento = x.IdTipoAlojamiento,
                NumeroAlojamiento = x.NumeroAlojamiento,
                NombreAlojamiento = x.NombreAlojamiento,
                Descripcion = x.Descripcion,
                NumeroHabitaciones = x.NumeroHabitaciones,
                CapacidadMaxima = x.CapacidadMaxima,
                NumeroHabitacionesTarifa = x.NumeroHabitacionesTarifa,
                Activo = x.Activo,
                NombreSede = x.Sede != null ? x.Sede.NombreSede : null,
                TipoAlojamiento = x.TipoAlojamiento != null ? x.TipoAlojamiento.Nombre : null
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CrearAlojamientoAsync(
        AlojamientoFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        var alojamiento = new Alojamiento
        {
            IdSede = request.IdSede,
            IdTipoAlojamiento = request.IdTipoAlojamiento,
            NumeroAlojamiento = request.NumeroAlojamiento.Trim(),
            NombreAlojamiento = request.NombreAlojamiento.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim(),
            NumeroHabitaciones = request.NumeroHabitaciones,
            CapacidadMaxima = request.CapacidadMaxima,
            NumeroHabitacionesTarifa = request.NumeroHabitacionesTarifa,
            Activo = request.Activo
        };

        _context.Alojamientos.Add(alojamiento);

        await _context.SaveChangesAsync(cancellationToken);

        return alojamiento.IdAlojamiento;
    }

    public async Task<bool> ActualizarAlojamientoAsync(
        AlojamientoFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        var alojamiento = await _context.Alojamientos
            .FirstOrDefaultAsync(x => x.IdAlojamiento == request.IdAlojamiento, cancellationToken);

        if (alojamiento is null)
            return false;

        alojamiento.IdSede = request.IdSede;
        alojamiento.IdTipoAlojamiento = request.IdTipoAlojamiento;
        alojamiento.NumeroAlojamiento = request.NumeroAlojamiento.Trim();
        alojamiento.NombreAlojamiento = request.NombreAlojamiento.Trim();
        alojamiento.Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim();
        alojamiento.NumeroHabitaciones = request.NumeroHabitaciones;
        alojamiento.CapacidadMaxima = request.CapacidadMaxima;
        alojamiento.NumeroHabitacionesTarifa = request.NumeroHabitacionesTarifa;
        alojamiento.Activo = request.Activo;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> InactivarAlojamientoAsync(
        int idAlojamiento,
        CancellationToken cancellationToken = default)
    {
        var alojamiento = await _context.Alojamientos
            .FirstOrDefaultAsync(x => x.IdAlojamiento == idAlojamiento, cancellationToken);

        if (alojamiento is null)
            return false;

        alojamiento.Activo = false;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ActivarAlojamientoAsync(
        int idAlojamiento,
        CancellationToken cancellationToken = default)
    {
        var alojamiento = await _context.Alojamientos
            .FirstOrDefaultAsync(x => x.IdAlojamiento == idAlojamiento, cancellationToken);

        if (alojamiento is null)
            return false;

        alojamiento.Activo = true;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerSedesActivasSelectAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Sedes
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.NombreSede)
            .Select(x => new CatalogoSelectDto
            {
                Id = x.IdSede,
                Nombre = x.NombreSede
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerTiposAlojamientoSelectAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.TiposAlojamiento
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Nombre)
            .Select(x => new CatalogoSelectDto
            {
                Id = x.IdTipoAlojamiento,
                Nombre = x.Nombre
            })
            .ToListAsync(cancellationToken);
    }
}
