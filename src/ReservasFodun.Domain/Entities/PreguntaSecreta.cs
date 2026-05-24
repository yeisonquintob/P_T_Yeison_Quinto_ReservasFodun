namespace ReservasFodun.Domain.Entities;

public class PreguntaSecreta
{
    public int IdPreguntaSecreta { get; set; }
    public string Pregunta { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
