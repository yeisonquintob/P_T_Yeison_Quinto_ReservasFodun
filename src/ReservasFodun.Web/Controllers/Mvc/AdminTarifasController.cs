using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.Interfaces.Services;
using ReservasFodun.Web.Extensions;

namespace ReservasFodun.Web.Controllers.Mvc;

[Authorize]
public class AdminTarifasController : Controller
{
    private readonly ITarifaService _tarifaService;

    public AdminTarifasController(ITarifaService tarifaService)
    {
        _tarifaService = tarifaService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var tarifas = await _tarifaService.ObtenerTarifasAdminAsync(cancellationToken);

        return View(tarifas);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("La tarifa seleccionada no es válida.");
            return RedirectToAction(nameof(Index));
        }

        var tarifa = await _tarifaService.ObtenerTarifaAdminPorIdAsync(id, cancellationToken);

        if (tarifa is null)
        {
            TempData.Warning("No se encontró la tarifa solicitada.");
            return RedirectToAction(nameof(Index));
        }

        return View(tarifa);
    }

    [HttpGet]
    public async Task<IActionResult> Crear(CancellationToken cancellationToken)
    {
        await CargarListasAsync(cancellationToken);

        var request = new TarifaFormularioRequest
        {
            Activo = true,
            PersonasIncluidas = 1
        };

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        TarifaFormularioRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(cancellationToken);
            TempData.Warning("Revisa los campos obligatorios antes de guardar la tarifa.");
            return View(request);
        }

        try
        {
            var id = await _tarifaService.CrearTarifaAsync(request, cancellationToken);

            TempData.Success("Tarifa creada correctamente.");

            return RedirectToAction(nameof(Detalle), new { id });
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
            TempData.Error("Ocurrió un error inesperado al crear la tarifa.");
            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("La tarifa seleccionada no es válida.");
            return RedirectToAction(nameof(Index));
        }

        var tarifa = await _tarifaService.ObtenerTarifaAdminPorIdAsync(id, cancellationToken);

        if (tarifa is null)
        {
            TempData.Warning("No se encontró la tarifa solicitada.");
            return RedirectToAction(nameof(Index));
        }

        var request = new TarifaFormularioRequest
        {
            IdTarifa = tarifa.IdTarifa,
            IdSede = tarifa.IdSede,
            IdAlojamiento = tarifa.IdAlojamiento,
            IdTemporada = tarifa.IdTemporada,
            NumeroHabitacionesTarifa = tarifa.NumeroHabitacionesTarifa,
            PersonasIncluidas = tarifa.PersonasIncluidas,
            TarifaBase = tarifa.TarifaBase,
            ValorPersonaAdicional = tarifa.ValorPersonaAdicional,
            EsTarifaEspecial = tarifa.EsTarifaEspecial,
            Descripcion = tarifa.Descripcion,
            Activo = tarifa.Activo
        };

        await CargarListasAsync(cancellationToken);

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        TarifaFormularioRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(cancellationToken);
            TempData.Warning("Revisa los campos obligatorios antes de actualizar la tarifa.");
            return View(request);
        }

        try
        {
            var actualizada = await _tarifaService.ActualizarTarifaAsync(request, cancellationToken);

            if (!actualizada)
            {
                TempData.Warning("No se encontró la tarifa que intentas actualizar.");
                return RedirectToAction(nameof(Index));
            }

            TempData.Success("Tarifa actualizada correctamente.");

            return RedirectToAction(nameof(Detalle), new { id = request.IdTarifa });
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
            TempData.Error("Ocurrió un error inesperado al actualizar la tarifa.");
            return View(request);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inactivar(int id, CancellationToken cancellationToken)
    {
        var inactivada = await _tarifaService.InactivarTarifaAsync(id, cancellationToken);

        if (inactivada)
            TempData.Success("Tarifa inactivada correctamente.");
        else
            TempData.Warning("No se encontró la tarifa que intentas inactivar.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activar(int id, CancellationToken cancellationToken)
    {
        var activada = await _tarifaService.ActivarTarifaAsync(id, cancellationToken);

        if (activada)
            TempData.Success("Tarifa activada correctamente.");
        else
            TempData.Warning("No se encontró la tarifa que intentas activar.");

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(CancellationToken cancellationToken)
    {
        var sedes = await _tarifaService.ObtenerSedesSelectAsync(cancellationToken);
        var alojamientos = await _tarifaService.ObtenerAlojamientosSelectAsync(cancellationToken);
        var temporadas = await _tarifaService.ObtenerTemporadasSelectAsync(cancellationToken);

        ViewBag.Sedes = new SelectList(sedes, "Id", "Nombre");
        ViewBag.Alojamientos = new SelectList(alojamientos, "Id", "Nombre");
        ViewBag.Temporadas = new SelectList(temporadas, "Id", "Nombre");
    }
}
