namespace ReservasFodun.Application.DTOs.Alojamientos;

public class SedeDto
{
    public int IdSede { get; set; }
    public string NombreSede { get; set; } = string.Empty;
    public string? NombreCorto { get; set; }
    public string? TipoSede { get; set; }
    public string? Municipio { get; set; }
    public string? Departamento { get; set; }
    public string? Direccion { get; set; }
    public string? Descripcion { get; set; }
    public int CapacidadTotal { get; set; }
}
