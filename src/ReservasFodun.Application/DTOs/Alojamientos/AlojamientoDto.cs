namespace ReservasFodun.Application.DTOs.Alojamientos;

public class AlojamientoDto
{
    public int IdAlojamiento { get; set; }
    public int IdSede { get; set; }
    public string NombreSede { get; set; } = string.Empty;
    public string NumeroAlojamiento { get; set; } = string.Empty;
    public string NombreAlojamiento { get; set; } = string.Empty;
    public string? TipoAlojamiento { get; set; }
    public string? Descripcion { get; set; }
    public int NumeroHabitaciones { get; set; }
    public int CapacidadMaxima { get; set; }
    public bool Activo { get; set; }
}
