using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.DTOs.Tarifas;

namespace ReservasFodun.Application.Interfaces.Repositories;

public interface ITarifaRepository
{
    Task<IEnumerable<TarifaDto>> ConsultarTarifasAsync(
        ConsultarTarifaRequest request,
        CancellationToken cancellationToken = default);

    Task<CalcularValorReservaResponse?> CalcularValorReservaAsync(
        CalcularValorReservaRequest request,
        CancellationToken cancellationToken = default);

    Task<CalculoTarifaResultadoDto?> CalcularTarifaAsync(
        CalculoTarifaRequestDto request,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TarifaAdminDto>> ObtenerTarifasAdminAsync(
        CancellationToken cancellationToken = default);

    Task<TarifaAdminDto?> ObtenerTarifaAdminPorIdAsync(
        int idTarifa,
        CancellationToken cancellationToken = default);

    Task<int> CrearTarifaAsync(
        TarifaFormularioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> ActualizarTarifaAsync(
        TarifaFormularioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> InactivarTarifaAsync(
        int idTarifa,
        CancellationToken cancellationToken = default);

    Task<bool> ActivarTarifaAsync(
        int idTarifa,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<CatalogoSelectDto>> ObtenerSedesSelectAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<CatalogoSelectDto>> ObtenerAlojamientosSelectAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<CatalogoSelectDto>> ObtenerTemporadasSelectAsync(
        CancellationToken cancellationToken = default);
}
