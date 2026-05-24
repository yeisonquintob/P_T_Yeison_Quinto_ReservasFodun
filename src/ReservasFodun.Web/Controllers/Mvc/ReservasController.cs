using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasFodun.Application.DTOs.Reservas;
using ReservasFodun.Application.Interfaces.Services;
using ReservasFodun.Web.Extensions;

namespace ReservasFodun.Web.Controllers.Mvc;

[Authorize]
public class ReservasController : Controller
{
    private readonly IReservaService _reservaService;
    private readonly ISedeService _sedeService;

    public ReservasController(
        IReservaService reservaService,
        ISedeService sedeService)
    {
        _reservaService = reservaService;
        _sedeService = sedeService;
    }

    [HttpGet]
    public async Task<IActionResult> Confirmar(CancellationToken cancellationToken)
    {
        ViewBag.Sedes = await _sedeService.ObtenerSedesAsync(cancellationToken);

        var request = new CrearReservaRequest
        {
            FechaLlegada = DateTime.Today.AddDays(1),
            FechaSalida = DateTime.Today.AddDays(2),
            NumeroPersonas = 1,
            NumeroHabitaciones = 1
        };

        TempData.Info("Verifica los datos antes de confirmar la reserva.");

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirmar(
        CrearReservaRequest request,
        CancellationToken cancellationToken)
    {
        ViewBag.Sedes = await _sedeService.ObtenerSedesAsync(cancellationToken);

        if (!ModelState.IsValid)
        {
            TempData.Warning("Revisa los campos del formulario antes de confirmar la reserva.");
            return View(request);
        }

        try
        {
            var idUsuario = ObtenerIdUsuario();

            var idReserva = await _reservaService.CrearReservaAsync(
                request,
                idUsuario,
                cancellationToken);

            TempData.Success("Reserva creada correctamente.");

            return RedirectToAction(nameof(Detalle), new { id = idReserva });
        }
        catch (UnauthorizedAccessException ex)
        {
            TempData.Error(ex.Message);
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            TempData.Warning(ex.Message);

            return View(request);
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al crear la reserva. Intenta nuevamente.");
            return View(request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> MisReservas(CancellationToken cancellationToken)
    {
        try
        {
            var idUsuario = ObtenerIdUsuario();

            var reservas = await _reservaService.ConsultarReservasUsuarioAsync(
                idUsuario,
                cancellationToken);

            if (!reservas.Any())
            {
                TempData.Info("Aún no tienes reservas registradas.");
            }

            return View(reservas);
        }
        catch (UnauthorizedAccessException ex)
        {
            TempData.Error(ex.Message);
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al consultar tus reservas.");
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData.Warning("La reserva seleccionada no es válida.");
            return RedirectToAction(nameof(MisReservas));
        }

        try
        {
            var idUsuario = ObtenerIdUsuario();

            var reserva = await _reservaService.ObtenerReservaPorIdAsync(
                id,
                idUsuario,
                cancellationToken);

            if (reserva is null)
            {
                TempData.Warning("No se encontró la reserva solicitada.");
                return RedirectToAction(nameof(MisReservas));
            }

            return View(reserva);
        }
        catch (UnauthorizedAccessException ex)
        {
            TempData.Error(ex.Message);
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al consultar el detalle de la reserva.");
            return RedirectToAction(nameof(MisReservas));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(
        CancelarReservaRequest request,
        CancellationToken cancellationToken)
    {
        if (request.IdReserva <= 0)
        {
            TempData.Warning("La reserva seleccionada no es válida.");
            return RedirectToAction(nameof(MisReservas));
        }

        try
        {
            var idUsuario = ObtenerIdUsuario();

            var cancelada = await _reservaService.CancelarReservaAsync(
                request,
                idUsuario,
                cancellationToken);

            if (!cancelada)
            {
                TempData.Warning("No se encontró la reserva o no se pudo cancelar.");
                return RedirectToAction(nameof(MisReservas));
            }

            TempData.Success("Reserva cancelada correctamente.");
            return RedirectToAction(nameof(MisReservas));
        }
        catch (UnauthorizedAccessException ex)
        {
            TempData.Error(ex.Message);
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        catch (ArgumentException ex)
        {
            TempData.Warning(ex.Message);
            return RedirectToAction(nameof(MisReservas));
        }
        catch (Exception)
        {
            TempData.Error("Ocurrió un error inesperado al cancelar la reserva.");
            return RedirectToAction(nameof(MisReservas));
        }
    }

    private string ObtenerIdUsuario()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("No se encontró un usuario autenticado.");
    }
}
