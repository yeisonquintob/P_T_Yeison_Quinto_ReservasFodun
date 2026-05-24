using ReservasFodun.Application.DTOs.Alojamientos;

namespace ReservasFodun.Application.Interfaces.Services;

public interface IDisponibilidadService
{
    Task<IEnumerable<AlojamientoDisponibleDto>> ConsultarDisponibilidadAsync(
        ConsultarDisponibilidadRequest request,
        CancellationToken cancellationToken = default);
}
