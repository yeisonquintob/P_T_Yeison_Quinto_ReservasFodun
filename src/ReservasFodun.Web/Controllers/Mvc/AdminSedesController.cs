using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.Interfaces.Services;
using ReservasFodun.Web.Extensions;

namespace ReservasFodun.Web.Controllers.Mvc;

[Authorize]
public class AdminSedesController : Controller
{
    private readonly ISedeService _sedeService;

    public AdminSedesController(ISedeService sedeService)
    {
        _sedeService = sedeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var sedes = await _sedeService.ObtenerSedesAdminAsync(cancellationToken);

        return View(sedes);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("La sede seleccionada no es válida.");
            return RedirectToAction(nameof(Index));
        }

        var sede = await _sedeService.ObtenerSedeAdminPorIdAsync(id, cancellationToken);

        if (sede is null)
        {
            TempData.Warning("No se encontró la sede solicitada.");
            return RedirectToAction(nameof(Index));
        }

        return View(sede);
    }

    [HttpGet]
    public async Task<IActionResult> Crear(CancellationToken cancellationToken)
    {
        await CargarListasAsync(cancellationToken);

        var request = new SedeFormularioRequest
        {
            Activo = true,
            CapacidadTotal = 1
        };

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        SedeFormularioRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(cancellationToken);
            TempData.Warning("Revisa los campos obligatorios antes de guardar la sede.");
            return View(request);
        }

        try
        {
            var idSede = await _sedeService.CrearSedeAsync(request, cancellationToken);

            TempData.Success("Sede creada correctamente.");

            return RedirectToAction(nameof(Detalle), new { id = idSede });
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
            TempData.Error("Ocurrió un error inesperado al crear la sede.");
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
            TempData.Warning("La sede seleccionada no es válida.");
            return RedirectToAction(nameof(Index));
        }

        var sede = await _sedeService.ObtenerSedeAdminPorIdAsync(id, cancellationToken);

        if (sede is null)
        {
            TempData.Warning("No se encontró la sede solicitada.");
            return RedirectToAction(nameof(Index));
        }

        var request = new SedeFormularioRequest
        {
            IdSede = sede.IdSede,
            IdTipoSede = sede.IdTipoSede,
            IdMunicipio = sede.IdMunicipio,
            NombreSede = sede.NombreSede,
            NombreCorto = sede.NombreCorto,
            Direccion = sede.Direccion,
            Descripcion = sede.Descripcion,
            CapacidadTotal = sede.CapacidadTotal,
            Activo = sede.Activo
        };

        await CargarListasAsync(cancellationToken);

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        SedeFormularioRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(cancellationToken);
            TempData.Warning("Revisa los campos obligatorios antes de actualizar la sede.");
            return View(request);
        }

        try
        {
            var actualizada = await _sedeService.ActualizarSedeAsync(request, cancellationToken);

            if (!actualizada)
            {
                TempData.Warning("No se encontró la sede que intentas actualizar.");
                return RedirectToAction(nameof(Index));
            }

            TempData.Success("Sede actualizada correctamente.");

            return RedirectToAction(nameof(Detalle), new { id = request.IdSede });
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
            TempData.Error("Ocurrió un error inesperado al actualizar la sede.");
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
            TempData.Warning("La sede seleccionada no es válida.");
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var inactivada = await _sedeService.InactivarSedeAsync(id, cancellationToken);

            if (!inactivada)
            {
                TempData.Warning("No se encontró la sede que intentas inactivar.");
                return RedirectToAction(nameof(Index));
            }

            TempData.Success("Sede inactivada correctamente.");
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            TempData.Warning(ex.Message);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al inactivar la sede.");
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task CargarListasAsync(CancellationToken cancellationToken)
    {
        var tiposSede = await _sedeService.ObtenerTiposSedeAsync(cancellationToken);
        var municipios = await _sedeService.ObtenerMunicipiosAsync(cancellationToken);

        ViewBag.TiposSede = new SelectList(tiposSede, "Id", "Nombre");
        ViewBag.Municipios = new SelectList(municipios, "Id", "Nombre");
    }
}
