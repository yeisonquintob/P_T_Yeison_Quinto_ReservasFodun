namespace ReservasFodun.Application.DTOs.Tarifas;

public class CalculoTarifaResultadoDto
{
    public int IdTarifa { get; set; }

    public int IdSede { get; set; }
    public int IdAlojamiento { get; set; }
    public int IdTemporada { get; set; }

    public string? NombreSede { get; set; }
    public string? NombreAlojamiento { get; set; }
    public string? NombreTemporada { get; set; }

    public DateTime FechaLlegada { get; set; }
    public DateTime FechaSalida { get; set; }

    public int NumeroNoches { get; set; }
    public int NumeroPersonas { get; set; }
    public int PersonasIncluidas { get; set; }
    public int PersonasAdicionales { get; set; }

    public decimal TarifaBase { get; set; }
    public decimal ValorPersonaAdicional { get; set; }
    public decimal ValorPersonasAdicionales { get; set; }
    public decimal ValorTotal { get; set; }

    public string? Descripcion { get; set; }
}
