namespace ReservasFodun.Application.DTOs.Reservas;

public class ReservaDetalleDto
{
    public int IdReservaDetalle { get; set; }

    public int IdReserva { get; set; }
    public int IdAlojamiento { get; set; }

    public string NombreAlojamiento { get; set; } = string.Empty;
    public string? TipoAlojamiento { get; set; }

    public int NumeroPersonas { get; set; }
    public int NumeroHabitaciones { get; set; }

    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
}
