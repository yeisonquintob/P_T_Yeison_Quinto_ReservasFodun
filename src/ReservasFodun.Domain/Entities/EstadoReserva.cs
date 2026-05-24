namespace ReservasFodun.Domain.Entities;

public class EstadoReserva
{
    public int IdEstadoReserva { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public bool BloqueaDisponibilidad { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
