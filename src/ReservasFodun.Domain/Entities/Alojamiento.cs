namespace ReservasFodun.Domain.Entities;

public class Alojamiento
{
    public int IdAlojamiento { get; set; }
    public int IdSede { get; set; }
    public int IdTipoAlojamiento { get; set; }

    public string NumeroAlojamiento { get; set; } = string.Empty;
    public string NombreAlojamiento { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public int NumeroHabitaciones { get; set; }
    public int CapacidadMaxima { get; set; }
    public int? NumeroHabitacionesTarifa { get; set; }

    public bool Activo { get; set; } = true;

    public Sede? Sede { get; set; }
    public TipoAlojamiento? TipoAlojamiento { get; set; }

    public ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();
    public ICollection<ReservaDetalle> ReservaDetalles { get; set; } = new List<ReservaDetalle>();
    public ICollection<AlojamientoCaracteristica> AlojamientoCaracteristicas { get; set; } = new List<AlojamientoCaracteristica>();
}
