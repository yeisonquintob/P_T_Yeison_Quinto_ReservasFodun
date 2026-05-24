using Microsoft.AspNetCore.Mvc;
using ReservasFodun.Application.DTOs.Alojamientos;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Web.Controllers.Api;

[ApiController]
[Route("api/disponibilidad")]
public class DisponibilidadApiController : ControllerBase
{
    private readonly IDisponibilidadService _disponibilidadService;

    public DisponibilidadApiController(IDisponibilidadService disponibilidadService)
    {
        _disponibilidadService = disponibilidadService;
    }

    [HttpGet]
    public async Task<IActionResult> ConsultarDisponibilidad(
        [FromQuery] ConsultarDisponibilidadRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _disponibilidadService.ConsultarDisponibilidadAsync(
                request,
                cancellationToken);

            return Ok(new
            {
                mensaje = resultado.Any()
                    ? "Disponibilidad consultada correctamente."
                    : "No se encontraron alojamientos disponibles con los filtros enviados.",
                total = resultado.Count(),
                datos = resultado
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                mensaje = "Ocurrió un error inesperado al consultar la disponibilidad."
            });
        }
    }
}
