namespace ReservasFodun.Application.DTOs.Admin;

public class TarifaAdminDto
{
    public int IdTarifa { get; set; }

    public int? IdSede { get; set; }
    public int? IdAlojamiento { get; set; }
    public int IdTemporada { get; set; }

    public int? NumeroHabitacionesTarifa { get; set; }
    public int PersonasIncluidas { get; set; }

    public decimal TarifaBase { get; set; }
    public decimal ValorPersonaAdicional { get; set; }

    public bool EsTarifaEspecial { get; set; }
    public bool Activo { get; set; }

    public string? Descripcion { get; set; }

    public string? NombreSede { get; set; }
    public string? NombreAlojamiento { get; set; }
    public string? NombreTemporada { get; set; }
}
