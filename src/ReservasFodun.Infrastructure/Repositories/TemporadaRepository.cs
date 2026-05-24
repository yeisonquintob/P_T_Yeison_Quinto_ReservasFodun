using Microsoft.EntityFrameworkCore;
using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Domain.Entities;
using ReservasFodun.Infrastructure.Data;

namespace ReservasFodun.Infrastructure.Repositories;

public class TemporadaRepository : ITemporadaRepository
{
    private readonly ApplicationDbContext _context;

    public TemporadaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TemporadaAdminDto>> ObtenerTemporadasAdminAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Temporadas
            .AsNoTracking()
            .Select(x => new TemporadaAdminDto
            {
                IdTemporada = x.IdTemporada,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                EsAlta = x.EsAlta,
                EsEspecial = x.EsEspecial,
                Activo = x.Activo,
                TotalTarifas = _context.Tarifas.Count(t => t.IdTemporada == x.IdTemporada)
            })
            .OrderByDescending(x => x.Activo)
            .ThenBy(x => x.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<TemporadaAdminDto?> ObtenerTemporadaAdminPorIdAsync(
        int idTemporada,
        CancellationToken cancellationToken = default)
    {
        return await _context.Temporadas
            .AsNoTracking()
            .Where(x => x.IdTemporada == idTemporada)
            .Select(x => new TemporadaAdminDto
            {
                IdTemporada = x.IdTemporada,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                EsAlta = x.EsAlta,
                EsEspecial = x.EsEspecial,
                Activo = x.Activo,
                TotalTarifas = _context.Tarifas.Count(t => t.IdTemporada == x.IdTemporada)
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CrearTemporadaAsync(
        TemporadaFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        var temporada = new Temporada
        {
            Nombre = request.Nombre.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim(),
            EsAlta = request.EsAlta,
            EsEspecial = request.EsEspecial,
            Activo = request.Activo
        };

        _context.Temporadas.Add(temporada);

        await _context.SaveChangesAsync(cancellationToken);

        return temporada.IdTemporada;
    }

    public async Task<bool> ActualizarTemporadaAsync(
        TemporadaFormularioRequest request,
        CancellationToken cancellationToken = default)
    {
        var temporada = await _context.Temporadas
            .FirstOrDefaultAsync(x => x.IdTemporada == request.IdTemporada, cancellationToken);

        if (temporada is null)
            return false;

        temporada.Nombre = request.Nombre.Trim();
        temporada.Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim();
        temporada.EsAlta = request.EsAlta;
        temporada.EsEspecial = request.EsEspecial;
        temporada.Activo = request.Activo;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> InactivarTemporadaAsync(
        int idTemporada,
        CancellationToken cancellationToken = default)
    {
        var temporada = await _context.Temporadas
            .FirstOrDefaultAsync(x => x.IdTemporada == idTemporada, cancellationToken);

        if (temporada is null)
            return false;

        temporada.Activo = false;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ActivarTemporadaAsync(
        int idTemporada,
        CancellationToken cancellationToken = default)
    {
        var temporada = await _context.Temporadas
            .FirstOrDefaultAsync(x => x.IdTemporada == idTemporada, cancellationToken);

        if (temporada is null)
            return false;

        temporada.Activo = true;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
