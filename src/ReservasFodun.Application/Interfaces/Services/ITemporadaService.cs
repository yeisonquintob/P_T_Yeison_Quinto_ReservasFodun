using ReservasFodun.Application.DTOs.Admin;

namespace ReservasFodun.Application.Interfaces.Services;

public interface ITemporadaService
{
    Task<IEnumerable<TemporadaAdminDto>> ObtenerTemporadasAdminAsync(
        CancellationToken cancellationToken = default);

    Task<TemporadaAdminDto?> ObtenerTemporadaAdminPorIdAsync(
        int idTemporada,
        CancellationToken cancellationToken = default);

    Task<int> CrearTemporadaAsync(
        TemporadaFormularioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> ActualizarTemporadaAsync(
        TemporadaFormularioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> InactivarTemporadaAsync(
        int idTemporada,
        CancellationToken cancellationToken = default);

    Task<bool> ActivarTemporadaAsync(
        int idTemporada,
        CancellationToken cancellationToken = default);
}
