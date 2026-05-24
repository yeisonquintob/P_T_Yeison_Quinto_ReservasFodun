using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasFodun.Application.DTOs.Reservas;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Web.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/reservas")]
public class ReservasApiController : ControllerBase
{
    private readonly IReservaService _reservaService;

    public ReservasApiController(IReservaService reservaService)
    {
        _reservaService = reservaService;
    }

    [HttpPost]
    public async Task<IActionResult> CrearReserva(
        [FromBody] CrearReservaRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var idUsuario = ObtenerIdUsuario();

            var idReserva = await _reservaService.CrearReservaAsync(
                request,
                idUsuario,
                cancellationToken);

            return Ok(new
            {
                mensaje = "Reserva creada correctamente.",
                idReserva
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                mensaje = "Ocurrió un error inesperado al crear la reserva."
            });
        }
    }

    [HttpGet("{idReserva:int}")]
    public async Task<IActionResult> ObtenerReservaPorId(
        int idReserva,
        CancellationToken cancellationToken)
    {
        try
        {
            if (idReserva <= 0)
                return BadRequest(new { mensaje = "El identificador de la reserva no es válido." });

            var idUsuario = ObtenerIdUsuario();

            var reserva = await _reservaService.ObtenerReservaPorIdAsync(
                idReserva,
                idUsuario,
                cancellationToken);

            if (reserva is null)
                return NotFound(new { mensaje = "No se encontró la reserva solicitada." });

            return Ok(new
            {
                mensaje = "Reserva consultada correctamente.",
                datos = reserva
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                mensaje = "Ocurrió un error inesperado al consultar la reserva."
            });
        }
    }

    [HttpGet("mis-reservas")]
    public async Task<IActionResult> ConsultarMisReservas(CancellationToken cancellationToken)
    {
        try
        {
            var idUsuario = ObtenerIdUsuario();

            var reservas = await _reservaService.ConsultarReservasUsuarioAsync(
                idUsuario,
                cancellationToken);

            return Ok(new
            {
                mensaje = reservas.Any()
                    ? "Reservas consultadas correctamente."
                    : "No tienes reservas registradas.",
                total = reservas.Count(),
                datos = reservas
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                mensaje = "Ocurrió un error inesperado al consultar tus reservas."
            });
        }
    }

    [HttpPut("{idReserva:int}/cancelar")]
    public async Task<IActionResult> CancelarReserva(
        int idReserva,
        [FromBody] CancelarReservaRequest? request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (idReserva <= 0)
                return BadRequest(new { mensaje = "El identificador de la reserva no es válido." });

            var idUsuario = ObtenerIdUsuario();

            request ??= new CancelarReservaRequest();
            request.IdReserva = idReserva;

            var cancelada = await _reservaService.CancelarReservaAsync(
                request,
                idUsuario,
                cancellationToken);

            if (!cancelada)
                return NotFound(new { mensaje = "No se encontró la reserva o no se pudo cancelar." });

            return Ok(new { mensaje = "Reserva cancelada correctamente." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                mensaje = "Ocurrió un error inesperado al cancelar la reserva."
            });
        }
    }

    private string ObtenerIdUsuario()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("No se encontró un usuario autenticado.");
    }
}
