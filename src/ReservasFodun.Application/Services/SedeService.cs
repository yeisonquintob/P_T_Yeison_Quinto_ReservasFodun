using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.DTOs.Alojamientos;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Application.Services;

public class SedeService : ISedeService
{
    private readonly ISedeRepository _sedeRepository;

    public SedeService(ISedeRepository sedeRepository)
    {
        _sedeRepository = sedeRepository;
    }

    public async Task<IEnumerable<SedeDto>> ObtenerSedesAsync(CancellationToken cancellationToken = default)
    {
        return await _sedeRepository.ObtenerSedesAsync(cancellationToken);
    }

    public async Task<SedeDto?> ObtenerSedePorIdAsync(int idSede, CancellationToken cancellationToken = default)
    {
        ValidarIdSede(idSede);
        return await _sedeRepository.ObtenerSedePorIdAsync(idSede, cancellationToken);
    }

    public async Task<IEnumerable<SedeAdminDto>> ObtenerSedesAdminAsync(CancellationToken cancellationToken = default)
    {
        return await _sedeRepository.ObtenerSedesAdminAsync(cancellationToken);
    }

    public async Task<SedeAdminDto?> ObtenerSedeAdminPorIdAsync(int idSede, CancellationToken cancellationToken = default)
    {
        ValidarIdSede(idSede);
        return await _sedeRepository.ObtenerSedeAdminPorIdAsync(idSede, cancellationToken);
    }

    public async Task<int> CrearSedeAsync(
        SedeFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidarFormulario(request);

        return await _sedeRepository.CrearSedeAsync(request, cancellationToken);
    }

    public async Task<bool> ActualizarSedeAsync(
        SedeFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.IdSede <= 0)
            throw new ArgumentException("El identificador de la sede no es válido.", nameof(request.IdSede));

        ValidarFormulario(request);

        return await _sedeRepository.ActualizarSedeAsync(request, cancellationToken);
    }

    public async Task<bool> InactivarSedeAsync(int idSede, CancellationToken cancellationToken = default)
    {
        ValidarIdSede(idSede);

        return await _sedeRepository.InactivarSedeAsync(idSede, cancellationToken);
    }

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerTiposSedeAsync(CancellationToken cancellationToken = default)
    {
        return await _sedeRepository.ObtenerTiposSedeAsync(cancellationToken);
    }

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerMunicipiosAsync(CancellationToken cancellationToken = default)
    {
        return await _sedeRepository.ObtenerMunicipiosAsync(cancellationToken);
    }

    private static void ValidarIdSede(int idSede)
    {
        if (idSede <= 0)
            throw new ArgumentException("El identificador de la sede no es válido.", nameof(idSede));
    }

    private static void ValidarFormulario(SedeFormularioRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La información de la sede no puede ser nula.");

        if (request.IdTipoSede <= 0)
            throw new ArgumentException("El tipo de sede es obligatorio.", nameof(request.IdTipoSede));

        if (request.IdMunicipio <= 0)
            throw new ArgumentException("El municipio es obligatorio.", nameof(request.IdMunicipio));

        if (string.IsNullOrWhiteSpace(request.NombreSede))
            throw new ArgumentException("El nombre de la sede es obligatorio.", nameof(request.NombreSede));

        if (request.CapacidadTotal <= 0)
            throw new ArgumentException("La capacidad total debe ser mayor que cero.", nameof(request.CapacidadTotal));
    }
}
