using ReservasFodun.Application.DTOs.Alojamientos;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Application.Services;

public class DisponibilidadService : IDisponibilidadService
{
    private readonly IAlojamientoRepository _alojamientoRepository;

    public DisponibilidadService(IAlojamientoRepository alojamientoRepository)
    {
        _alojamientoRepository = alojamientoRepository;
    }

    public async Task<IEnumerable<AlojamientoDisponibleDto>> ConsultarDisponibilidadAsync(
        ConsultarDisponibilidadRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidarSolicitud(request);

        if (request.NumeroPersonas.HasValue && request.NumeroPersonas.Value > 0)
        {
            return await _alojamientoRepository.ConsultarDisponibilidadPorFechaPersonasAsync(
                request.FechaLlegada,
                request.FechaSalida,
                request.NumeroPersonas.Value,
                request.IdSede,
                cancellationToken);
        }

        return await _alojamientoRepository.ConsultarDisponibilidadPorFechaAsync(
            request.FechaLlegada,
            request.FechaSalida,
            request.IdSede,
            cancellationToken);
    }

    private static void ValidarSolicitud(ConsultarDisponibilidadRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La solicitud de disponibilidad no puede ser nula.");

        if (request.FechaLlegada == default)
            throw new ArgumentException("La fecha de llegada es obligatoria.", nameof(request.FechaLlegada));

        if (request.FechaSalida == default)
            throw new ArgumentException("La fecha de salida es obligatoria.", nameof(request.FechaSalida));

        if (request.FechaSalida <= request.FechaLlegada)
            throw new ArgumentException("La fecha de salida debe ser mayor que la fecha de llegada.");

        if (request.FechaLlegada.Date < DateTime.Today)
            throw new ArgumentException("La fecha de llegada no puede ser anterior a la fecha actual.");

        if (request.NumeroPersonas.HasValue && request.NumeroPersonas.Value <= 0)
            throw new ArgumentException("El número de personas debe ser mayor que cero.", nameof(request.NumeroPersonas));

        if (request.IdSede.HasValue && request.IdSede.Value <= 0)
            throw new ArgumentException("La sede seleccionada no es válida.", nameof(request.IdSede));
    }
}
