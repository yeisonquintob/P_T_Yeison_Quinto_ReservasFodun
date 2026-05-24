using Microsoft.AspNetCore.Mvc;
using ReservasFodun.Application.DTOs.Tarifas;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Web.Controllers.Api;

[ApiController]
[Route("api/tarifas")]
public class TarifasApiController : ControllerBase
{
    private readonly ITarifaService _tarifaService;

    public TarifasApiController(ITarifaService tarifaService)
    {
        _tarifaService = tarifaService;
    }

    [HttpGet]
    public async Task<IActionResult> ConsultarTarifas(
        [FromQuery] ConsultarTarifaRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var tarifas = await _tarifaService.ConsultarTarifasAsync(
                request,
                cancellationToken);

            return Ok(new
            {
                mensaje = tarifas.Any()
                    ? "Tarifas consultadas correctamente."
                    : "No se encontraron tarifas con los filtros enviados.",
                total = tarifas.Count(),
                datos = tarifas
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
                mensaje = "Ocurrió un error inesperado al consultar las tarifas."
            });
        }
    }

    [HttpPost("calcular")]
    public async Task<IActionResult> CalcularValorReserva(
        [FromBody] CalcularValorReservaRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _tarifaService.CalcularValorReservaAsync(
                request,
                cancellationToken);

            if (resultado is null)
            {
                return NotFound(new
                {
                    mensaje = "No fue posible calcular la tarifa con los datos enviados."
                });
            }

            return Ok(new
            {
                mensaje = "Valor de reserva calculado correctamente.",
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
                mensaje = "Ocurrió un error inesperado al calcular el valor de la reserva."
            });
        }
    }
}
