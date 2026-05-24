using ReservasFodun.Application.DTOs.Reservas;

namespace ReservasFodun.Application.Interfaces.Repositories;

public interface IReservaRepository
{
    Task<int> CrearReservaAsync(
        CrearReservaRequest request,
        string idUsuario,
        CancellationToken cancellationToken = default);

    Task<ReservaDto?> ObtenerReservaPorIdAsync(
        int idReserva,
        string idUsuario,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<MisReservasDto>> ConsultarReservasUsuarioAsync(
        string idUsuario,
        CancellationToken cancellationToken = default);

    Task<bool> CancelarReservaAsync(
        int idReserva,
        string idUsuario,
        string? motivoCancelacion,
        CancellationToken cancellationToken = default);
}
