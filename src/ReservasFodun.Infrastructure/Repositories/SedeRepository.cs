using Microsoft.EntityFrameworkCore;
using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.DTOs.Alojamientos;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Domain.Entities;
using ReservasFodun.Infrastructure.Data;

namespace ReservasFodun.Infrastructure.Repositories;

public class SedeRepository : ISedeRepository
{
    private readonly ApplicationDbContext _context;

    public SedeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SedeDto>> ObtenerSedesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sedes
            .AsNoTracking()
            .Include(x => x.TipoSede)
            .Include(x => x.Municipio)
                .ThenInclude(x => x!.Departamento)
            .Where(x => x.Activo)
            .Select(x => new SedeDto
            {
                IdSede = x.IdSede,
                NombreSede = x.NombreSede,
                NombreCorto = x.NombreCorto,
                TipoSede = x.TipoSede != null ? x.TipoSede.Nombre : null,
                Municipio = x.Municipio != null ? x.Municipio.Nombre : null,
                Departamento = x.Municipio != null && x.Municipio.Departamento != null
                    ? x.Municipio.Departamento.Nombre
                    : null,
                Direccion = x.Direccion,
                Descripcion = x.Descripcion,
                CapacidadTotal = x.CapacidadTotal
            })
            .OrderBy(x => x.NombreSede)
            .ToListAsync(cancellationToken);
    }

    public async Task<SedeDto?> ObtenerSedePorIdAsync(
        int idSede,
        CancellationToken cancellationToken = default)
    {
        return await _context.Sedes
            .AsNoTracking()
            .Include(x => x.TipoSede)
            .Include(x => x.Municipio)
                .ThenInclude(x => x!.Departamento)
            .Where(x => x.IdSede == idSede && x.Activo)
            .Select(x => new SedeDto
            {
                IdSede = x.IdSede,
                NombreSede = x.NombreSede,
                NombreCorto = x.NombreCorto,
                TipoSede = x.TipoSede != null ? x.TipoSede.Nombre : null,
                Municipio = x.Municipio != null ? x.Municipio.Nombre : null,
                Departamento = x.Municipio != null && x.Municipio.Departamento != null
                    ? x.Municipio.Departamento.Nombre
                    : null,
                Direccion = x.Direccion,
                Descripcion = x.Descripcion,
                CapacidadTotal = x.CapacidadTotal
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<SedeAdminDto>> ObtenerSedesAdminAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Sedes
            .AsNoTracking()
            .Include(x => x.TipoSede)
            .Include(x => x.Municipio)
                .ThenInclude(x => x!.Departamento)
            .Select(x => new SedeAdminDto
            {
                IdSede = x.IdSede,
                IdTipoSede = x.IdTipoSede,
                IdMunicipio = x.IdMunicipio,
                NombreSede = x.NombreSede,
                NombreCorto = x.NombreCorto,
                Direccion = x.Direccion,
                Descripcion = x.Descripcion,
                CapacidadTotal = x.CapacidadTotal,
                Activo = x.Activo,
                TipoSede = x.TipoSede != null ? x.TipoSede.Nombre : null,
                Municipio = x.Municipio != null ? x.Municipio.Nombre : null,
                Departamento = x.Municipio != null && x.Municipio.Departamento != null
                    ? x.Municipio.Departamento.Nombre
                    : null
            })
            .OrderByDescending(x => x.Activo)
            .ThenBy(x => x.NombreSede)
            .ToListAsync(cancellationToken);
    }

    public async Task<SedeAdminDto?> ObtenerSedeAdminPorIdAsync(
        int idSede,
        CancellationToken cancellationToken = default)
    {
        return await _context.Sedes
            .AsNoTracking()
            .Include(x => x.TipoSede)
            .Include(x => x.Municipio)
                .ThenInclude(x => x!.Departamento)
            .Where(x => x.IdSede == idSede)
            .Select(x => new SedeAdminDto
            {
                IdSede = x.IdSede,
                IdTipoSede = x.IdTipoSede,
                IdMunicipio = x.IdMunicipio,
                NombreSede = x.NombreSede,
                NombreCorto = x.NombreCorto,
                Direccion = x.Direccion,
                Descripcion = x.Descripcion,
                CapacidadTotal = x.CapacidadTotal,
                Activo = x.Activo,
                TipoSede = x.TipoSede != null ? x.TipoSede.Nombre : null,
                Municipio = x.Municipio != null ? x.Municipio.Nombre : null,
                Departamento = x.Municipio != null && x.Municipio.Departamento != null
                    ? x.Municipio.Departamento.Nombre
                    : null
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CrearSedeAsync(
        SedeFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        var sede = new Sede
        {
            IdTipoSede = request.IdTipoSede,
            IdMunicipio = request.IdMunicipio,
            NombreSede = request.NombreSede.Trim(),
            NombreCorto = string.IsNullOrWhiteSpace(request.NombreCorto) ? null : request.NombreCorto.Trim(),
            Direccion = string.IsNullOrWhiteSpace(request.Direccion) ? null : request.Direccion.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim(),
            CapacidadTotal = request.CapacidadTotal,
            Activo = request.Activo
        };

        _context.Sedes.Add(sede);

        await _context.SaveChangesAsync(cancellationToken);

        return sede.IdSede;
    }

    public async Task<bool> ActualizarSedeAsync(
        SedeFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        var sede = await _context.Sedes
            .FirstOrDefaultAsync(x => x.IdSede == request.IdSede, cancellationToken);

        if (sede is null)
            return false;

        sede.IdTipoSede = request.IdTipoSede;
        sede.IdMunicipio = request.IdMunicipio;
        sede.NombreSede = request.NombreSede.Trim();
        sede.NombreCorto = string.IsNullOrWhiteSpace(request.NombreCorto) ? null : request.NombreCorto.Trim();
        sede.Direccion = string.IsNullOrWhiteSpace(request.Direccion) ? null : request.Direccion.Trim();
        sede.Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim();
        sede.CapacidadTotal = request.CapacidadTotal;
        sede.Activo = request.Activo;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> InactivarSedeAsync(
        int idSede,
        CancellationToken cancellationToken = default)
    {
        var sede = await _context.Sedes
            .FirstOrDefaultAsync(x => x.IdSede == idSede, cancellationToken);

        if (sede is null)
            return false;

        sede.Activo = false;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerTiposSedeAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.TiposSede
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Nombre)
            .Select(x => new CatalogoSelectDto
            {
                Id = x.IdTipoSede,
                Nombre = x.Nombre
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CatalogoSelectDto>> ObtenerMunicipiosAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Municipios
            .AsNoTracking()
            .Include(x => x.Departamento)
            .OrderBy(x => x.Nombre)
            .Select(x => new CatalogoSelectDto
            {
                Id = x.IdMunicipio,
                Nombre = x.Departamento != null
                    ? x.Nombre + " - " + x.Departamento.Nombre
                    : x.Nombre
            })
            .ToListAsync(cancellationToken);
    }
}
