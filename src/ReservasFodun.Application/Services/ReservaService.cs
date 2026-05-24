using ReservasFodun.Application.DTOs.Reservas;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Application.Services;

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _reservaRepository;

    public ReservaService(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
    }

    public async Task<int> CrearReservaAsync(
        CrearReservaRequest request,
        string idUsuario,
        CancellationToken cancellationToken = default)
    {
        ValidarUsuario(idUsuario);
        ValidarCrearReserva(request);

        return await _reservaRepository.CrearReservaAsync(request, idUsuario, cancellationToken);
    }

    public async Task<ReservaDto?> ObtenerReservaPorIdAsync(
        int idReserva,
        string idUsuario,
        CancellationToken cancellationToken = default)
    {
        ValidarUsuario(idUsuario);

        if (idReserva <= 0)
            throw new ArgumentException("El identificador de la reserva no es válido.", nameof(idReserva));

        return await _reservaRepository.ObtenerReservaPorIdAsync(idReserva, idUsuario, cancellationToken);
    }

    public async Task<IEnumerable<MisReservasDto>> ConsultarReservasUsuarioAsync(
        string idUsuario,
        CancellationToken cancellationToken = default)
    {
        ValidarUsuario(idUsuario);

        return await _reservaRepository.ConsultarReservasUsuarioAsync(idUsuario, cancellationToken);
    }

    public async Task<bool> CancelarReservaAsync(
        CancelarReservaRequest request,
        string idUsuario,
        CancellationToken cancellationToken = default)
    {
        ValidarUsuario(idUsuario);

        if (request is null)
            throw new ArgumentNullException(nameof(request), "La solicitud de cancelación no puede ser nula.");

        if (request.IdReserva <= 0)
            throw new ArgumentException("El identificador de la reserva no es válido.", nameof(request.IdReserva));

        return await _reservaRepository.CancelarReservaAsync(
            request.IdReserva,
            idUsuario,
            request.MotivoCancelacion,
            cancellationToken);
    }

    private static void ValidarCrearReserva(CrearReservaRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La solicitud de reserva no puede ser nula.");

        if (request.IdSede <= 0)
            throw new ArgumentException("La sede es obligatoria.", nameof(request.IdSede));

        if (request.IdAlojamiento <= 0)
            throw new ArgumentException("El alojamiento es obligatorio.", nameof(request.IdAlojamiento));

        if (request.FechaLlegada == default)
            throw new ArgumentException("La fecha de llegada es obligatoria.", nameof(request.FechaLlegada));

        if (request.FechaSalida == default)
            throw new ArgumentException("La fecha de salida es obligatoria.", nameof(request.FechaSalida));

        if (request.FechaSalida <= request.FechaLlegada)
            throw new ArgumentException("La fecha de salida debe ser mayor que la fecha de llegada.");

        if (request.NumeroPersonas <= 0)
            throw new ArgumentException("El número de personas debe ser mayor que cero.", nameof(request.NumeroPersonas));

        if (request.NumeroHabitaciones <= 0)
            throw new ArgumentException("El número de habitaciones debe ser mayor que cero.", nameof(request.NumeroHabitaciones));

        if (request.ValorTotal < 0)
            throw new ArgumentException("El valor total de la reserva no puede ser negativo.", nameof(request.ValorTotal));
    }

    private static void ValidarUsuario(string idUsuario)
    {
        if (string.IsNullOrWhiteSpace(idUsuario))
            throw new UnauthorizedAccessException("No se encontró un usuario autenticado para realizar esta operación.");
    }
}
