using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Application.Services;

public class TipoAlojamientoService : ITipoAlojamientoService
{
    private readonly ITipoAlojamientoRepository _tipoAlojamientoRepository;

    public TipoAlojamientoService(ITipoAlojamientoRepository tipoAlojamientoRepository)
    {
        _tipoAlojamientoRepository = tipoAlojamientoRepository;
    }

    public async Task<IEnumerable<TipoAlojamientoAdminDto>> ObtenerTiposAlojamientoAdminAsync(
        CancellationToken cancellationToken = default)
    {
        return await _tipoAlojamientoRepository.ObtenerTiposAlojamientoAdminAsync(cancellationToken);
    }

    public async Task<TipoAlojamientoAdminDto?> ObtenerTipoAlojamientoAdminPorIdAsync(
        int idTipoAlojamiento,
        CancellationToken cancellationToken = default)
    {
        ValidarId(idTipoAlojamiento);

        return await _tipoAlojamientoRepository.ObtenerTipoAlojamientoAdminPorIdAsync(
            idTipoAlojamiento,
            cancellationToken);
    }

    public async Task<int> CrearTipoAlojamientoAsync(
        TipoAlojamientoFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidarFormulario(request);

        return await _tipoAlojamientoRepository.CrearTipoAlojamientoAsync(request, cancellationToken);
    }

    public async Task<bool> ActualizarTipoAlojamientoAsync(
        TipoAlojamientoFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.IdTipoAlojamiento <= 0)
            throw new ArgumentException("El identificador del tipo de alojamiento no es válido.", nameof(request.IdTipoAlojamiento));

        ValidarFormulario(request);

        return await _tipoAlojamientoRepository.ActualizarTipoAlojamientoAsync(request, cancellationToken);
    }

    public async Task<bool> InactivarTipoAlojamientoAsync(
        int idTipoAlojamiento,
        CancellationToken cancellationToken = default)
    {
        ValidarId(idTipoAlojamiento);

        return await _tipoAlojamientoRepository.InactivarTipoAlojamientoAsync(idTipoAlojamiento, cancellationToken);
    }

    public async Task<bool> ActivarTipoAlojamientoAsync(
        int idTipoAlojamiento,
        CancellationToken cancellationToken = default)
    {
        ValidarId(idTipoAlojamiento);

        return await _tipoAlojamientoRepository.ActivarTipoAlojamientoAsync(idTipoAlojamiento, cancellationToken);
    }

    private static void ValidarId(int idTipoAlojamiento)
    {
        if (idTipoAlojamiento <= 0)
            throw new ArgumentException("El identificador del tipo de alojamiento no es válido.", nameof(idTipoAlojamiento));
    }

    private static void ValidarFormulario(TipoAlojamientoFormularioRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La información del tipo de alojamiento no puede ser nula.");

        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new ArgumentException("El nombre del tipo de alojamiento es obligatorio.", nameof(request.Nombre));
    }
}
