namespace ReservasFodun.Domain.Entities;

public class ReservaServicioAdicional
{
    public int IdReservaServicioAdicional { get; set; }

    public int IdReserva { get; set; }
    public int IdServicioAdicional { get; set; }

    public int Cantidad { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }

    public Reserva? Reserva { get; set; }
    public ServicioAdicional? ServicioAdicional { get; set; }
}
