using System.ComponentModel.DataAnnotations;

namespace ReservasFodun.Application.DTOs.Admin;

public class ServicioAdicionalFormularioRequest
{
    public int IdServicioAdicional { get; set; }

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El nombre del servicio adicional es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }

    [Display(Name = "Valor")]
    [Range(0, double.MaxValue, ErrorMessage = "El valor debe ser mayor o igual a cero.")]
    public decimal Valor { get; set; }

    [Display(Name = "Se cobra por persona")]
    public bool PorPersona { get; set; }

    [Display(Name = "Se cobra por noche")]
    public bool PorNoche { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
