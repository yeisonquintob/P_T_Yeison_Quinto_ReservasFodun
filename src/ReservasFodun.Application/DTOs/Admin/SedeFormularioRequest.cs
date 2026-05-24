using System.ComponentModel.DataAnnotations;

namespace ReservasFodun.Application.DTOs.Admin;

public class SedeFormularioRequest
{
    public int IdSede { get; set; }

    [Display(Name = "Tipo de sede")]
    [Range(1, int.MaxValue, ErrorMessage = "El tipo de sede es obligatorio.")]
    public int IdTipoSede { get; set; }

    [Display(Name = "Municipio")]
    [Range(1, int.MaxValue, ErrorMessage = "El municipio es obligatorio.")]
    public int IdMunicipio { get; set; }

    [Display(Name = "Nombre de la sede")]
    [Required(ErrorMessage = "El nombre de la sede es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string NombreSede { get; set; } = string.Empty;

    [Display(Name = "Nombre corto")]
    [StringLength(100, ErrorMessage = "El nombre corto no puede superar los 100 caracteres.")]
    public string? NombreCorto { get; set; }

    [Display(Name = "Dirección")]
    [StringLength(250, ErrorMessage = "La dirección no puede superar los 250 caracteres.")]
    public string? Direccion { get; set; }

    [Display(Name = "Descripción")]
    [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
    public string? Descripcion { get; set; }

    [Display(Name = "Capacidad total")]
    [Range(1, int.MaxValue, ErrorMessage = "La capacidad total debe ser mayor que cero.")]
    public int CapacidadTotal { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
