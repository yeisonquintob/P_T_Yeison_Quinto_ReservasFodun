using System.ComponentModel.DataAnnotations;

namespace ReservasFodun.Application.DTOs.Admin;

public class TipoAlojamientoFormularioRequest
{
    public int IdTipoAlojamiento { get; set; }

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El nombre del tipo de alojamiento es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
