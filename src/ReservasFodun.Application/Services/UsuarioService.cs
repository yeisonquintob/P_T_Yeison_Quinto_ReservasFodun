using ReservasFodun.Application.DTOs.Usuarios;
using ReservasFodun.Application.Interfaces.Repositories;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<PerfilUsuarioDto?> ObtenerPerfilPorUsuarioIdAsync(
        string idUsuario,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idUsuario))
            throw new ArgumentException("El identificador del usuario es obligatorio.", nameof(idUsuario));

        return await _usuarioRepository.ObtenerPerfilPorUsuarioIdAsync(
            idUsuario,
            cancellationToken);
    }

    public async Task<int> CrearPerfilUsuarioAsync(
        string idUsuario,
        RegistroUsuarioRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idUsuario))
            throw new ArgumentException("El identificador del usuario es obligatorio.", nameof(idUsuario));

        ValidarRegistro(request);

        var numeroDocumento = request.NumeroDocumento?.Trim()
            ?? throw new ArgumentException("El número de documento es obligatorio.", nameof(request.NumeroDocumento));

        var documentoExiste = await _usuarioRepository.ExisteNumeroDocumentoAsync(
            numeroDocumento,
            cancellationToken);

        if (documentoExiste)
            throw new InvalidOperationException("Ya existe un usuario registrado con este número de documento.");

        return await _usuarioRepository.CrearPerfilUsuarioAsync(
            idUsuario,
            request,
            cancellationToken);
    }

    public async Task<bool> ActualizarPerfilUsuarioAsync(
        string idUsuario,
        ActualizarPerfilUsuarioRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idUsuario))
            throw new ArgumentException("El identificador del usuario es obligatorio.", nameof(idUsuario));

        if (request is null)
            throw new ArgumentNullException(nameof(request), "La información del perfil no puede ser nula.");

        if (string.IsNullOrWhiteSpace(request.NumeroDocumento))
            throw new ArgumentException("El número de documento es obligatorio.", nameof(request.NumeroDocumento));

        if (string.IsNullOrWhiteSpace(request.Nombres))
            throw new ArgumentException("Los nombres son obligatorios.", nameof(request.Nombres));

        if (string.IsNullOrWhiteSpace(request.Apellidos))
            throw new ArgumentException("Los apellidos son obligatorios.", nameof(request.Apellidos));

        return await _usuarioRepository.ActualizarPerfilUsuarioAsync(
            idUsuario,
            request,
            cancellationToken);
    }

    public async Task<bool> ExisteNumeroDocumentoAsync(
        string numeroDocumento,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(numeroDocumento))
            return false;

        return await _usuarioRepository.ExisteNumeroDocumentoAsync(
            numeroDocumento.Trim(),
            cancellationToken);
    }

    private static void ValidarRegistro(RegistroUsuarioRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La información de registro no puede ser nula.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("El correo electrónico es obligatorio.", nameof(request.Email));

        if (string.IsNullOrWhiteSpace(request.TipoDocumento))
            throw new ArgumentException("El tipo de documento es obligatorio.", nameof(request.TipoDocumento));

        if (string.IsNullOrWhiteSpace(request.NumeroDocumento))
            throw new ArgumentException("El número de documento es obligatorio.", nameof(request.NumeroDocumento));

        if (string.IsNullOrWhiteSpace(request.Nombres))
            throw new ArgumentException("Los nombres son obligatorios.", nameof(request.Nombres));

        if (string.IsNullOrWhiteSpace(request.Apellidos))
            throw new ArgumentException("Los apellidos son obligatorios.", nameof(request.Apellidos));
    }
}
