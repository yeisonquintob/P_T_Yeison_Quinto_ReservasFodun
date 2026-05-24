using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.DTOs.Tarifas;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Application.Services;

public class TarifaService : ITarifaService
{
    private readonly ITarifaRepository _tarifaRepository;

    public TarifaService(ITarifaRepository tarifaRepository)
    {
        _tarifaRepository = tarifaRepository;
    }

    public async Task<IEnumerable<TarifaDto>> ConsultarTarifasAsync(
        ConsultarTarifaRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La información de consulta no puede ser nula.");

        if (request.IdSede <= 0)
            throw new ArgumentException("La sede es obligatoria.", nameof(request.IdSede));

        if (request.IdTemporada <= 0)
            throw new ArgumentException("La temporada es obligatoria.", nameof(request.IdTemporada));

        return await _tarifaRepository.ConsultarTarifasAsync(request, cancellationToken);
    }

    public async Task<CalcularValorReservaResponse?> CalcularValorReservaAsync(
        CalcularValorReservaRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La información de cálculo no puede ser nula.");

        if (request.IdSede <= 0)
            throw new ArgumentException("La sede es obligatoria.", nameof(request.IdSede));

        if (request.FechaSalida.Date <= request.FechaLlegada.Date)
            throw new ArgumentException("La fecha de salida debe ser mayor que la fecha de llegada.", nameof(request.FechaSalida));

        if (request.NumeroPersonas <= 0)
            throw new ArgumentException("El número de personas debe ser mayor que cero.", nameof(request.NumeroPersonas));

        if (request.NumeroHabitaciones <= 0)
            throw new ArgumentException("El número de habitaciones debe ser mayor que cero.", nameof(request.NumeroHabitaciones));

        return await _tarifaRepository.CalcularValorReservaAsync(request, cancellationToken);
    }

    public async Task<CalculoTarifaResultadoDto?> CalcularTarifaAsync(
        CalculoTarifaRequestDto request,
        CancellationToken cancellationToken = default)
    {
        return await _tarifaRepository.CalcularTarifaAsync(request, cancellationToken);
    }

    public async Task<IEnumerable<TarifaAdminDto>> ObtenerTarifasAdminAsync(
        CancellationToken cancellationToken = default)
    {
        return await _tarifaRepository.ObtenerTarifasAdminAsync(cancellationToken);
    }

    public async Task<TarifaAdminDto?> ObtenerTarifaAdminPorIdAsync(
        int idTarifa,
        CancellationToken cancellationToken = default)
    {
        ValidarId(idTarifa);

        return await _tarifaRepository.ObtenerTarifaAdminPorIdAsync(idTarifa, cancellationToken);
    }

    public async Task<int> CrearTarifaAsync(
        TarifaFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidarFormulario(request);

        return await _tarifaRepository.CrearTarifaAsync(request, cancellationToken);
    }

    public async Task<bool> ActualizarTarifaAsync(
        TarifaFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.IdTarifa <= 0)
            throw new ArgumentException("El identificador de la tarifa no es válido.", nameof(request.IdTarifa));

        ValidarFormulario(request);

        return await _tarifaRepository.ActualizarTarifaAsync(request, cancellationToken);
    }

    public async Task<bool> InactivarTarifaAsync(
        int idTarifa,
        CancellationToken cancellationToken = default)
    {
        ValidarId(idTarifa);

        return await _tarifaRepository.InactivarTarifaAsync(idTarifa, cancellationToken);
    }

    public async Task<bool> ActivarTarifaAsync(
        int idTarifa,
        CancellationToken cancellationToken = default)
    {
        ValidarId(idTarifa);

        return await _tarifaRepository.ActivarTarifaAsync(idTarifa, cancellationToken);
    }

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerSedesSelectAsync(
        CancellationToken cancellationToken = default)
    {
        return await _tarifaRepository.ObtenerSedesSelectAsync(cancellationToken);
    }

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerAlojamientosSelectAsync(
        CancellationToken cancellationToken = default)
    {
        return await _tarifaRepository.ObtenerAlojamientosSelectAsync(cancellationToken);
    }

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerTemporadasSelectAsync(
        CancellationToken cancellationToken = default)
    {
        return await _tarifaRepository.ObtenerTemporadasSelectAsync(cancellationToken);
    }

    private static void ValidarId(int idTarifa)
    {
        if (idTarifa <= 0)
            throw new ArgumentException("El identificador de la tarifa no es válido.", nameof(idTarifa));
    }

    private static void ValidarFormulario(TarifaFormularioRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La información de la tarifa no puede ser nula.");

        if (request.IdTemporada <= 0)
            throw new ArgumentException("La temporada es obligatoria.", nameof(request.IdTemporada));

        if (request.PersonasIncluidas <= 0)
            throw new ArgumentException("Las personas incluidas deben ser mayor que cero.", nameof(request.PersonasIncluidas));

        if (request.TarifaBase < 0)
            throw new ArgumentException("La tarifa base no puede ser negativa.", nameof(request.TarifaBase));

        if (request.ValorPersonaAdicional < 0)
            throw new ArgumentException("El valor por persona adicional no puede ser negativo.", nameof(request.ValorPersonaAdicional));

        if (request.NumeroHabitacionesTarifa.HasValue &&
            request.NumeroHabitacionesTarifa.Value is not (1 or 2))
            throw new ArgumentException("El número de habitaciones tarifa debe ser 1, 2 o vacío.", nameof(request.NumeroHabitacionesTarifa));
    }
}
