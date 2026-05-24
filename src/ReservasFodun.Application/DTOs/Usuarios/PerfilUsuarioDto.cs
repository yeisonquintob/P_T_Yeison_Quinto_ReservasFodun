namespace ReservasFodun.Application.DTOs.Usuarios;

public class PerfilUsuarioDto
{
    public int IdPerfilUsuario { get; set; }

    public string IdUsuario { get; set; } = string.Empty;
    public string? Email { get; set; }

    public string? TipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }

    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }

    public string? NombreCompleto => $"{Nombres} {Apellidos}".Trim();

    public string? Telefono { get; set; }
    public string? Direccion { get; set; }

    public bool Activo { get; set; }
}
