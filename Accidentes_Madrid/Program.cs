using System;
using System.Collections;
using System.Diagnostics;
using Accidentes_Madrid.Repository;
using Accidentes_Madrid.Service;

// Cargar los tres archivos.
// El repositorio ya utiliza Task.WhenAll.
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

// Crear los dos servicios con los mismos datos.
var linq = new AccidentesLinqAnalyzer(accidentes);
var dataframe = new AccidentesDataFrameAnalyzer(accidentes);

// 1. Total de accidentes.
Consultar("1. Total de accidentes", servicio =>
    Console.WriteLine(servicio.TotalAccidentes()));

// 2. Los cinco distritos con más accidentes.
Consultar("2. Top cinco distritos", servicio =>
    Mostrar(servicio.TopCinco()));

// 3. Accidentes por tipo.
Consultar("3. Accidentes por tipo", servicio =>
    Mostrar(servicio.AccidentesPorTipo()));

// 4. Accidentes por estado meteorológico.
Consultar("4. Accidentes por meteorología", servicio =>
    Mostrar(servicio.AccidentesPorMeteorologia()));

// 5. Personas implicadas por sexo.
Consultar("5. Personas por sexo", servicio =>
    Mostrar(servicio.PersonaPorSexo()));

// 6. Personas implicadas por rango de edad.
Consultar("6. Personas por rango de edad", servicio =>
    Mostrar(servicio.PersonaPorNombre()));

// 7. Positivos en alcohol.
Consultar("7. Positivos en alcohol", servicio =>
    Console.WriteLine(servicio.PositivoAlchol()));

// 8. Positivos en drogas.
Consultar("8. Positivos en drogas", servicio =>
    Console.WriteLine(servicio.PositivoDroga()));

// 9. Accidentes por día de la semana.
Consultar("9. Accidentes por día de la semana", servicio =>
    Mostrar(servicio.AccidentePorDiaSemana()));

// 10. Accidentes por mes.
Consultar("10. Accidentes por mes", servicio =>
    Mostrar(servicio.AccidentePorMes()));

// 11. Hora con más accidentes.
Consultar("11. Hora con más accidentes", servicio =>
    Console.WriteLine(servicio.HorasConMasAccidentes()));

// 12. Lesiones más frecuentes.
Consultar("12. Lesiones más frecuentes", servicio =>
    Mostrar(servicio.LesionesMasFrecuentes()));

// 13. Tipo de vehículo más implicado.
Consultar("13. Tipo de vehículo más implicado", servicio =>
    Console.WriteLine(servicio.TipoVehiculoMasImplicado()));

// 14. Accidentes con peatones.
Consultar("14. Accidentes con peatones", servicio =>
    Console.WriteLine(servicio.AccidentesConPeatones()));

// 15. Proporción hombre/mujer.
Consultar("15. Proporción hombre/mujer", servicio =>
    Console.WriteLine(servicio.ProporcionHombreMujer()));

// 16. Distritos con más peatones.
Consultar("16. Distritos con más peatones", servicio =>
    Mostrar(servicio.DistrictoConMasPeatones()));

// 17. Fin de semana frente a entre semana.
Consultar("17. Fin de semana frente a entre semana", servicio =>
    Console.WriteLine(servicio.FinSemanaEntreSemana()));

// 18. Media de accidentes por día.
Consultar("18. Media de accidentes por día", servicio =>
    Console.WriteLine(servicio.MediaAccidentePorDia()));

// 19. Accidentes con alcohol y drogas.
Consultar("19. Accidentes con alcohol y drogas", servicio =>
    Console.WriteLine(servicio.AccidentePorAlcoholDrogras()));

// 20. Rangos de edad de peatones.
Consultar("20. Rangos de edad de peatones", servicio =>
    Mostrar(servicio.RangoEdadPeatones()));

// 21. Distritos con más positivos en alcohol.
Consultar("21. Distritos con más positivos en alcohol", servicio =>
    Mostrar(servicio.DistrictoMasAlcohol()));

// 22. Accidentes por código de distrito.
Consultar("22. Accidentes por código de distrito", servicio =>
    Mostrar(servicio.AccidentesPorCodigoDistrito()));

// 23. Accidentes por año.
Consultar("23. Accidentes por año", servicio =>
    Mostrar(servicio.AccidentePorAnio()));

// 24. Evolución mensual por año.
Consultar("24. Evolución mensual por año", servicio =>
    Mostrar(servicio.AccidentePorAnioAlcohol()));

// 25. Distrito con más accidentes por año.
Consultar("25. Distrito con más accidentes por año", servicio =>
    Mostrar(servicio.DistrictoConMasAccidentesPorAnio()));

// 26. Positivos en alcohol por año.
Consultar("26. Positivos en alcohol por año", servicio =>
    Mostrar(servicio.PositivoAlcoholAnio()));

// 27. Fin de semana frente a entre semana por año.
Consultar("27. Fin de semana frente a entre semana por año", servicio =>
    Mostrar(servicio.FinDeSemanaEntreSemana()));

// 28. Hora pico por año.
Consultar("28. Hora pico por año", servicio =>
    Mostrar(servicio.HoraPicoPorAnio()));

// 29. Lesión más frecuente por año.
Consultar("29. Lesión más frecuente por año", servicio =>
    Mostrar(servicio.LesionesMasFreeecuentesPorAnio()));

// 30. Peatones por año.
Consultar("30. Peatones por año", servicio =>
    Mostrar(servicio.PeatonesPorAnio()));


// Ejecutar cada consulta con LINQ y después con DataFrame.
void Consultar(string nombre, Action<IAccidentesAnalyzer> consulta)
{
    Console.WriteLine($"\n--- {nombre} ---");

    var cronometro = new Stopwatch();

    // LINQ
    Console.WriteLine("LINQ:");

    cronometro.Start();
    consulta(linq);
    cronometro.Stop();

    Console.WriteLine(
        $"Tiempo: {cronometro.Elapsed.TotalMilliseconds:F3} ms"
    );

    // DATAFRAME
    Console.WriteLine("\nDATAFRAME:");

    cronometro.Restart();
    consulta(dataframe);
    cronometro.Stop();

    Console.WriteLine(
        $"Tiempo: {cronometro.Elapsed.TotalMilliseconds:F3} ms"
    );
}

// Mostrar los elementos de una lista o un diccionario.
void Mostrar(IEnumerable resultados)
{
    foreach (var resultado in resultados)
    {
        Console.WriteLine(resultado);
    }
}