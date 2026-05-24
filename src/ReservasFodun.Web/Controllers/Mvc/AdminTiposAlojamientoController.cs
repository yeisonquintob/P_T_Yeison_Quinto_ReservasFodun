using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.Interfaces.Services;
using ReservasFodun.Web.Extensions;

namespace ReservasFodun.Web.Controllers.Mvc;

[Authorize]
public class AdminTiposAlojamientoController : Controller
{
    private readonly ITipoAlojamientoService _tipoAlojamientoService;

    public AdminTiposAlojamientoController(ITipoAlojamientoService tipoAlojamientoService)
    {
        _tipoAlojamientoService = tipoAlojamientoService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var tipos = await _tipoAlojamientoService.ObtenerTiposAlojamientoAdminAsync(cancellationToken);

        return View(tipos);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("El tipo de alojamiento seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        var tipo = await _tipoAlojamientoService.ObtenerTipoAlojamientoAdminPorIdAsync(id, cancellationToken);

        if (tipo is null)
        {
            TempData.Warning("No se encontró el tipo de alojamiento solicitado.");
            return RedirectToAction(nameof(Index));
        }

        return View(tipo);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        var request = new TipoAlojamientoFormularioRequest
        {
            Activo = true
        };

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        TipoAlojamientoFormularioRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData.Warning("Revisa los campos obligatorios antes de guardar el tipo de alojamiento.");
            return View(request);
        }

        try
        {
            var id = await _tipoAlojamientoService.CrearTipoAlojamientoAsync(request, cancellationToken);

            TempData.Success("Tipo de alojamiento creado correctamente.");

            return RedirectToAction(nameof(Detalle), new { id });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            TempData.Warning(ex.Message);
            return View(request);
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al crear el tipo de alojamiento.");
            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("El tipo de alojamiento seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        var tipo = await _tipoAlojamientoService.ObtenerTipoAlojamientoAdminPorIdAsync(id, cancellationToken);

        if (tipo is null)
        {
            TempData.Warning("No se encontró el tipo de alojamiento solicitado.");
            return RedirectToAction(nameof(Index));
        }

        var request = new TipoAlojamientoFormularioRequest
        {
            IdTipoAlojamiento = tipo.IdTipoAlojamiento,
            Nombre = tipo.Nombre,
            Descripcion = tipo.Descripcion,
            Activo = tipo.Activo
        };

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        TipoAlojamientoFormularioRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData.Warning("Revisa los campos obligatorios antes de actualizar el tipo de alojamiento.");
            return View(request);
        }

        try
        {
            var actualizado = await _tipoAlojamientoService.ActualizarTipoAlojamientoAsync(request, cancellationToken);

            if (!actualizado)
            {
                TempData.Warning("No se encontró el tipo de alojamiento que intentas actualizar.");
                return RedirectToAction(nameof(Index));
            }

            TempData.Success("Tipo de alojamiento actualizado correctamente.");

            return RedirectToAction(nameof(Detalle), new { id = request.IdTipoAlojamiento });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            TempData.Warning(ex.Message);
            return View(request);
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al actualizar el tipo de alojamiento.");
            return View(request);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inactivar(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("El tipo de alojamiento seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        var inactivado = await _tipoAlojamientoService.InactivarTipoAlojamientoAsync(id, cancellationToken);

        TempData.Success(inactivado
            ? "Tipo de alojamiento inactivado correctamente."
            : "No se encontró el tipo de alojamiento que intentas inactivar.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activar(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("El tipo de alojamiento seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        var activado = await _tipoAlojamientoService.ActivarTipoAlojamientoAsync(id, cancellationToken);

        TempData.Success(activado
            ? "Tipo de alojamiento activado correctamente."
            : "No se encontró el tipo de alojamiento que intentas activar.");

        return RedirectToAction(nameof(Index));
    }
}
