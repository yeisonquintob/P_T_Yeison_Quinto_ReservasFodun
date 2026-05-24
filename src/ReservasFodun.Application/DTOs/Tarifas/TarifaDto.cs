namespace ReservasFodun.Application.DTOs.Tarifas;

public class TarifaDto
{
    public int IdTarifa { get; set; }

    public int? IdSede { get; set; }
    public string? NombreSede { get; set; }

    public int? IdAlojamiento { get; set; }
    public string? NombreAlojamiento { get; set; }

    public int IdTemporada { get; set; }
    public string NombreTemporada { get; set; } = string.Empty;

    public int? NumeroHabitacionesTarifa { get; set; }
    public int PersonasIncluidas { get; set; }

    public decimal TarifaBase { get; set; }
    public decimal ValorPersonaAdicional { get; set; }

    public string? TipoTarifa { get; set; }
}
