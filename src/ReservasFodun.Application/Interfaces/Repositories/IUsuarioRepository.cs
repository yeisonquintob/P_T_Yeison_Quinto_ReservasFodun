using ReservasFodun.Application.DTOs.Usuarios;

namespace ReservasFodun.Application.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<PerfilUsuarioDto?> ObtenerPerfilPorUsuarioIdAsync(
        string idUsuario,
        CancellationToken cancellationToken = default);

    Task<int> CrearPerfilUsuarioAsync(
        string idUsuario,
        RegistroUsuarioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> ActualizarPerfilUsuarioAsync(
        string idUsuario,
        ActualizarPerfilUsuarioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteNumeroDocumentoAsync(
        string numeroDocumento,
        CancellationToken cancellationToken = default);
}
