namespace ReservasFodun.Domain.Entities;

public class ReservaDetalle
{
    public int IdReservaDetalle { get; set; }

    public int IdReserva { get; set; }
    public int IdAlojamiento { get; set; }

    public int NumeroPersonas { get; set; }
    public int NumeroHabitaciones { get; set; }

    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }

    public Reserva? Reserva { get; set; }
    public Alojamiento? Alojamiento { get; set; }
}
