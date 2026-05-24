using Microsoft.AspNetCore.Mvc;
using ReservasFodun.Application.DTOs.Tarifas;
using ReservasFodun.Application.Interfaces.Services;
using ReservasFodun.Web.Extensions;

namespace ReservasFodun.Web.Controllers.Mvc;

public class TarifasController : Controller
{
    private readonly ITarifaService _tarifaService;
    private readonly ISedeService _sedeService;

    public TarifasController(
        ITarifaService tarifaService,
        ISedeService sedeService)
    {
        _tarifaService = tarifaService;
        _sedeService = sedeService;
    }

    [HttpGet]
    public async Task<IActionResult> Consultar(CancellationToken cancellationToken)
    {
        ViewBag.Sedes = await _sedeService.ObtenerSedesAsync(cancellationToken);

        var request = new ConsultarTarifaRequest
        {
            NumeroPersonas = 1,
            NumeroHabitaciones = 1
        };

        TempData.Info("Consulta las tarifas disponibles según sede, alojamiento, personas y habitaciones.");

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Consultar(
        ConsultarTarifaRequest request,
        CancellationToken cancellationToken)
    {
        ViewBag.Sedes = await _sedeService.ObtenerSedesAsync(cancellationToken);

        if (!ModelState.IsValid)
        {
            TempData.Warning("Revisa los filtros ingresados antes de consultar tarifas.");
            return View(request);
        }

        try
        {
            var tarifas = await _tarifaService.ConsultarTarifasAsync(
                request,
                cancellationToken);

            var listaTarifas = tarifas.ToList();

            ViewBag.Filtros = request;

            if (!listaTarifas.Any())
            {
                TempData.Warning("No se encontraron tarifas con los filtros seleccionados.");
            }
            else
            {
                TempData.Success("Tarifas consultadas correctamente.");
            }

            return View("Resultados", listaTarifas);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            TempData.Warning(ex.Message);

            return View(request);
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al consultar las tarifas.");
            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Calcular(CancellationToken cancellationToken)
    {
        ViewBag.Sedes = await _sedeService.ObtenerSedesAsync(cancellationToken);

        var request = new CalcularValorReservaRequest
        {
            FechaLlegada = DateTime.Today.AddDays(1),
            FechaSalida = DateTime.Today.AddDays(2),
            NumeroPersonas = 1,
            NumeroHabitaciones = 1
        };

        TempData.Info("Ingresa los datos de la reserva para calcular el valor aproximado.");

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Calcular(
        CalcularValorReservaRequest request,
        CancellationToken cancellationToken)
    {
        ViewBag.Sedes = await _sedeService.ObtenerSedesAsync(cancellationToken);

        if (!ModelState.IsValid)
        {
            TempData.Warning("Revisa los datos ingresados antes de calcular la reserva.");
            return View(request);
        }

        try
        {
            var resultado = await _tarifaService.CalcularValorReservaAsync(
                request,
                cancellationToken);

            if (resultado is null)
            {
                const string mensaje = "No fue posible calcular la tarifa con los datos enviados.";

                ModelState.AddModelError(string.Empty, mensaje);
                TempData.Warning(mensaje);

                return View(request);
            }

            TempData.Success("Valor de reserva calculado correctamente.");

            return View("Resultado", resultado);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            TempData.Warning(ex.Message);

            return View(request);
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al calcular el valor de la reserva.");
            return View(request);
        }
    }
}
