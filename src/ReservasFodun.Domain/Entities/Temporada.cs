namespace ReservasFodun.Domain.Entities;

public class Temporada
{
    public int IdTemporada { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public bool EsAlta { get; set; }
    public bool EsEspecial { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();
}
