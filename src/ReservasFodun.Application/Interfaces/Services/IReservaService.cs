using ReservasFodun.Application.DTOs.Reservas;

namespace ReservasFodun.Application.Interfaces.Services;

public interface IReservaService
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
        CancelarReservaRequest request,
        string idUsuario,
        CancellationToken cancellationToken = default);
}
