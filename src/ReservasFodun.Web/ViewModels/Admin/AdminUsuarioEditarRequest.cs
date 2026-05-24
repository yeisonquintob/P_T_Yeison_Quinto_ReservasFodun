using System.ComponentModel.DataAnnotations;

namespace ReservasFodun.Web.ViewModels.Admin;

public class AdminUsuarioEditarRequest
{
    public string Id { get; set; } = string.Empty;

    [Display(Name = "Usuario")]
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "Correo electrónico")]
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Teléfono")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Correo confirmado")]
    public bool EmailConfirmed { get; set; }

    [Display(Name = "Bloqueo habilitado")]
    public bool LockoutEnabled { get; set; }
}
