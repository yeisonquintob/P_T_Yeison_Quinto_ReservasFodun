namespace ReservasFodun.Domain.Entities;

public class TipoSede
{
    public int IdTipoSede { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Sede> Sedes { get; set; } = new List<Sede>();
}
