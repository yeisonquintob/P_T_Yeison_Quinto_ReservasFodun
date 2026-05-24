namespace ReservasFodun.Domain.Entities;

public class Departamento
{
    public int IdDepartamento { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
}
