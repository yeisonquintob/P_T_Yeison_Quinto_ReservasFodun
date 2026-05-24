using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.DTOs.Tarifas;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Domain.Entities;
using ReservasFodun.Infrastructure.Data;

namespace ReservasFodun.Infrastructure.Repositories;

public class TarifaRepository : ITarifaRepository
{
    private readonly ApplicationDbContext _context;

    public TarifaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TarifaDto>> ConsultarTarifasAsync(
        ConsultarTarifaRequest request,
        CancellationToken cancellationToken = default)
    {
        return await _context.Tarifas
            .AsNoTracking()
            .Include(x => x.Sede)
            .Include(x => x.Alojamiento)
            .Include(x => x.Temporada)
            .Where(x => x.Activo)
            .Where(x => x.IdTemporada == request.IdTemporada)
            .Where(x => x.IdSede == request.IdSede || x.IdSede == null)
            .Where(x => !request.IdAlojamiento.HasValue ||
                        x.IdAlojamiento == request.IdAlojamiento ||
                        x.IdAlojamiento == null)
            .Where(x => !request.NumeroHabitaciones.HasValue ||
                        x.NumeroHabitacionesTarifa == request.NumeroHabitaciones ||
                        x.NumeroHabitacionesTarifa == null)
            .OrderByDescending(x => x.IdAlojamiento.HasValue)
            .ThenByDescending(x => x.NumeroHabitacionesTarifa.HasValue)
            .ThenBy(x => x.TarifaBase)
            .Select(x => new TarifaDto
            {
                IdTarifa = x.IdTarifa,
                IdSede = x.IdSede,
                NombreSede = x.Sede != null ? x.Sede.NombreSede : "Todas las sedes recreativas",
                IdAlojamiento = x.IdAlojamiento,
                NombreAlojamiento = x.Alojamiento != null ? x.Alojamiento.NombreAlojamiento : null,
                IdTemporada = x.IdTemporada,
                NombreTemporada = x.Temporada != null ? x.Temporada.Nombre : string.Empty,
                NumeroHabitacionesTarifa = x.NumeroHabitacionesTarifa,
                PersonasIncluidas = x.PersonasIncluidas,
                TarifaBase = x.TarifaBase,
                ValorPersonaAdicional = x.ValorPersonaAdicional,
                TipoTarifa = x.EsTarifaEspecial ? "Especial" : "Ordinaria"
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CalcularValorReservaResponse?> CalcularValorReservaAsync(
        CalcularValorReservaRequest request,
        CancellationToken cancellationToken = default)
    {
        var numeroNoches = (request.FechaSalida.Date - request.FechaLlegada.Date).Days;

        if (numeroNoches <= 0)
            return null;

        var tarifa = await _context.Tarifas
            .AsNoTracking()
            .Include(x => x.Sede)
            .Include(x => x.Alojamiento)
            .Include(x => x.Temporada)
            .Where(x => x.Activo)
            .Where(x => x.IdSede == request.IdSede || x.IdSede == null)
            .Where(x => !request.IdAlojamiento.HasValue ||
                        x.IdAlojamiento == request.IdAlojamiento ||
                        x.IdAlojamiento == null)
            .Where(x => x.NumeroHabitacionesTarifa == request.NumeroHabitaciones ||
                        x.NumeroHabitacionesTarifa == null)
            .OrderByDescending(x => x.IdAlojamiento.HasValue)
            .ThenByDescending(x => x.NumeroHabitacionesTarifa.HasValue)
            .ThenByDescending(x => x.EsTarifaEspecial)
            .FirstOrDefaultAsync(cancellationToken);

        if (tarifa is null)
            return null;

        var personasAdicionales = Math.Max(0, request.NumeroPersonas - tarifa.PersonasIncluidas);
        var valorTarifa = tarifa.TarifaBase * numeroNoches;
        var valorPersonasAdicionales = personasAdicionales * tarifa.ValorPersonaAdicional * numeroNoches;

        decimal valorLavanderia = 0;

        if (request.IncluirLavanderia)
        {
            var lavanderia = await _context.ServiciosAdicionales
                .AsNoTracking()
                .Where(x => x.Activo && x.Nombre.Contains("Lavander"))
                .OrderBy(x => x.Nombre)
                .FirstOrDefaultAsync(cancellationToken);

            if (lavanderia is not null)
            {
                valorLavanderia = lavanderia.Valor;

                if (lavanderia.PorPersona)
                    valorLavanderia *= request.NumeroPersonas;

                if (lavanderia.PorNoche)
                    valorLavanderia *= numeroNoches;
            }
        }

        var valorTarifaOrdinaria = tarifa.EsTarifaEspecial ? 0 : valorTarifa;
        var valorTarifaEspecial = tarifa.EsTarifaEspecial ? valorTarifa : 0;
        var valorSubtotal = valorTarifa + valorPersonasAdicionales;
        var valorTotal = valorSubtotal + valorLavanderia;

        return new CalcularValorReservaResponse
        {
            IdSede = request.IdSede,
            NombreSede = tarifa.Sede != null ? tarifa.Sede.NombreSede : "Sede seleccionada",
            IdAlojamiento = request.IdAlojamiento,
            NombreAlojamiento = tarifa.Alojamiento != null ? tarifa.Alojamiento.NombreAlojamiento : null,
            FechaLlegada = request.FechaLlegada,
            FechaSalida = request.FechaSalida,
            NumeroNoches = numeroNoches,
            NumeroPersonas = request.NumeroPersonas,
            NumeroHabitaciones = request.NumeroHabitaciones,
            DiasOrdinarios = tarifa.EsTarifaEspecial ? 0 : numeroNoches,
            DiasEspeciales = tarifa.EsTarifaEspecial ? numeroNoches : 0,
            ValorTarifaOrdinaria = valorTarifaOrdinaria,
            ValorTarifaEspecial = valorTarifaEspecial,
            ValorPersonasAdicionales = valorPersonasAdicionales,
            ValorLavanderia = valorLavanderia,
            ValorSubtotal = valorSubtotal,
            ValorTotal = valorTotal,
            DetalleCalculo = $"Noches: {numeroNoches}. Personas adicionales: {personasAdicionales}."
        };
    }

    public async Task<CalculoTarifaResultadoDto?> CalcularTarifaAsync(
        CalculoTarifaRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var sql = @"
            EXEC sp_CalcularTarifaReserva
                @p_IdSede,
                @p_IdAlojamiento,
                @p_FechaLlegada,
                @p_FechaSalida,
                @p_NumeroPersonas,
                @p_NumeroHabitaciones";

        var parametros = new[]
        {
            new SqlParameter("@p_IdSede", request.IdSede),
            new SqlParameter("@p_IdAlojamiento", request.IdAlojamiento),
            new SqlParameter("@p_FechaLlegada", request.FechaLlegada),
            new SqlParameter("@p_FechaSalida", request.FechaSalida),
            new SqlParameter("@p_NumeroPersonas", request.NumeroPersonas),
            new SqlParameter("@p_NumeroHabitaciones", request.NumeroHabitaciones)
        };

        return await _context.Database
            .SqlQueryRaw<CalculoTarifaResultadoDto>(sql, parametros)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TarifaAdminDto>> ObtenerTarifasAdminAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Tarifas
            .AsNoTracking()
            .Include(x => x.Sede)
            .Include(x => x.Alojamiento)
            .Include(x => x.Temporada)
            .Select(x => new TarifaAdminDto
            {
                IdTarifa = x.IdTarifa,
                IdSede = x.IdSede,
                IdAlojamiento = x.IdAlojamiento,
                IdTemporada = x.IdTemporada,
                NumeroHabitacionesTarifa = x.NumeroHabitacionesTarifa,
                PersonasIncluidas = x.PersonasIncluidas,
                TarifaBase = x.TarifaBase,
                ValorPersonaAdicional = x.ValorPersonaAdicional,
                EsTarifaEspecial = x.EsTarifaEspecial,
                Descripcion = x.Descripcion,
                Activo = x.Activo,
                NombreSede = x.Sede != null ? x.Sede.NombreSede : "Todas las sedes recreativas",
                NombreAlojamiento = x.Alojamiento != null ? x.Alojamiento.NombreAlojamiento : null,
                NombreTemporada = x.Temporada != null ? x.Temporada.Nombre : null
            })
            .OrderByDescending(x => x.Activo)
            .ThenBy(x => x.NombreSede)
            .ThenBy(x => x.NombreAlojamiento)
            .ThenBy(x => x.NombreTemporada)
            .ToListAsync(cancellationToken);
    }

    public async Task<TarifaAdminDto?> ObtenerTarifaAdminPorIdAsync(
        int idTarifa,
        CancellationToken cancellationToken = default)
    {
        return await _context.Tarifas
            .AsNoTracking()
            .Include(x => x.Sede)
            .Include(x => x.Alojamiento)
            .Include(x => x.Temporada)
            .Where(x => x.IdTarifa == idTarifa)
            .Select(x => new TarifaAdminDto
            {
                IdTarifa = x.IdTarifa,
                IdSede = x.IdSede,
                IdAlojamiento = x.IdAlojamiento,
                IdTemporada = x.IdTemporada,
                NumeroHabitacionesTarifa = x.NumeroHabitacionesTarifa,
                PersonasIncluidas = x.PersonasIncluidas,
                TarifaBase = x.TarifaBase,
                ValorPersonaAdicional = x.ValorPersonaAdicional,
                EsTarifaEspecial = x.EsTarifaEspecial,
                Descripcion = x.Descripcion,
                Activo = x.Activo,
                NombreSede = x.Sede != null ? x.Sede.NombreSede : "Todas las sedes recreativas",
                NombreAlojamiento = x.Alojamiento != null ? x.Alojamiento.NombreAlojamiento : null,
                NombreTemporada = x.Temporada != null ? x.Temporada.Nombre : null
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CrearTarifaAsync(
        TarifaFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        var tarifa = new Tarifa
        {
            IdSede = request.IdSede,
            IdAlojamiento = request.IdAlojamiento,
            IdTemporada = request.IdTemporada,
            NumeroHabitacionesTarifa = request.NumeroHabitacionesTarifa,
            PersonasIncluidas = request.PersonasIncluidas,
            TarifaBase = request.TarifaBase,
            ValorPersonaAdicional = request.ValorPersonaAdicional,
            EsTarifaEspecial = request.EsTarifaEspecial,
            Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim(),
            Activo = request.Activo
        };

        _context.Tarifas.Add(tarifa);

        await _context.SaveChangesAsync(cancellationToken);

        return tarifa.IdTarifa;
    }

    public async Task<bool> ActualizarTarifaAsync(
        TarifaFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        var tarifa = await _context.Tarifas
            .FirstOrDefaultAsync(x => x.IdTarifa == request.IdTarifa, cancellationToken);

        if (tarifa is null)
            return false;

        tarifa.IdSede = request.IdSede;
        tarifa.IdAlojamiento = request.IdAlojamiento;
        tarifa.IdTemporada = request.IdTemporada;
        tarifa.NumeroHabitacionesTarifa = request.NumeroHabitacionesTarifa;
        tarifa.PersonasIncluidas = request.PersonasIncluidas;
        tarifa.TarifaBase = request.TarifaBase;
        tarifa.ValorPersonaAdicional = request.ValorPersonaAdicional;
        tarifa.EsTarifaEspecial = request.EsTarifaEspecial;
        tarifa.Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim();
        tarifa.Activo = request.Activo;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> InactivarTarifaAsync(
        int idTarifa,
        CancellationToken cancellationToken = default)
    {
        var tarifa = await _context.Tarifas
            .FirstOrDefaultAsync(x => x.IdTarifa == idTarifa, cancellationToken);

        if (tarifa is null)
            return false;

        tarifa.Activo = false;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ActivarTarifaAsync(
        int idTarifa,
        CancellationToken cancellationToken = default)
    {
        var tarifa = await _context.Tarifas
            .FirstOrDefaultAsync(x => x.IdTarifa == idTarifa, cancellationToken);

        if (tarifa is null)
            return false;

        tarifa.Activo = true;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerSedesSelectAsync(
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

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerAlojamientosSelectAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Alojamientos
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.NombreAlojamiento)
            .Select(x => new CatalogoSelectDto
            {
                Id = x.IdAlojamiento,
                Nombre = x.NombreAlojamiento
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerTemporadasSelectAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Temporadas
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Nombre)
            .Select(x => new CatalogoSelectDto
            {
                Id = x.IdTemporada,
                Nombre = x.Nombre
            })
            .ToListAsync(cancellationToken);
    }
}
