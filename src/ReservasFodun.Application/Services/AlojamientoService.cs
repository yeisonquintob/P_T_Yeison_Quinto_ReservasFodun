using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.DTOs.Alojamientos;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Application.Services;

public class AlojamientoService : IAlojamientoService
{
    private readonly IAlojamientoRepository _alojamientoRepository;

    public AlojamientoService(IAlojamientoRepository alojamientoRepository)
    {
        _alojamientoRepository = alojamientoRepository;
    }

    public async Task<IEnumerable<AlojamientoDto>> ObtenerAlojamientosPorSedeAsync(
        int idSede,
        CancellationToken cancellationToken = default)
    {
        if (idSede <= 0)
            throw new ArgumentException("El identificador de la sede no es válido.", nameof(idSede));

        return await _alojamientoRepository.ObtenerAlojamientosPorSedeAsync(idSede, cancellationToken);
    }

    public async Task<IEnumerable<AlojamientoAdminDto>> ObtenerAlojamientosAdminAsync(
        CancellationToken cancellationToken = default)
    {
        return await _alojamientoRepository.ObtenerAlojamientosAdminAsync(cancellationToken);
    }

    public async Task<AlojamientoAdminDto?> ObtenerAlojamientoAdminPorIdAsync(
        int idAlojamiento,
        CancellationToken cancellationToken = default)
    {
        ValidarIdAlojamiento(idAlojamiento);

        return await _alojamientoRepository.ObtenerAlojamientoAdminPorIdAsync(idAlojamiento, cancellationToken);
    }

    public async Task<int> CrearAlojamientoAsync(
        AlojamientoFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidarFormulario(request);

        return await _alojamientoRepository.CrearAlojamientoAsync(request, cancellationToken);
    }

    public async Task<bool> ActualizarAlojamientoAsync(
        AlojamientoFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.IdAlojamiento <= 0)
            throw new ArgumentException("El identificador del alojamiento no es válido.", nameof(request.IdAlojamiento));

        ValidarFormulario(request);

        return await _alojamientoRepository.ActualizarAlojamientoAsync(request, cancellationToken);
    }

    public async Task<bool> InactivarAlojamientoAsync(
        int idAlojamiento,
        CancellationToken cancellationToken = default)
    {
        ValidarIdAlojamiento(idAlojamiento);

        return await _alojamientoRepository.InactivarAlojamientoAsync(idAlojamiento, cancellationToken);
    }

    public async Task<bool> ActivarAlojamientoAsync(
        int idAlojamiento,
        CancellationToken cancellationToken = default)
    {
        ValidarIdAlojamiento(idAlojamiento);

        return await _alojamientoRepository.ActivarAlojamientoAsync(idAlojamiento, cancellationToken);
    }

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerSedesActivasSelectAsync(
        CancellationToken cancellationToken = default)
    {
        return await _alojamientoRepository.ObtenerSedesActivasSelectAsync(cancellationToken);
    }

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerTiposAlojamientoSelectAsync(
        CancellationToken cancellationToken = default)
    {
        return await _alojamientoRepository.ObtenerTiposAlojamientoSelectAsync(cancellationToken);
    }

    private static void ValidarIdAlojamiento(int idAlojamiento)
    {
        if (idAlojamiento <= 0)
            throw new ArgumentException("El identificador del alojamiento no es válido.", nameof(idAlojamiento));
    }

    private static void ValidarFormulario(AlojamientoFormularioRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La información del alojamiento no puede ser nula.");

        if (request.IdSede <= 0)
            throw new ArgumentException("La sede es obligatoria.", nameof(request.IdSede));

        if (request.IdTipoAlojamiento <= 0)
            throw new ArgumentException("El tipo de alojamiento es obligatorio.", nameof(request.IdTipoAlojamiento));

        if (string.IsNullOrWhiteSpace(request.NumeroAlojamiento))
            throw new ArgumentException("El número del alojamiento es obligatorio.", nameof(request.NumeroAlojamiento));

        if (string.IsNullOrWhiteSpace(request.NombreAlojamiento))
            throw new ArgumentException("El nombre del alojamiento es obligatorio.", nameof(request.NombreAlojamiento));

        if (request.NumeroHabitaciones <= 0)
            throw new ArgumentException("El número de habitaciones debe ser mayor que cero.", nameof(request.NumeroHabitaciones));

        if (request.CapacidadMaxima <= 0)
            throw new ArgumentException("La capacidad máxima debe ser mayor que cero.", nameof(request.CapacidadMaxima));

        if (request.NumeroHabitacionesTarifa.HasValue && request.NumeroHabitacionesTarifa.Value <= 0)
            throw new ArgumentException("El número de habitaciones para tarifa debe ser mayor que cero.", nameof(request.NumeroHabitacionesTarifa));
    }
}
