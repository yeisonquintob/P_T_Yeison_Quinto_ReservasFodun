namespace ReservasFodun.Domain.Entities;

public class AlojamientoCaracteristica
{
    public int IdAlojamientoCaracteristica { get; set; }
    public int IdAlojamiento { get; set; }
    public int IdCaracteristica { get; set; }

    public Alojamiento? Alojamiento { get; set; }
    public Caracteristica? Caracteristica { get; set; }
}
