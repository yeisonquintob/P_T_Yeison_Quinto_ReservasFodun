using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.DTOs.Alojamientos;

namespace ReservasFodun.Application.Interfaces.Repositories;

public interface IAlojamientoRepository
{
    Task<IEnumerable<AlojamientoDto>> ObtenerAlojamientosPorSedeAsync(
        int idSede,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AlojamientoDisponibleDto>> ConsultarDisponibilidadPorFechaAsync(
        DateTime fechaLlegada,
        DateTime fechaSalida,
        int? idSede,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AlojamientoDisponibleDto>> ConsultarDisponibilidadPorFechaPersonasAsync(
        DateTime fechaLlegada,
        DateTime fechaSalida,
        int numeroPersonas,
        int? idSede,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AlojamientoAdminDto>> ObtenerAlojamientosAdminAsync(
        CancellationToken cancellationToken = default);

    Task<AlojamientoAdminDto?> ObtenerAlojamientoAdminPorIdAsync(
        int idAlojamiento,
        CancellationToken cancellationToken = default);

    Task<int> CrearAlojamientoAsync(
        AlojamientoFormularioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> ActualizarAlojamientoAsync(
        AlojamientoFormularioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> InactivarAlojamientoAsync(
        int idAlojamiento,
        CancellationToken cancellationToken = default);

    Task<bool> ActivarAlojamientoAsync(
        int idAlojamiento,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<CatalogoSelectDto>> ObtenerSedesActivasSelectAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<CatalogoSelectDto>> ObtenerTiposAlojamientoSelectAsync(
        CancellationToken cancellationToken = default);
}
