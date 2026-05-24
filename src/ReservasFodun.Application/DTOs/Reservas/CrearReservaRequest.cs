namespace ReservasFodun.Application.DTOs.Reservas;

public class CrearReservaRequest
{
    public int IdSede { get; set; }
    public int IdAlojamiento { get; set; }

    public DateTime FechaLlegada { get; set; }
    public DateTime FechaSalida { get; set; }

    public int NumeroPersonas { get; set; }
    public int NumeroHabitaciones { get; set; }

    public decimal ValorSubtotal { get; set; }
    public decimal ValorTotal { get; set; }

    public string? Observaciones { get; set; }

    public List<CrearReservaServicioAdicionalRequest> ServiciosAdicionales { get; set; } = new();
}
