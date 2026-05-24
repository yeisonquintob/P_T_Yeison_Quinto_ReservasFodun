namespace ReservasFodun.Application.DTOs.Reservas;

public class CrearReservaServicioAdicionalRequest
{
    public int IdServicioAdicional { get; set; }
    public int Cantidad { get; set; }
    public decimal ValorUnitario { get; set; }
}
