namespace ReservasFodun.Application.DTOs.Alojamientos;

public class AlojamientoDisponibleDto
{
    public int IdSede { get; set; }
    public string NombreSede { get; set; } = string.Empty;

    public int IdAlojamiento { get; set; }
    public string NumeroAlojamiento { get; set; } = string.Empty;
    public string NombreAlojamiento { get; set; } = string.Empty;
    public string? TipoAlojamiento { get; set; }

    public int NumeroHabitaciones { get; set; }
    public int CapacidadMaxima { get; set; }

    public DateTime FechaLlegada { get; set; }
    public DateTime FechaSalida { get; set; }
    public int NumeroNoches { get; set; }

    public bool Disponible { get; set; }
    public string? ObservacionDisponibilidad { get; set; }
}
