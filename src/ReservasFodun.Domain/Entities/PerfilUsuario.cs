namespace ReservasFodun.Domain.Entities;

public class PerfilUsuario
{
    public int IdPerfilUsuario { get; set; }

    public string IdUsuario { get; set; } = string.Empty;

    public string? TipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }

    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }

    public bool Activo { get; set; } = true;
}
