using System.ComponentModel.DataAnnotations;

namespace ReservasFodun.Application.DTOs.Admin;

public class AlojamientoFormularioRequest
{
    public int IdAlojamiento { get; set; }

    [Display(Name = "Sede")]
    [Range(1, int.MaxValue, ErrorMessage = "La sede es obligatoria.")]
    public int IdSede { get; set; }

    [Display(Name = "Tipo de alojamiento")]
    [Range(1, int.MaxValue, ErrorMessage = "El tipo de alojamiento es obligatorio.")]
    public int IdTipoAlojamiento { get; set; }

    [Display(Name = "Número")]
    [Required(ErrorMessage = "El número del alojamiento es obligatorio.")]
    [StringLength(50, ErrorMessage = "El número no puede superar los 50 caracteres.")]
    public string NumeroAlojamiento { get; set; } = string.Empty;

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El nombre del alojamiento es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string NombreAlojamiento { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
    public string? Descripcion { get; set; }

    [Display(Name = "Número de habitaciones")]
    [Range(1, int.MaxValue, ErrorMessage = "El número de habitaciones debe ser mayor que cero.")]
    public int NumeroHabitaciones { get; set; }

    [Display(Name = "Capacidad máxima")]
    [Range(1, int.MaxValue, ErrorMessage = "La capacidad máxima debe ser mayor que cero.")]
    public int CapacidadMaxima { get; set; }

    [Display(Name = "Habitaciones para tarifa")]
    public int? NumeroHabitacionesTarifa { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
