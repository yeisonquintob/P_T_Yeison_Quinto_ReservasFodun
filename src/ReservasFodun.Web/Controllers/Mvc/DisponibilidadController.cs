using Microsoft.AspNetCore.Mvc;
using ReservasFodun.Application.DTOs.Alojamientos;
using ReservasFodun.Application.Interfaces.Services;
using ReservasFodun.Web.Extensions;

namespace ReservasFodun.Web.Controllers.Mvc;

public class DisponibilidadController : Controller
{
    private readonly IDisponibilidadService _disponibilidadService;
    private readonly ISedeService _sedeService;

    public DisponibilidadController(
        IDisponibilidadService disponibilidadService,
        ISedeService sedeService)
    {
        _disponibilidadService = disponibilidadService;
        _sedeService = sedeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewBag.Sedes = await _sedeService.ObtenerSedesAsync(cancellationToken);

        var request = new ConsultarDisponibilidadRequest
        {
            FechaLlegada = DateTime.Today.AddDays(1),
            FechaSalida = DateTime.Today.AddDays(2)
        };

        TempData.Info("Selecciona las fechas, la sede y el número de personas para consultar disponibilidad.");

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resultados(
        ConsultarDisponibilidadRequest request,
        CancellationToken cancellationToken)
    {
        ViewBag.Sedes = await _sedeService.ObtenerSedesAsync(cancellationToken);

        try
        {
            var resultados = await _disponibilidadService.ConsultarDisponibilidadAsync(
                request,
                cancellationToken);

            ViewBag.Filtros = request;

            if (!resultados.Any())
            {
                TempData.Warning("No se encontraron alojamientos disponibles con los filtros seleccionados.");
            }
            else
            {
                TempData.Success("Consulta de disponibilidad realizada correctamente.");
            }

            return View(resultados);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            TempData.Warning(ex.Message);

            return View("Index", request);
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al consultar la disponibilidad. Intenta nuevamente.");

            return View("Index", request);
        }
    }
}
