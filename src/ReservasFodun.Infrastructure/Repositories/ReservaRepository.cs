using Microsoft.EntityFrameworkCore;
using ReservasFodun.Application.DTOs.Reservas;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Domain.Entities;
using ReservasFodun.Infrastructure.Data;

namespace ReservasFodun.Infrastructure.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly ApplicationDbContext _context;

    public ReservaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CrearReservaAsync(
        CrearReservaRequest request,
        string idUsuario,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La solicitud de reserva no puede ser nula.");

        if (string.IsNullOrWhiteSpace(idUsuario))
            throw new ArgumentException("El identificador del usuario es obligatorio.", nameof(idUsuario));

        if (request.FechaSalida <= request.FechaLlegada)
            throw new ArgumentException("La fecha de salida debe ser mayor que la fecha de llegada.");

        var numeroNoches = (request.FechaSalida.Date - request.FechaLlegada.Date).Days;

        if (numeroNoches <= 0)
            throw new ArgumentException("El número de noches debe ser mayor que cero.");

        var reserva = new Reserva
        {
            CodigoReserva = GenerarCodigoReserva(),
            IdUsuario = idUsuario,
            IdSede = request.IdSede,
            IdEstadoReserva = 1,
            FechaLlegada = request.FechaLlegada,
            FechaSalida = request.FechaSalida,
            NumeroNoches = numeroNoches,
            NumeroPersonas = request.NumeroPersonas,
            NumeroHabitaciones = request.NumeroHabitaciones,
            ValorSubtotal = request.ValorSubtotal,
            ValorTotal = request.ValorTotal,
            Observaciones = request.Observaciones,
            FechaCreacion = DateTime.Now,
            FechaModificacion = DateTime.Now
        };

        _context.Set<Reserva>().Add(reserva);
        await _context.SaveChangesAsync(cancellationToken);

        return reserva.IdReserva;
    }

    public async Task<IEnumerable<MisReservasDto>> ConsultarReservasUsuarioAsync(
        string idUsuario,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idUsuario))
            throw new ArgumentException("El identificador del usuario es obligatorio.", nameof(idUsuario));

        return await _context.Set<Reserva>()
            .AsNoTracking()
            .Include(x => x.Sede)
            .Include(x => x.EstadoReserva)
            .Where(x => x.IdUsuario == idUsuario)
            .OrderByDescending(x => x.FechaCreacion)
            .Select(x => new MisReservasDto
            {
                IdReserva = x.IdReserva,
                NombreSede = x.Sede != null ? x.Sede.NombreSede : string.Empty,
                EstadoReserva = x.EstadoReserva != null ? x.EstadoReserva.Nombre : string.Empty,
                FechaLlegada = x.FechaLlegada,
                FechaSalida = x.FechaSalida,
                NumeroPersonas = x.NumeroPersonas,
                NumeroHabitaciones = x.NumeroHabitaciones,
                ValorTotal = x.ValorTotal,
                FechaCreacion = x.FechaCreacion,
                PermiteCancelar = x.EstadoReserva != null && x.EstadoReserva.BloqueaDisponibilidad
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ReservaDto?> ObtenerReservaPorIdAsync(
        int idReserva,
        string idUsuario,
        CancellationToken cancellationToken = default)
    {
        if (idReserva <= 0)
            throw new ArgumentException("El identificador de la reserva no es válido.", nameof(idReserva));

        if (string.IsNullOrWhiteSpace(idUsuario))
            throw new ArgumentException("El identificador del usuario es obligatorio.", nameof(idUsuario));

        return await _context.Set<Reserva>()
            .AsNoTracking()
            .Include(x => x.Sede)
            .Include(x => x.EstadoReserva)
            .Where(x => x.IdReserva == idReserva && x.IdUsuario == idUsuario)
            .Select(x => new ReservaDto
            {
                IdReserva = x.IdReserva,
                CodigoReserva = x.CodigoReserva,
                IdUsuario = x.IdUsuario,
                IdSede = x.IdSede,
                NombreSede = x.Sede != null ? x.Sede.NombreSede : string.Empty,
                IdEstadoReserva = x.IdEstadoReserva,
                EstadoReserva = x.EstadoReserva != null ? x.EstadoReserva.Nombre : string.Empty,
                FechaLlegada = x.FechaLlegada,
                FechaSalida = x.FechaSalida,
                NumeroNoches = x.NumeroNoches,
                NumeroPersonas = x.NumeroPersonas,
                NumeroHabitaciones = x.NumeroHabitaciones,
                ValorSubtotal = x.ValorSubtotal,
                ValorTotal = x.ValorTotal,
                Observaciones = x.Observaciones,
                FechaCreacion = x.FechaCreacion,
                FechaModificacion = x.FechaModificacion,
                PermiteCancelar = x.EstadoReserva != null && x.EstadoReserva.BloqueaDisponibilidad,
                Detalles = new List<ReservaDetalleDto>()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> CancelarReservaAsync(
        int idReserva,
        string idUsuario,
        string? motivoCancelacion,
        CancellationToken cancellationToken = default)
    {
        if (idReserva <= 0)
            throw new ArgumentException("El identificador de la reserva no es válido.", nameof(idReserva));

        if (string.IsNullOrWhiteSpace(idUsuario))
            throw new ArgumentException("El identificador del usuario es obligatorio.", nameof(idUsuario));

        var reserva = await _context.Set<Reserva>()
            .FirstOrDefaultAsync(
                x => x.IdReserva == idReserva && x.IdUsuario == idUsuario,
                cancellationToken);

        if (reserva is null)
            return false;

        reserva.IdEstadoReserva = 3;
        reserva.FechaModificacion = DateTime.Now;

        if (!string.IsNullOrWhiteSpace(motivoCancelacion))
        {
            reserva.Observaciones = string.IsNullOrWhiteSpace(reserva.Observaciones)
                ? $"Cancelación: {motivoCancelacion.Trim()}"
                : $"{reserva.Observaciones} | Cancelación: {motivoCancelacion.Trim()}";
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static string GenerarCodigoReserva()
    {
        return $"RES-{DateTime.Now:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";
    }
}
