using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Application.Services;

public class TemporadaService : ITemporadaService
{
    private readonly ITemporadaRepository _temporadaRepository;

    public TemporadaService(ITemporadaRepository temporadaRepository)
    {
        _temporadaRepository = temporadaRepository;
    }

    public async Task<IEnumerable<TemporadaAdminDto>> ObtenerTemporadasAdminAsync(
        CancellationToken cancellationToken = default)
    {
        return await _temporadaRepository.ObtenerTemporadasAdminAsync(cancellationToken);
    }

    public async Task<TemporadaAdminDto?> ObtenerTemporadaAdminPorIdAsync(
        int idTemporada,
        CancellationToken cancellationToken = default)
    {
        ValidarId(idTemporada);

        return await _temporadaRepository.ObtenerTemporadaAdminPorIdAsync(idTemporada, cancellationToken);
    }

    public async Task<int> CrearTemporadaAsync(
        TemporadaFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidarFormulario(request);

        return await _temporadaRepository.CrearTemporadaAsync(request, cancellationToken);
    }

    public async Task<bool> ActualizarTemporadaAsync(
        TemporadaFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.IdTemporada <= 0)
            throw new ArgumentException("El identificador de la temporada no es válido.", nameof(request.IdTemporada));

        ValidarFormulario(request);

        return await _temporadaRepository.ActualizarTemporadaAsync(request, cancellationToken);
    }

    public async Task<bool> InactivarTemporadaAsync(
        int idTemporada,
        CancellationToken cancellationToken = default)
    {
        ValidarId(idTemporada);

        return await _temporadaRepository.InactivarTemporadaAsync(idTemporada, cancellationToken);
    }

    public async Task<bool> ActivarTemporadaAsync(
        int idTemporada,
        CancellationToken cancellationToken = default)
    {
        ValidarId(idTemporada);

        return await _temporadaRepository.ActivarTemporadaAsync(idTemporada, cancellationToken);
    }

    private static void ValidarId(int idTemporada)
    {
        if (idTemporada <= 0)
            throw new ArgumentException("El identificador de la temporada no es válido.", nameof(idTemporada));
    }

    private static void ValidarFormulario(TemporadaFormularioRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La información de la temporada no puede ser nula.");

        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new ArgumentException("El nombre de la temporada es obligatorio.", nameof(request.Nombre));
    }
}
