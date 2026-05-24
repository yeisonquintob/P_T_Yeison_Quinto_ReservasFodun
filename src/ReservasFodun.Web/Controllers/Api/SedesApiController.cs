using Microsoft.AspNetCore.Mvc;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Web.Controllers.Api;

[ApiController]
[Route("api/sedes")]
public class SedesApiController : ControllerBase
{
    private readonly ISedeService _sedeService;

    public SedesApiController(ISedeService sedeService)
    {
        _sedeService = sedeService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerSedes(CancellationToken cancellationToken)
    {
        try
        {
            var sedes = await _sedeService.ObtenerSedesAsync(cancellationToken);

            return Ok(new
            {
                mensaje = "Sedes consultadas correctamente.",
                total = sedes.Count(),
                datos = sedes
            });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                mensaje = "Ocurrió un error inesperado al consultar las sedes."
            });
        }
    }

    [HttpGet("{idSede:int}")]
    public async Task<IActionResult> ObtenerSedePorId(
        int idSede,
        CancellationToken cancellationToken)
    {
        try
        {
            if (idSede <= 0)
                return BadRequest(new { mensaje = "El identificador de la sede no es válido." });

            var sede = await _sedeService.ObtenerSedePorIdAsync(idSede, cancellationToken);

            if (sede is null)
                return NotFound(new { mensaje = "No se encontró la sede solicitada." });

            return Ok(new
            {
                mensaje = "Sede consultada correctamente.",
                datos = sede
            });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                mensaje = "Ocurrió un error inesperado al consultar la sede."
            });
        }
    }
}
