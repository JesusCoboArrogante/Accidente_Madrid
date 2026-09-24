using System;
using System.Collections;
using System.Diagnostics;
using Accidentes_Madrid.Repository;
using Accidentes_Madrid.Service;


var repositorio = new AccidenteRepository();
var cronometroCarga = Stopwatch.StartNew();

string carpetaData = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "data")
);

var accidentes = await repositorio.CargarAsync(
    Path.Combine(carpetaData, "300228-1-accidentes-trafico-detalle-csv.csv"),
    Path.Combine(carpetaData, "300228-2-accidentes-trafico-detalle-csv.csv"),
    Path.Combine(carpetaData, "300228-34-accidentes-trafico-detalle.csv")
);
cronometroCarga.Stop();

Console.WriteLine($"Registros cargados: {accidentes.Count}");
Console.WriteLine(
    $"Tiempo de carga: {cronometroCarga.Elapsed.TotalMilliseconds:F3} ms"
);


var linq = new AccidentesLinqAnalyzer(accidentes);
var dataframe = new AccidentesDataFrameAnalyzer(accidentes);


Consultar("1. Total de accidentes", servicio =>
    Console.WriteLine(servicio.TotalAccidentes()));


Consultar("2. Top cinco distritos", servicio =>
    Mostrar(servicio.TopCinco()));


Consultar("3. Accidentes por tipo", servicio =>
    Mostrar(servicio.AccidentesPorTipo()));


Consultar("4. Accidentes por meteorología", servicio =>
    Mostrar(servicio.AccidentesPorMeteorologia()));


Consultar("5. Personas por sexo", servicio =>
    Mostrar(servicio.PersonaPorSexo()));


Consultar("6. Personas por rango de edad", servicio =>
    Mostrar(servicio.PersonaPorNombre()));


Consultar("7. Positivos en alcohol", servicio =>
    Console.WriteLine(servicio.PositivoAlchol()));

Consultar("8. Positivos en drogas", servicio =>
    Console.WriteLine(servicio.PositivoDroga()));

Consultar("9. Accidentes por día de la semana", servicio =>
    Mostrar(servicio.AccidentePorDiaSemana()));


Consultar("10. Accidentes por mes", servicio =>
    Mostrar(servicio.AccidentePorMes()));


Consultar("11. Hora con más accidentes", servicio =>
    Console.WriteLine(servicio.HorasConMasAccidentes()));


Consultar("12. Lesiones más frecuentes", servicio =>
    Mostrar(servicio.LesionesMasFrecuentes()));


Consultar("13. Tipo de vehículo más implicado", servicio =>
    Console.WriteLine(servicio.TipoVehiculoMasImplicado()));


Consultar("14. Accidentes con peatones", servicio =>
    Console.WriteLine(servicio.AccidentesConPeatones()));


Consultar("15. Proporción hombre/mujer", servicio =>
    Console.WriteLine(servicio.ProporcionHombreMujer()));


Consultar("16. Distritos con más peatones", servicio =>
    Mostrar(servicio.DistrictoConMasPeatones()));


Consultar("17. Fin de semana frente a entre semana", servicio =>
    Console.WriteLine(servicio.FinSemanaEntreSemana()));


Consultar("18. Media de accidentes por día", servicio =>
    Console.WriteLine(servicio.MediaAccidentePorDia()));


Consultar("19. Accidentes con alcohol y drogas", servicio =>
    Console.WriteLine(servicio.AccidentePorAlcoholDrogras()));


Consultar("20. Rangos de edad de peatones", servicio =>
    Mostrar(servicio.RangoEdadPeatones()));


Consultar("21. Distritos con más positivos en alcohol", servicio =>
    Mostrar(servicio.DistrictoMasAlcohol()));


Consultar("22. Accidentes por código de distrito", servicio =>
    Mostrar(servicio.AccidentesPorCodigoDistrito()));


Consultar("23. Accidentes por año", servicio =>
    Mostrar(servicio.AccidentePorAnio()));


Consultar("24. Evolución mensual por año", servicio =>
    Mostrar(servicio.AccidentePorAnioAlcohol()));


Consultar("25. Distrito con más accidentes por año", servicio =>
    Mostrar(servicio.DistrictoConMasAccidentesPorAnio()));


Consultar("26. Positivos en alcohol por año", servicio =>
    Mostrar(servicio.PositivoAlcoholAnio()));


Consultar("27. Fin de semana frente a entre semana por año", servicio =>
    Mostrar(servicio.FinDeSemanaEntreSemana()));


Consultar("28. Hora pico por año", servicio =>
    Mostrar(servicio.HoraPicoPorAnio()));


Consultar("29. Lesión más frecuente por año", servicio =>
    Mostrar(servicio.LesionesMasFreeecuentesPorAnio()));


Consultar("30. Peatones por año", servicio =>
    Mostrar(servicio.PeatonesPorAnio()));



void Consultar(string nombre, Action<IAccidentesAnalyzer> consulta)
{
    Console.WriteLine($"\n--- {nombre} ---");

    var cronometro = new Stopwatch();

  
    Console.WriteLine("LINQ:");

    cronometro.Start();
    consulta(linq);
    cronometro.Stop();

    Console.WriteLine(
        $"Tiempo: {cronometro.Elapsed.TotalMilliseconds:F3} ms"
    );

 
    Console.WriteLine("\nDATAFRAME:");

    cronometro.Restart();
    consulta(dataframe);
    cronometro.Stop();

    Console.WriteLine(
        $"Tiempo: {cronometro.Elapsed.TotalMilliseconds:F3} ms"
    );
}


void Mostrar(IEnumerable resultados)
{
    foreach (var resultado in resultados)
    {
        Console.WriteLine(resultado);
    }
}