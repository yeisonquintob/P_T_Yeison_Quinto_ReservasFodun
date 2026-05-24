namespace ReservasFodun.Application.DTOs.Admin;

public class SedeAdminDto
{
    public int IdSede { get; set; }
    public int IdTipoSede { get; set; }
    public int IdMunicipio { get; set; }

    public string NombreSede { get; set; } = string.Empty;
    public string? NombreCorto { get; set; }
    public string? Direccion { get; set; }
    public string? Descripcion { get; set; }

    public int CapacidadTotal { get; set; }
    public bool Activo { get; set; }

    public string? TipoSede { get; set; }
    public string? Municipio { get; set; }
    public string? Departamento { get; set; }
}
