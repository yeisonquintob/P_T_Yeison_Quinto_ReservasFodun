namespace ReservasFodun.Domain.Entities;

public class Pago
{
    public int IdPago { get; set; }

    public int IdReserva { get; set; }

    public DateTime FechaPago { get; set; }
    public decimal ValorPagado { get; set; }

    public string? MetodoPago { get; set; }
    public string? ReferenciaPago { get; set; }
    public string? EstadoPago { get; set; }
    public string? Observaciones { get; set; }

    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioCreacion { get; set; }

    public Reserva? Reserva { get; set; }
}
