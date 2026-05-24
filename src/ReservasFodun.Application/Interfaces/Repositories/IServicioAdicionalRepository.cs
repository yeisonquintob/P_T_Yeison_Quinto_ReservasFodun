using ReservasFodun.Application.DTOs.Admin;

namespace ReservasFodun.Application.Interfaces.Repositories;

public interface IServicioAdicionalRepository
{
    Task<IEnumerable<ServicioAdicionalAdminDto>> ObtenerServiciosAdicionalesAdminAsync(
        CancellationToken cancellationToken = default);

    Task<ServicioAdicionalAdminDto?> ObtenerServicioAdicionalAdminPorIdAsync(
        int idServicioAdicional,
        CancellationToken cancellationToken = default);

    Task<int> CrearServicioAdicionalAsync(
        ServicioAdicionalFormularioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> ActualizarServicioAdicionalAsync(
        ServicioAdicionalFormularioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> InactivarServicioAdicionalAsync(
        int idServicioAdicional,
        CancellationToken cancellationToken = default);

    Task<bool> ActivarServicioAdicionalAsync(
        int idServicioAdicional,
        CancellationToken cancellationToken = default);
}
