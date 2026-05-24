namespace ReservasFodun.Application.DTOs.Tarifas;

public class ConsultarTarifaRequest
{
    public int IdSede { get; set; }
    public int? IdAlojamiento { get; set; }
    public int IdTemporada { get; set; }
    public int NumeroPersonas { get; set; }
    public int? NumeroHabitaciones { get; set; }
}
