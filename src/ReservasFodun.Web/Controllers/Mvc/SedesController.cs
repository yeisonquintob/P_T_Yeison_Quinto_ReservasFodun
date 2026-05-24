using Microsoft.AspNetCore.Mvc;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Web.Controllers.Mvc;

public class SedesController : Controller
{
    private readonly ISedeService _sedeService;

    public SedesController(ISedeService sedeService)
    {
        _sedeService = sedeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var sedes = await _sedeService.ObtenerSedesAsync(cancellationToken);
        return View(sedes);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData["Error"] = "La sede seleccionada no es válida.";
            return RedirectToAction(nameof(Index));
        }

        var sede = await _sedeService.ObtenerSedePorIdAsync(id, cancellationToken);

        if (sede is null)
        {
            TempData["Error"] = "No se encontró la sede solicitada.";
            return RedirectToAction(nameof(Index));
        }

        return View(sede);
    }
}
