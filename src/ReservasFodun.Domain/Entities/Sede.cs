namespace ReservasFodun.Domain.Entities;

public class Sede
{
    public int IdSede { get; set; }
    public int IdTipoSede { get; set; }
    public int IdMunicipio { get; set; }

    public string NombreSede { get; set; } = string.Empty;
    public string? NombreCorto { get; set; }
    public string? Direccion { get; set; }
    public string? Descripcion { get; set; }

    public int CapacidadTotal { get; set; }
    public bool Activo { get; set; } = true;

    public TipoSede? TipoSede { get; set; }
    public Municipio? Municipio { get; set; }

    public ICollection<Alojamiento> Alojamientos { get; set; } = new List<Alojamiento>();
    public ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
