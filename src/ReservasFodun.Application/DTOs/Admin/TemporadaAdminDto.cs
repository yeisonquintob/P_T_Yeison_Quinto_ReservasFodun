namespace ReservasFodun.Application.DTOs.Admin;

public class TemporadaAdminDto
{
    public int IdTemporada { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public bool EsAlta { get; set; }
    public bool EsEspecial { get; set; }
    public bool Activo { get; set; }

    public int TotalTarifas { get; set; }
}
