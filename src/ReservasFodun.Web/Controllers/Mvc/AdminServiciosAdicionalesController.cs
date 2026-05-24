using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.Interfaces.Services;
using ReservasFodun.Web.Extensions;

namespace ReservasFodun.Web.Controllers.Mvc;

[Authorize]
public class AdminServiciosAdicionalesController : Controller
{
    private readonly IServicioAdicionalService _servicioAdicionalService;

    public AdminServiciosAdicionalesController(IServicioAdicionalService servicioAdicionalService)
    {
        _servicioAdicionalService = servicioAdicionalService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var servicios = await _servicioAdicionalService.ObtenerServiciosAdicionalesAdminAsync(cancellationToken);

        return View(servicios);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("El servicio adicional seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        var servicio = await _servicioAdicionalService.ObtenerServicioAdicionalAdminPorIdAsync(id, cancellationToken);

        if (servicio is null)
        {
            TempData.Warning("No se encontró el servicio adicional solicitado.");
            return RedirectToAction(nameof(Index));
        }

        return View(servicio);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        var request = new ServicioAdicionalFormularioRequest
        {
            Activo = true
        };

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        ServicioAdicionalFormularioRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData.Warning("Revisa los campos obligatorios antes de guardar el servicio adicional.");
            return View(request);
        }

        try
        {
            var id = await _servicioAdicionalService.CrearServicioAdicionalAsync(request, cancellationToken);

            TempData.Success("Servicio adicional creado correctamente.");

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
            TempData.Error("Ocurrió un error inesperado al crear el servicio adicional.");
            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("El servicio adicional seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        var servicio = await _servicioAdicionalService.ObtenerServicioAdicionalAdminPorIdAsync(id, cancellationToken);

        if (servicio is null)
        {
            TempData.Warning("No se encontró el servicio adicional solicitado.");
            return RedirectToAction(nameof(Index));
        }

        var request = new ServicioAdicionalFormularioRequest
        {
            IdServicioAdicional = servicio.IdServicioAdicional,
            Nombre = servicio.Nombre,
            Descripcion = servicio.Descripcion,
            Valor = servicio.Valor,
            PorPersona = servicio.PorPersona,
            PorNoche = servicio.PorNoche,
            Activo = servicio.Activo
        };

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        ServicioAdicionalFormularioRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData.Warning("Revisa los campos obligatorios antes de actualizar el servicio adicional.");
            return View(request);
        }

        try
        {
            var actualizado = await _servicioAdicionalService.ActualizarServicioAdicionalAsync(request, cancellationToken);

            if (!actualizado)
            {
                TempData.Warning("No se encontró el servicio adicional que intentas actualizar.");
                return RedirectToAction(nameof(Index));
            }

            TempData.Success("Servicio adicional actualizado correctamente.");

            return RedirectToAction(nameof(Detalle), new { id = request.IdServicioAdicional });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            TempData.Warning(ex.Message);
            return View(request);
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al actualizar el servicio adicional.");
            return View(request);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inactivar(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("El servicio adicional seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        var inactivado = await _servicioAdicionalService.InactivarServicioAdicionalAsync(id, cancellationToken);

        if (inactivado)
            TempData.Success("Servicio adicional inactivado correctamente.");
        else
            TempData.Warning("No se encontró el servicio adicional que intentas inactivar.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activar(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("El servicio adicional seleccionado no es válido.");
            return RedirectToAction(nameof(Index));
        }

        var activado = await _servicioAdicionalService.ActivarServicioAdicionalAsync(id, cancellationToken);

        if (activado)
            TempData.Success("Servicio adicional activado correctamente.");
        else
            TempData.Warning("No se encontró el servicio adicional que intentas activar.");

        return RedirectToAction(nameof(Index));
    }
}
