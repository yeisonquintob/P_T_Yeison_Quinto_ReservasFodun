namespace ReservasFodun.Application.DTOs.Reservas;

public class MisReservasDto
{
    public int IdReserva { get; set; }

    public string NombreSede { get; set; } = string.Empty;
    public string EstadoReserva { get; set; } = string.Empty;

    public DateTime FechaLlegada { get; set; }
    public DateTime FechaSalida { get; set; }

    public int NumeroPersonas { get; set; }
    public int NumeroHabitaciones { get; set; }

    public decimal ValorTotal { get; set; }

    public DateTime FechaCreacion { get; set; }

    public bool PermiteCancelar { get; set; }
}
