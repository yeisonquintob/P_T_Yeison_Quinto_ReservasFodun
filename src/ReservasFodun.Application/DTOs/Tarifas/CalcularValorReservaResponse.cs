namespace ReservasFodun.Application.DTOs.Tarifas;

public class CalcularValorReservaResponse
{
    public int IdSede { get; set; }
    public string NombreSede { get; set; } = string.Empty;

    public int? IdAlojamiento { get; set; }
    public string? NombreAlojamiento { get; set; }

    public DateTime FechaLlegada { get; set; }
    public DateTime FechaSalida { get; set; }

    public int NumeroNoches { get; set; }
    public int NumeroPersonas { get; set; }
    public int NumeroHabitaciones { get; set; }

    public int DiasOrdinarios { get; set; }
    public int DiasEspeciales { get; set; }

    public decimal ValorTarifaOrdinaria { get; set; }
    public decimal ValorTarifaEspecial { get; set; }
    public decimal ValorPersonasAdicionales { get; set; }
    public decimal ValorLavanderia { get; set; }

    public decimal ValorSubtotal { get; set; }
    public decimal ValorTotal { get; set; }

    public string? DetalleCalculo { get; set; }
}
