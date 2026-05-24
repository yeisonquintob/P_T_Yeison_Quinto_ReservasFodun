namespace ReservasFodun.Domain.Entities;

public class Reserva
{
    public int IdReserva { get; set; }

    public string CodigoReserva { get; set; } = string.Empty;

    public string IdUsuario { get; set; } = string.Empty;

    public int IdSede { get; set; }

    public int IdEstadoReserva { get; set; }

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

    public Sede? Sede { get; set; }

    public EstadoReserva? EstadoReserva { get; set; }

    public ICollection<ReservaDetalle> ReservaDetalles { get; set; } = new List<ReservaDetalle>();

    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public ICollection<ReservaServicioAdicional> ReservaServiciosAdicionales { get; set; } = new List<ReservaServicioAdicional>();
}
