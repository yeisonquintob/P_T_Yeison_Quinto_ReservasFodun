namespace ReservasFodun.Domain.Entities;

public class TipoAlojamiento
{
    public int IdTipoAlojamiento { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Alojamiento> Alojamientos { get; set; } = new List<Alojamiento>();
}
