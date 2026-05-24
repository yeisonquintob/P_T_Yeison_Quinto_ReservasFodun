using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Application.Services;

public class ServicioAdicionalService : IServicioAdicionalService
{
    private readonly IServicioAdicionalRepository _servicioAdicionalRepository;

    public ServicioAdicionalService(IServicioAdicionalRepository servicioAdicionalRepository)
    {
        _servicioAdicionalRepository = servicioAdicionalRepository;
    }

    public async Task<IEnumerable<ServicioAdicionalAdminDto>> ObtenerServiciosAdicionalesAdminAsync(
        CancellationToken cancellationToken = default)
    {
        return await _servicioAdicionalRepository.ObtenerServiciosAdicionalesAdminAsync(cancellationToken);
    }

    public async Task<ServicioAdicionalAdminDto?> ObtenerServicioAdicionalAdminPorIdAsync(
        int idServicioAdicional,
        CancellationToken cancellationToken = default)
    {
        ValidarId(idServicioAdicional);

        return await _servicioAdicionalRepository.ObtenerServicioAdicionalAdminPorIdAsync(
            idServicioAdicional,
            cancellationToken);
    }

    public async Task<int> CrearServicioAdicionalAsync(
        ServicioAdicionalFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidarFormulario(request);

        return await _servicioAdicionalRepository.CrearServicioAdicionalAsync(request, cancellationToken);
    }

    public async Task<bool> ActualizarServicioAdicionalAsync(
        ServicioAdicionalFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.IdServicioAdicional <= 0)
            throw new ArgumentException("El identificador del servicio adicional no es válido.", nameof(request.IdServicioAdicional));

        ValidarFormulario(request);

        return await _servicioAdicionalRepository.ActualizarServicioAdicionalAsync(request, cancellationToken);
    }

    public async Task<bool> InactivarServicioAdicionalAsync(
        int idServicioAdicional,
        CancellationToken cancellationToken = default)
    {
        ValidarId(idServicioAdicional);

        return await _servicioAdicionalRepository.InactivarServicioAdicionalAsync(idServicioAdicional, cancellationToken);
    }

    public async Task<bool> ActivarServicioAdicionalAsync(
        int idServicioAdicional,
        CancellationToken cancellationToken = default)
    {
        ValidarId(idServicioAdicional);

        return await _servicioAdicionalRepository.ActivarServicioAdicionalAsync(idServicioAdicional, cancellationToken);
    }

    private static void ValidarId(int idServicioAdicional)
    {
        if (idServicioAdicional <= 0)
            throw new ArgumentException("El identificador del servicio adicional no es válido.", nameof(idServicioAdicional));
    }

    private static void ValidarFormulario(ServicioAdicionalFormularioRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La información del servicio adicional no puede ser nula.");

        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new ArgumentException("El nombre del servicio adicional es obligatorio.", nameof(request.Nombre));

        if (request.Valor < 0)
            throw new ArgumentException("El valor del servicio adicional no puede ser negativo.", nameof(request.Valor));
    }
}
