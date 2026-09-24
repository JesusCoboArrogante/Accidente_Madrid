namespace Accidentes_Madrid.Models;

public class Accidentes
{
    public string numExpediente { get; set; }
    public DateOnly? fecha { get; set; }
    public int anio => fecha.Value.Year;
    public int mes => fecha.Value.Month;
    public int dia => fecha.Value.Day;
    public DayOfWeek diaSemana => fecha.Value.DayOfWeek;
    
    public TimeOnly hora { get; set; }
    public string? localidad { get; set; }
    public string numero { get; set; }
    public int? codDistricto  { get; set; }
    public string districto { get; set; }
    public string? tipoAccidente { get; set; }
    public string? estadoMeteorologico { get; set; }
    public string? tipoVehiculo { get; set; }
    public TipoPersona tipoPersona { get; set; }
    public string rangoEdad { get; set; }
    public Sexo sexo { get; set; }
    public int? codLesividad { get; set; }
    public string? lesividad {get; set; }
    public double? cooordenadaXUtm { get; set; }
    public double? coordenadaYUtm { get; set; }
    public bool? positivoAlcohol { get; set; }
    public bool? positivoDroga { get; set; }
}