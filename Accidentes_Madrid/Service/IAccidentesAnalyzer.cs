using Accidentes_Madrid.Models;

namespace Accidentes_Madrid.Service;

public interface IAccidentesAnalyzer
{
    
    int TotalAccidentes();

    List<(string districto, int total)> TopCinco();

    Dictionary<string, int> AccidentesPorTipo();
    
    Dictionary<string, int> AccidentesPorMeteorologia();
    
    Dictionary<Sexo, int> PersonaPorSexo();
    
    Dictionary<string, int> PersonaPorNombre();

    int PositivoAlchol();
    
    int PositivoDroga();

    Dictionary<DayOfWeek, int> AccidentePorDiaSemana();

    Dictionary<int, int> AccidentePorMes();

    (int hora, int total)? HorasConMasAccidentes();

    List<(string lesividad, int total)> LesionesMasFrecuentes();

    (string tipoVehiculo, int total)? TipoVehiculoMasImplicado();

    int AccidentesConPeatones();

    (double porcentajeHombre, double porcentajeMujeres)? ProporcionHombreMujer();

    List<(string districto, int total)> DistrictoConMasPeatones();

    (int finDeSemana, int EntreSemana) FinSemanaEntreSemana();

    double MediaAccidentePorDia();
    
    int AccidentePorAlcoholDrogras();

    List<(string rangoEdad, int total)> RangoEdadPeatones();

    List<(string districto, int total)> DistrictoMasAlcohol();
    
    Dictionary<int, int> AccidentePorAnio();
    
    Dictionary<(int anio, int mes), int> AccidentePorAnioAlcohol();

    Dictionary<int, (string districto, int total)> DistrictoConMasAccidentesPorAnio();
    
    Dictionary<int, int> PositivoAlcoholAnio();
    
    Dictionary<int, (int finDeSemana, int entreSemana)> FinDeSemanaEntreSemana();
    
    Dictionary<int, (string levisidad, int total)> LesionesMasFreeecuentesPorAnio();
    
    Dictionary<int, int> PeatonesPorAnio();
    
    Dictionary<int, int> AccidentesPorCodigoDistrito();
    
    Dictionary<int, (int hora, int total)> HoraPicoPorAnio();
}