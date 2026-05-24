namespace ReservasFodun.Application.DTOs.Usuarios;

public class RegistroUsuarioRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? TipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }

    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }

    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}
