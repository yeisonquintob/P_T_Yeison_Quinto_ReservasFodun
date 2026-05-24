namespace ReservasFodun.Domain.Entities;

public class Tarifa
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

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;

    public Sede? Sede { get; set; }
    public Alojamiento? Alojamiento { get; set; }
    public Temporada? Temporada { get; set; }
}
