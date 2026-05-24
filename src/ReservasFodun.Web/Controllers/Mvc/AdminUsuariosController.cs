using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasFodun.Web.Extensions;
using ReservasFodun.Web.ViewModels.Admin;

namespace ReservasFodun.Web.Controllers.Mvc;

[Authorize]
public class AdminUsuariosController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public AdminUsuariosController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var usuarios = await _userManager.Users
            .AsNoTracking()
            .OrderBy(x => x.Email)
            .Select(x => new AdminUsuarioListaViewModel
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                EmailConfirmed = x.EmailConfirmed,
                LockoutEnabled = x.LockoutEnabled,
                LockoutEnd = x.LockoutEnd
            })
            .ToListAsync(cancellationToken);

        return View(usuarios);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            TempData.Warning("El usuario seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario is null)
        {
            TempData.Warning("No se encontró el usuario solicitado.");
            return RedirectToAction(nameof(Index));
        }

        var model = new AdminUsuarioDetalleViewModel
        {
            Id = usuario.Id,
            UserName = usuario.UserName,
            Email = usuario.Email,
            PhoneNumber = usuario.PhoneNumber,
            EmailConfirmed = usuario.EmailConfirmed,
            PhoneNumberConfirmed = usuario.PhoneNumberConfirmed,
            TwoFactorEnabled = usuario.TwoFactorEnabled,
            LockoutEnabled = usuario.LockoutEnabled,
            LockoutEnd = usuario.LockoutEnd,
            AccessFailedCount = usuario.AccessFailedCount,
            Roles = new List<string>()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Editar(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            TempData.Warning("El usuario seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario is null)
        {
            TempData.Warning("No se encontró el usuario solicitado.");
            return RedirectToAction(nameof(Index));
        }

        var request = new AdminUsuarioEditarRequest
        {
            Id = usuario.Id,
            UserName = usuario.UserName ?? string.Empty,
            Email = usuario.Email ?? string.Empty,
            PhoneNumber = usuario.PhoneNumber,
            EmailConfirmed = usuario.EmailConfirmed,
            LockoutEnabled = usuario.LockoutEnabled
        };

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(AdminUsuarioEditarRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData.Warning("Revisa los campos obligatorios antes de actualizar el usuario.");
            return View(request);
        }

        var usuario = await _userManager.FindByIdAsync(request.Id);

        if (usuario is null)
        {
            TempData.Warning("No se encontró el usuario que intentas actualizar.");
            return RedirectToAction(nameof(Index));
        }

        usuario.UserName = request.UserName.Trim();
        usuario.Email = request.Email.Trim();
        usuario.NormalizedUserName = request.UserName.Trim().ToUpperInvariant();
        usuario.NormalizedEmail = request.Email.Trim().ToUpperInvariant();
        usuario.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? null
            : request.PhoneNumber.Trim();
        usuario.EmailConfirmed = request.EmailConfirmed;
        usuario.LockoutEnabled = request.LockoutEnabled;

        var resultado = await _userManager.UpdateAsync(usuario);

        if (!resultado.Succeeded)
        {
            foreach (var error in resultado.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            TempData.Error("No fue posible actualizar el usuario.");
            return View(request);
        }

        TempData.Success("Usuario actualizado correctamente.");
        return RedirectToAction(nameof(Detalle), new { id = usuario.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarCorreo(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario is null)
        {
            TempData.Warning("No se encontró el usuario seleccionado.");
            return RedirectToAction(nameof(Index));
        }

        usuario.EmailConfirmed = true;

        var resultado = await _userManager.UpdateAsync(usuario);

        if (!resultado.Succeeded)
        {
            TempData.Error("No fue posible confirmar el correo del usuario.");
            return RedirectToAction(nameof(Detalle), new { id });
        }

        TempData.Success("Correo confirmado correctamente.");
        return RedirectToAction(nameof(Detalle), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Bloquear(string id)
    {
        var usuarioActualId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id == usuarioActualId)
        {
            TempData.Warning("No puedes bloquear tu propio usuario.");
            return RedirectToAction(nameof(Index));
        }

        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario is null)
        {
            TempData.Warning("No se encontró el usuario seleccionado.");
            return RedirectToAction(nameof(Index));
        }

        await _userManager.SetLockoutEnabledAsync(usuario, true);
        await _userManager.SetLockoutEndDateAsync(usuario, DateTimeOffset.UtcNow.AddYears(100));

        TempData.Success("Usuario bloqueado correctamente.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desbloquear(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario is null)
        {
            TempData.Warning("No se encontró el usuario seleccionado.");
            return RedirectToAction(nameof(Index));
        }

        await _userManager.SetLockoutEndDateAsync(usuario, null);
        await _userManager.ResetAccessFailedCountAsync(usuario);

        TempData.Success("Usuario desbloqueado correctamente.");
        return RedirectToAction(nameof(Index));
    }
}
