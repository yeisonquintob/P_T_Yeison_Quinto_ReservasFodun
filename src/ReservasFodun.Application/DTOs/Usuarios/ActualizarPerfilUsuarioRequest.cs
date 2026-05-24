namespace ReservasFodun.Application.DTOs.Usuarios;

public class ActualizarPerfilUsuarioRequest
{
    public string? TipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }

    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }

    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}
