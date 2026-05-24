namespace ReservasFodun.Domain.Entities;

public class Municipio
{
    public int IdMunicipio { get; set; }
    public int IdDepartamento { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public Departamento? Departamento { get; set; }
    public ICollection<Sede> Sedes { get; set; } = new List<Sede>();
}
