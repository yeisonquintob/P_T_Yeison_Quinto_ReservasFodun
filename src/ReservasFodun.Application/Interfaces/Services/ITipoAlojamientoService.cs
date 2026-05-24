using ReservasFodun.Application.DTOs.Admin;

namespace ReservasFodun.Application.Interfaces.Services;

public interface ITipoAlojamientoService
{
    Task<IEnumerable<TipoAlojamientoAdminDto>> ObtenerTiposAlojamientoAdminAsync(
        CancellationToken cancellationToken = default);

    Task<TipoAlojamientoAdminDto?> ObtenerTipoAlojamientoAdminPorIdAsync(
        int idTipoAlojamiento,
        CancellationToken cancellationToken = default);

    Task<int> CrearTipoAlojamientoAsync(
        TipoAlojamientoFormularioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> ActualizarTipoAlojamientoAsync(
        TipoAlojamientoFormularioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> InactivarTipoAlojamientoAsync(
        int idTipoAlojamiento,
        CancellationToken cancellationToken = default);

    Task<bool> ActivarTipoAlojamientoAsync(
        int idTipoAlojamiento,
        CancellationToken cancellationToken = default);
}
