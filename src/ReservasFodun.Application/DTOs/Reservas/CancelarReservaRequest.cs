namespace ReservasFodun.Application.DTOs.Reservas;

public class CancelarReservaRequest
{
    public int IdReserva { get; set; }
    public string? MotivoCancelacion { get; set; }
}
