using Microsoft.EntityFrameworkCore;
using ReservasFodun.Application.DTOs.Usuarios;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Domain.Entities;
using ReservasFodun.Infrastructure.Data;

namespace ReservasFodun.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PerfilUsuarioDto?> ObtenerPerfilPorUsuarioIdAsync(
        string idUsuario,
        CancellationToken cancellationToken = default)
    {
        var perfil = await _context.Set<PerfilUsuario>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdUsuario == idUsuario, cancellationToken);

        if (perfil is null)
            return null;

        var usuario = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == idUsuario, cancellationToken);

        return new PerfilUsuarioDto
        {
            IdPerfilUsuario = perfil.IdPerfilUsuario,
            IdUsuario = perfil.IdUsuario,
            Email = usuario?.Email ?? string.Empty,
            TipoDocumento = perfil.TipoDocumento,
            NumeroDocumento = perfil.NumeroDocumento,
            Nombres = perfil.Nombres,
            Apellidos = perfil.Apellidos,
            Telefono = perfil.Telefono,
            Direccion = perfil.Direccion,
            Activo = perfil.Activo
        };
    }

    public async Task<int> CrearPerfilUsuarioAsync(
        string idUsuario,
        RegistroUsuarioRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La información de registro no puede ser nula.");

        var numeroDocumento = request.NumeroDocumento?.Trim();

        if (string.IsNullOrWhiteSpace(numeroDocumento))
            throw new ArgumentException("El número de documento es obligatorio.", nameof(request.NumeroDocumento));

        var existePerfil = await _context.Set<PerfilUsuario>()
            .AnyAsync(x => x.IdUsuario == idUsuario, cancellationToken);

        if (existePerfil)
            return 0;

        var existeDocumento = await ExisteNumeroDocumentoAsync(
            numeroDocumento,
            cancellationToken);

        if (existeDocumento)
            throw new InvalidOperationException("Ya existe un usuario registrado con este número de documento.");

        var perfil = new PerfilUsuario
        {
            IdUsuario = idUsuario,
            TipoDocumento = request.TipoDocumento?.Trim(),
            NumeroDocumento = numeroDocumento,
            Nombres = request.Nombres?.Trim() ?? string.Empty,
            Apellidos = request.Apellidos?.Trim() ?? string.Empty,
            Telefono = request.Telefono?.Trim(),
            Direccion = request.Direccion?.Trim(),
            Activo = true,
            FechaCreacion = DateTime.Now,
            FechaModificacion = DateTime.Now
        };

        _context.Set<PerfilUsuario>().Add(perfil);
        await _context.SaveChangesAsync(cancellationToken);

        return perfil.IdPerfilUsuario;
    }

    public async Task<bool> ActualizarPerfilUsuarioAsync(
        string idUsuario,
        ActualizarPerfilUsuarioRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La información del perfil no puede ser nula.");

        var perfil = await _context.Set<PerfilUsuario>()
            .FirstOrDefaultAsync(x => x.IdUsuario == idUsuario, cancellationToken);

        if (perfil is null)
            return false;

        perfil.TipoDocumento = request.TipoDocumento?.Trim();
        perfil.NumeroDocumento = request.NumeroDocumento?.Trim() ?? perfil.NumeroDocumento;
        perfil.Nombres = request.Nombres?.Trim() ?? perfil.Nombres;
        perfil.Apellidos = request.Apellidos?.Trim() ?? perfil.Apellidos;
        perfil.Telefono = request.Telefono?.Trim();
        perfil.Direccion = request.Direccion?.Trim();
        perfil.FechaModificacion = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ExisteNumeroDocumentoAsync(
        string numeroDocumento,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(numeroDocumento))
            return false;

        var documento = numeroDocumento.Trim();

        return await _context.Set<PerfilUsuario>()
            .AsNoTracking()
            .AnyAsync(x => x.NumeroDocumento == documento, cancellationToken);
    }
}
