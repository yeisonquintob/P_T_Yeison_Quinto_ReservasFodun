namespace ReservasFodun.Application.DTOs.Alojamientos;

public class ConsultarDisponibilidadRequest
{
    public int? IdSede { get; set; }
    public DateTime FechaLlegada { get; set; }
    public DateTime FechaSalida { get; set; }
    public int? NumeroPersonas { get; set; }
}
