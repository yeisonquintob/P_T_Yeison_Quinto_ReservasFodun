namespace ReservasFodun.Application.DTOs.Tarifas;

public class CalculoTarifaRequestDto
{
    public int IdSede { get; set; }
    public int IdAlojamiento { get; set; }

    public DateTime FechaLlegada { get; set; }
    public DateTime FechaSalida { get; set; }

    public int NumeroPersonas { get; set; }
    public int NumeroHabitaciones { get; set; }
}
