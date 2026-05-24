using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.Interfaces.Services;
using ReservasFodun.Web.Extensions;

namespace ReservasFodun.Web.Controllers.Mvc;

[Authorize]
public class AdminAlojamientosController : Controller
{
    private readonly IAlojamientoService _alojamientoService;

    public AdminAlojamientosController(IAlojamientoService alojamientoService)
    {
        _alojamientoService = alojamientoService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var alojamientos = await _alojamientoService.ObtenerAlojamientosAdminAsync(cancellationToken);

        return View(alojamientos);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("El alojamiento seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        var alojamiento = await _alojamientoService.ObtenerAlojamientoAdminPorIdAsync(id, cancellationToken);

        if (alojamiento is null)
        {
            TempData.Warning("No se encontró el alojamiento solicitado.");
            return RedirectToAction(nameof(Index));
        }

        return View(alojamiento);
    }

    [HttpGet]
    public async Task<IActionResult> Crear(CancellationToken cancellationToken)
    {
        await CargarListasAsync(cancellationToken);

        var request = new AlojamientoFormularioRequest
        {
            Activo = true,
            NumeroHabitaciones = 1,
            CapacidadMaxima = 1
        };

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        AlojamientoFormularioRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(cancellationToken);
            TempData.Warning("Revisa los campos obligatorios antes de guardar el alojamiento.");
            return View(request);
        }

        try
        {
            var idAlojamiento = await _alojamientoService.CrearAlojamientoAsync(request, cancellationToken);

            TempData.Success("Alojamiento creado correctamente.");

            return RedirectToAction(nameof(Detalle), new { id = idAlojamiento });
        }
        catch (ArgumentException ex)
        {
            await CargarListasAsync(cancellationToken);
            ModelState.AddModelError(string.Empty, ex.Message);
            TempData.Warning(ex.Message);
            return View(request);
        }
        catch (Exception)
        {
            await CargarListasAsync(cancellationToken);
            TempData.Error("Ocurrió un error inesperado al crear el alojamiento.");
            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("El alojamiento seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        var alojamiento = await _alojamientoService.ObtenerAlojamientoAdminPorIdAsync(id, cancellationToken);

        if (alojamiento is null)
        {
            TempData.Warning("No se encontró el alojamiento solicitado.");
            return RedirectToAction(nameof(Index));
        }

        var request = new AlojamientoFormularioRequest
        {
            IdAlojamiento = alojamiento.IdAlojamiento,
            IdSede = alojamiento.IdSede,
            IdTipoAlojamiento = alojamiento.IdTipoAlojamiento,
            NumeroAlojamiento = alojamiento.NumeroAlojamiento,
            NombreAlojamiento = alojamiento.NombreAlojamiento,
            Descripcion = alojamiento.Descripcion,
            NumeroHabitaciones = alojamiento.NumeroHabitaciones,
            CapacidadMaxima = alojamiento.CapacidadMaxima,
            NumeroHabitacionesTarifa = alojamiento.NumeroHabitacionesTarifa,
            Activo = alojamiento.Activo
        };

        await CargarListasAsync(cancellationToken);

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        AlojamientoFormularioRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(cancellationToken);
            TempData.Warning("Revisa los campos obligatorios antes de actualizar el alojamiento.");
            return View(request);
        }

        try
        {
            var actualizado = await _alojamientoService.ActualizarAlojamientoAsync(request, cancellationToken);

            if (!actualizado)
            {
                TempData.Warning("No se encontró el alojamiento que intentas actualizar.");
                return RedirectToAction(nameof(Index));
            }

            TempData.Success("Alojamiento actualizado correctamente.");

            return RedirectToAction(nameof(Detalle), new { id = request.IdAlojamiento });
        }
        catch (ArgumentException ex)
        {
            await CargarListasAsync(cancellationToken);
            ModelState.AddModelError(string.Empty, ex.Message);
            TempData.Warning(ex.Message);
            return View(request);
        }
        catch (Exception)
        {
            await CargarListasAsync(cancellationToken);
            TempData.Error("Ocurrió un error inesperado al actualizar el alojamiento.");
            return View(request);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inactivar(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("El alojamiento seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var inactivado = await _alojamientoService.InactivarAlojamientoAsync(id, cancellationToken);

            if (!inactivado)
            {
                TempData.Warning("No se encontró el alojamiento que intentas inactivar.");
                return RedirectToAction(nameof(Index));
            }

            TempData.Success("Alojamiento inactivado correctamente.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al inactivar el alojamiento.");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activar(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("El alojamiento seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var activado = await _alojamientoService.ActivarAlojamientoAsync(id, cancellationToken);

            if (!activado)
            {
                TempData.Warning("No se encontró el alojamiento que intentas activar.");
                return RedirectToAction(nameof(Index));
            }

            TempData.Success("Alojamiento activado correctamente.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al activar el alojamiento.");
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task CargarListasAsync(CancellationToken cancellationToken)
    {
        var sedes = await _alojamientoService.ObtenerSedesActivasSelectAsync(cancellationToken);
        var tiposAlojamiento = await _alojamientoService.ObtenerTiposAlojamientoSelectAsync(cancellationToken);

        ViewBag.Sedes = new SelectList(sedes, "Id", "Nombre");
        ViewBag.TiposAlojamiento = new SelectList(tiposAlojamiento, "Id", "Nombre");
    }
}
