using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.DTOs.Alojamientos;

namespace ReservasFodun.Application.Interfaces.Services;

public interface ISedeService
{
    Task<IEnumerable<SedeDto>> ObtenerSedesAsync(
        CancellationToken cancellationToken = default);

    Task<SedeDto?> ObtenerSedePorIdAsync(
        int idSede,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<SedeAdminDto>> ObtenerSedesAdminAsync(
        CancellationToken cancellationToken = default);

    Task<SedeAdminDto?> ObtenerSedeAdminPorIdAsync(
        int idSede,
        CancellationToken cancellationToken = default);

    Task<int> CrearSedeAsync(
        SedeFormularioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> ActualizarSedeAsync(
        SedeFormularioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> InactivarSedeAsync(
        int idSede,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<CatalogoSelectDto>> ObtenerTiposSedeAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<CatalogoSelectDto>> ObtenerMunicipiosAsync(
        CancellationToken cancellationToken = default);
}
