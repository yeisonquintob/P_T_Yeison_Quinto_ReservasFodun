namespace ReservasFodun.Application.DTOs.Admin;

public class AlojamientoAdminDto
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

    public bool Activo { get; set; }

    public string? NombreSede { get; set; }
    public string? TipoAlojamiento { get; set; }
}
