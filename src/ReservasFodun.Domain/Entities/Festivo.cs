namespace ReservasFodun.Domain.Entities;

public class Festivo
{
    public int IdFestivo { get; set; }
    public DateTime Fecha { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
