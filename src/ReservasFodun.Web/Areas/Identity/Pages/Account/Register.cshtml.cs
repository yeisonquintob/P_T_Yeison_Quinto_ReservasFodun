using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReservasFodun.Application.DTOs.Usuarios;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Web.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IUsuarioService _usuarioService;

    public RegisterModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ILogger<RegisterModel> logger,
        IUsuarioService usuarioService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _usuarioService = usuarioService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

    public class InputModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
        [Display(Name = "Tipo documento")]
        public string TipoDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [StringLength(30, ErrorMessage = "El número de documento no puede superar los 30 caracteres.")]
        [Display(Name = "Número documento")]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los nombres no pueden superar los 100 caracteres.")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden superar los 100 caracteres.")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "El teléfono no puede superar los 30 caracteres.")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(250, ErrorMessage = "La dirección no puede superar los 250 caracteres.")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ReturnUrl = returnUrl;

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (!ModelState.IsValid)
            return Page();

        var documentoExiste = await _usuarioService.ExisteNumeroDocumentoAsync(
            Input.NumeroDocumento,
            HttpContext.RequestAborted);

        if (documentoExiste)
        {
            ModelState.AddModelError(
                "Input.NumeroDocumento",
                "Ya existe un usuario registrado con este número de documento.");

            return Page();
        }

        var user = new IdentityUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        _logger.LogInformation("Usuario registrado correctamente en AspNetUsers.");

        var perfilRequest = new RegistroUsuarioRequest
        {
            Email = Input.Email,
            Password = Input.Password,
            ConfirmPassword = Input.ConfirmPassword,
            TipoDocumento = Input.TipoDocumento,
            NumeroDocumento = Input.NumeroDocumento,
            Nombres = Input.Nombres,
            Apellidos = Input.Apellidos,
            Telefono = Input.Telefono,
            Direccion = Input.Direccion
        };

        try
        {
            await _usuarioService.CrearPerfilUsuarioAsync(
                user.Id,
                perfilRequest,
                HttpContext.RequestAborted);

            _logger.LogInformation("Perfil de usuario creado correctamente.");

            await _signInManager.SignInAsync(user, isPersistent: false);

            return LocalRedirect(returnUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el perfil del usuario. Se eliminará el usuario creado en AspNetUsers.");

            await _userManager.DeleteAsync(user);

            ModelState.AddModelError(
                string.Empty,
                "No fue posible crear el perfil del usuario. Verifica la información e intenta nuevamente.");

            return Page();
        }
    }
}
