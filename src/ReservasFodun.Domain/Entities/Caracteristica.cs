namespace ReservasFodun.Domain.Entities;

public class Caracteristica
{
    public int IdCaracteristica { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<AlojamientoCaracteristica> AlojamientoCaracteristicas { get; set; } = new List<AlojamientoCaracteristica>();
}
