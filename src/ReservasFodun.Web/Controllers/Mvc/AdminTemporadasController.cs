using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasFodun.Application.DTOs.Admin;
using ReservasFodun.Application.Interfaces.Services;
using ReservasFodun.Web.Extensions;

namespace ReservasFodun.Web.Controllers.Mvc;

[Authorize]
public class AdminTemporadasController : Controller
{
    private readonly ITemporadaService _temporadaService;

    public AdminTemporadasController(ITemporadaService temporadaService)
    {
        _temporadaService = temporadaService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var temporadas = await _temporadaService.ObtenerTemporadasAdminAsync(cancellationToken);

        return View(temporadas);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("La temporada seleccionada no es válida.");
            return RedirectToAction(nameof(Index));
        }

        var temporada = await _temporadaService.ObtenerTemporadaAdminPorIdAsync(id, cancellationToken);

        if (temporada is null)
        {
            TempData.Warning("No se encontró la temporada solicitada.");
            return RedirectToAction(nameof(Index));
        }

        return View(temporada);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        var request = new TemporadaFormularioRequest
        {
            Activo = true
        };

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        TemporadaFormularioRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData.Warning("Revisa los campos obligatorios antes de guardar la temporada.");
            return View(request);
        }

        try
        {
            var id = await _temporadaService.CrearTemporadaAsync(request, cancellationToken);

            TempData.Success("Temporada creada correctamente.");

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
            TempData.Error("Ocurrió un error inesperado al crear la temporada.");
            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("La temporada seleccionada no es válida.");
            return RedirectToAction(nameof(Index));
        }

        var temporada = await _temporadaService.ObtenerTemporadaAdminPorIdAsync(id, cancellationToken);

        if (temporada is null)
        {
            TempData.Warning("No se encontró la temporada solicitada.");
            return RedirectToAction(nameof(Index));
        }

        var request = new TemporadaFormularioRequest
        {
            IdTemporada = temporada.IdTemporada,
            Nombre = temporada.Nombre,
            Descripcion = temporada.Descripcion,
            EsAlta = temporada.EsAlta,
            EsEspecial = temporada.EsEspecial,
            Activo = temporada.Activo
        };

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        TemporadaFormularioRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData.Warning("Revisa los campos obligatorios antes de actualizar la temporada.");
            return View(request);
        }

        try
        {
            var actualizada = await _temporadaService.ActualizarTemporadaAsync(request, cancellationToken);

            if (!actualizada)
            {
                TempData.Warning("No se encontró la temporada que intentas actualizar.");
                return RedirectToAction(nameof(Index));
            }

            TempData.Success("Temporada actualizada correctamente.");

            return RedirectToAction(nameof(Detalle), new { id = request.IdTemporada });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            TempData.Warning(ex.Message);
            return View(request);
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al actualizar la temporada.");
            return View(request);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inactivar(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("La temporada seleccionada no es válida.");
            return RedirectToAction(nameof(Index));
        }

        var inactivada = await _temporadaService.InactivarTemporadaAsync(id, cancellationToken);

        if (inactivada)
            TempData.Success("Temporada inactivada correctamente.");
        else
            TempData.Warning("No se encontró la temporada que intentas inactivar.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activar(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("La temporada seleccionada no es válida.");
            return RedirectToAction(nameof(Index));
        }

        var activada = await _temporadaService.ActivarTemporadaAsync(id, cancellationToken);

        if (activada)
            TempData.Success("Temporada activada correctamente.");
        else
            TempData.Warning("No se encontró la temporada que intentas activar.");

        return RedirectToAction(nameof(Index));
    }
}
