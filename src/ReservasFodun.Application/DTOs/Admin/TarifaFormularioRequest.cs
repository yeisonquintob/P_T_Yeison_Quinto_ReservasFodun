using System.ComponentModel.DataAnnotations;

namespace ReservasFodun.Application.DTOs.Admin;

public class TarifaFormularioRequest
{
    public int IdTarifa { get; set; }

    [Display(Name = "Sede")]
    public int? IdSede { get; set; }

    [Display(Name = "Alojamiento")]
    public int? IdAlojamiento { get; set; }

    [Display(Name = "Temporada")]
    [Range(1, int.MaxValue, ErrorMessage = "La temporada es obligatoria.")]
    public int IdTemporada { get; set; }

    [Display(Name = "Habitaciones tarifa")]
    public int? NumeroHabitacionesTarifa { get; set; }

    [Display(Name = "Personas incluidas")]
    [Range(1, int.MaxValue, ErrorMessage = "Las personas incluidas deben ser mayor que cero.")]
    public int PersonasIncluidas { get; set; } = 1;

    [Display(Name = "Tarifa base")]
    [Range(0, double.MaxValue, ErrorMessage = "La tarifa base debe ser mayor o igual a cero.")]
    public decimal TarifaBase { get; set; }

    [Display(Name = "Valor persona adicional")]
    [Range(0, double.MaxValue, ErrorMessage = "El valor por persona adicional debe ser mayor o igual a cero.")]
    public decimal ValorPersonaAdicional { get; set; }

    [Display(Name = "Es tarifa especial")]
    public bool EsTarifaEspecial { get; set; }

    [Display(Name = "Descripción")]
    [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
    public string? Descripcion { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
