namespace ReservasFodun.Domain.Entities;

public class ServicioAdicional
{
    public int IdServicioAdicional { get; set; }

    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public decimal Valor { get; set; }

    public bool PorPersona { get; set; }
    public bool PorNoche { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<ReservaServicioAdicional> ReservaServiciosAdicionales { get; set; } = new List<ReservaServicioAdicional>();
}
