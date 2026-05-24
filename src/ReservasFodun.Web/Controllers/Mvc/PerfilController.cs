using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasFodun.Application.DTOs.Usuarios;
using ReservasFodun.Application.Interfaces.Services;
using ReservasFodun.Web.Extensions;

namespace ReservasFodun.Web.Controllers.Mvc;

[Authorize]
public class PerfilController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public PerfilController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Editar));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(CancellationToken cancellationToken)
    {
        try
        {
            var idUsuario = ObtenerIdUsuario();

            var perfil = await _usuarioService.ObtenerPerfilPorUsuarioIdAsync(
                idUsuario,
                cancellationToken);

            if (perfil is null)
            {
                TempData.Warning("No se encontró información de perfil para editar.");
                return RedirectToAction("Index", "Home");
            }

            var request = new ActualizarPerfilUsuarioRequest
            {
                TipoDocumento = perfil.TipoDocumento,
                NumeroDocumento = perfil.NumeroDocumento,
                Nombres = perfil.Nombres,
                Apellidos = perfil.Apellidos,
                Telefono = perfil.Telefono,
                Direccion = perfil.Direccion
            };

            return View(request);
        }
        catch (UnauthorizedAccessException ex)
        {
            TempData.Error(ex.Message);
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al cargar la información del perfil.");
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        ActualizarPerfilUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData.Warning("Revisa los datos ingresados antes de guardar el perfil.");
            return View(request);
        }

        try
        {
            var idUsuario = ObtenerIdUsuario();

            var actualizado = await _usuarioService.ActualizarPerfilUsuarioAsync(
                idUsuario,
                request,
                cancellationToken);

            if (!actualizado)
            {
                TempData.Warning("No fue posible actualizar el perfil.");
                return View(request);
            }

            TempData.Success("Perfil actualizado correctamente.");
            return RedirectToAction(nameof(Editar));
        }
        catch (UnauthorizedAccessException ex)
        {
            TempData.Error(ex.Message);
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            TempData.Warning(ex.Message);

            return View(request);
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al actualizar el perfil.");
            return View(request);
        }
    }

    private string ObtenerIdUsuario()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("No se encontró un usuario autenticado.");
    }
}
