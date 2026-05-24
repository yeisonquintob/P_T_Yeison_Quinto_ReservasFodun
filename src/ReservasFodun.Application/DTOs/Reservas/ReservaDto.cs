namespace ReservasFodun.Application.DTOs.Reservas;

public class ReservaDto
{
    public int IdReserva { get; set; }

    public string CodigoReserva { get; set; } = string.Empty;

    public string IdUsuario { get; set; } = string.Empty;

    public int IdSede { get; set; }

    public string NombreSede { get; set; } = string.Empty;

    public int IdEstadoReserva { get; set; }

    public string EstadoReserva { get; set; } = string.Empty;

    public DateTime FechaLlegada { get; set; }

    public DateTime FechaSalida { get; set; }

    public int NumeroNoches { get; set; }

    public int NumeroPersonas { get; set; }

    public int NumeroHabitaciones { get; set; }

    public decimal ValorSubtotal { get; set; }

    public decimal ValorTotal { get; set; }

    public string? Observaciones { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public bool PermiteCancelar { get; set; }

    public List<ReservaDetalleDto> Detalles { get; set; } = new();
}
