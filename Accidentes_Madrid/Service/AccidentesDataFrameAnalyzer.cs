using System;
using System.Collections.Generic;
using System.Linq;
using Accidentes_Madrid.Models;
using Microsoft.Data.Analysis;

namespace Accidentes_Madrid.Service;

public class AccidentesDataFrameAnalyzer : IAccidentesAnalyzer
{
    private readonly DataFrame datos;

    public AccidentesDataFrameAnalyzer(List<Accidentes> accidentes)
    {
        datos = new DataFrame(
            new StringDataFrameColumn(
                "Expediente",
                accidentes.Select(a => a.numExpediente)
            ),
            new StringDataFrameColumn(
                "Distrito",
                accidentes.Select(a => a.districto)
            ),
            new Int32DataFrameColumn(
                "CodigoDistrito",
                accidentes.Select(a => (int?)a.codDistricto)
            ),
            new StringDataFrameColumn(
                "Tipo",
                accidentes.Select(a => a.tipoAccidente)
            ),
            new StringDataFrameColumn(
                "Meteorologia",
                accidentes.Select(a => a.estadoMeteorologico)
            ),
            new Int32DataFrameColumn(
                "Sexo",
                accidentes.Select(a => (int)a.sexo)
            ),
            new StringDataFrameColumn(
                "Edad",
                accidentes.Select(a => a.rangoEdad)
            ),
            new BooleanDataFrameColumn(
                "Alcohol",
                accidentes.Select(a => (bool?)a.positivoAlcohol)
            ),
            new BooleanDataFrameColumn(
                "Droga",
                accidentes.Select(a => (bool?)a.positivoDroga)
            ),
            new Int32DataFrameColumn(
                "DiaSemana",
                accidentes.Select(a =>
                    a.fecha.HasValue
                        ? (int?)a.fecha.Value.DayOfWeek
                        : null)
            ),
            new Int32DataFrameColumn(
                "Mes",
                accidentes.Select(a => a.fecha?.Month)
            ),
            new Int32DataFrameColumn(
                "Anio",
                accidentes.Select(a => a.fecha?.Year)
            ),
            new Int32DataFrameColumn(
                "AnioMes",
                accidentes.Select(a =>
                    a.fecha.HasValue
                        ? (int?)(a.fecha.Value.Year * 100
                                 + a.fecha.Value.Month)
                        : null)
            ),
            new Int32DataFrameColumn(
                "Dia",
                accidentes.Select(a => a.fecha?.DayNumber)
            ),
            new Int32DataFrameColumn(
                "Hora",
                accidentes.Select(a => a.hora.Hour)
            ),
            new StringDataFrameColumn(
                "Lesividad",
                accidentes.Select(a => a.lesividad)
            ),
            new StringDataFrameColumn(
                "Vehiculo",
                accidentes.Select(a => a.tipoVehiculo)
            ),
            new BooleanDataFrameColumn(
                "Peaton",
                accidentes.Select(a =>
                    a.tipoPersona == TipoPersona.Peaton ||
                    a.tipoPersona == TipoPersona.PeatonAtropelladoSc)
            ),
            new Int32DataFrameColumn(
                "Registro",
                accidentes.Select(a => 1)
            )
        );
    }

   
    public int TotalAccidentes()
    {
        return NumeroFilas(Unicos(datos));
    }

   
    public List<(string districto, int total)> TopCinco()
    {
        return Ranking(Unicos(datos), "Distrito")
            .Take(5)
            .Select(r => (districto: r.valor, total: r.total))
            .ToList();
    }


    public Dictionary<string, int> AccidentesPorTipo()
    {
        var tabla = Unicos(SinNulos(datos, "Tipo"));

        return ContarPor<string>(tabla, "Tipo");
    }

    
    public Dictionary<string, int> AccidentesPorMeteorologia()
    {
        var tabla = Unicos(SinNulos(datos, "Meteorologia"));

        return ContarPor<string>(tabla, "Meteorologia");
    }

    
    public Dictionary<Sexo, int> PersonaPorSexo()
    {
        return ContarPor<int>(datos, "Sexo")
            .ToDictionary(g => (Sexo)g.Key, g => g.Value);
    }

    
    public Dictionary<string, int> PersonaPorNombre()
    {
        return ContarPor<string>(datos, "Edad");
    }

   
    public int PositivoAlchol()
    {
        return NumeroFilas(Igual(datos, "Alcohol", true));
    }

  
    public int PositivoDroga()
    {
        return NumeroFilas(Igual(datos, "Droga", true));
    }

    
    public Dictionary<DayOfWeek, int> AccidentePorDiaSemana()
    {
        var tabla = Unicos(SinNulos(datos, "DiaSemana"));

        return ContarPor<int>(tabla, "DiaSemana")
            .ToDictionary(
                g => (DayOfWeek)g.Key,
                g => g.Value
            );
    }

    
    public Dictionary<int, int> AccidentePorMes()
    {
        var tabla = Unicos(SinNulos(datos, "Mes"));

        return ContarPor<int>(tabla, "Mes");
    }

   
    public (int hora, int total)? HorasConMasAccidentes()
    {
        var recuentos = ContarPor<int>(Unicos(datos), "Hora");

        if (recuentos.Count == 0)
        {
            return null;
        }

        var primero = recuentos
            .OrderByDescending(g => g.Value)
            .ThenBy(g => g.Key)
            .First();

        return (primero.Key, primero.Value);
    }


    public List<(string lesividad, int total)> LesionesMasFrecuentes()
    {
        return Ranking(datos, "Lesividad");
    }

   
    public (string tipoVehiculo, int total)? TipoVehiculoMasImplicado()
    {
        var tabla = Filtrar(
            datos,
            i => !string.IsNullOrWhiteSpace(
                datos.Columns["Vehiculo"][i] as string)
        );

        var ranking = Ranking(tabla, "Vehiculo");

        if (ranking.Count == 0)
        {
            return null;
        }

        return (ranking[0].valor, ranking[0].total);
    }

  
    public int AccidentesConPeatones()
    {
        var peatones = Igual(datos, "Peaton", true);

       
        return NumeroFilas(Unicos(peatones));
    }

  
    public (double porcentajeHombre, double porcentajeMujeres)?
        ProporcionHombreMujer()
    {
        int hombres = NumeroFilas(
            Igual(datos, "Sexo", (int)Sexo.Hombre)
        );

        int mujeres = NumeroFilas(
            Igual(datos, "Sexo", (int)Sexo.Mujer)
        );

        int total = hombres + mujeres;

        if (total == 0)
        {
            return null;
        }

        return (
            hombres * 100.0 / total,
            mujeres * 100.0 / total
        );
    }

 
    public List<(string districto, int total)> DistrictoConMasPeatones()
    {
        return Ranking(
            Igual(datos, "Peaton", true),
            "Distrito"
        );
    }

   
    public (int finDeSemana, int EntreSemana) FinSemanaEntreSemana()
    {
        var tabla = Unicos(SinNulos(datos, "DiaSemana"));

        int finDeSemana = ContarFinDeSemana(tabla);

        return (
            finDeSemana,
            NumeroFilas(tabla) - finDeSemana
        );
    }

    public double MediaAccidentePorDia()
    {
        var tabla = Unicos(SinNulos(datos, "Dia"));
        var porDia = ContarPor<int>(tabla, "Dia");

        if (porDia.Count == 0)
        {
            return 0;
        }

        int primerDia = porDia.Keys.Min();
        int ultimoDia = porDia.Keys.Max();
        int numeroDias = ultimoDia - primerDia + 1;

        return (double)NumeroFilas(tabla) / numeroDias;
    }

 
    public int AccidentePorAlcoholDrogras()
    {
        var tabla = Igual(datos, "Alcohol", true);
        tabla = Igual(tabla, "Droga", true);

        return NumeroFilas(Unicos(tabla));
    }

  
    public List<(string rangoEdad, int total)> RangoEdadPeatones()
    {
        return Ranking(
            Igual(datos, "Peaton", true),
            "Edad"
        );
    }

    
    public List<(string districto, int total)> DistrictoMasAlcohol()
    {
        return Ranking(
            Igual(datos, "Alcohol", true),
            "Distrito"
        );
    }

    
    public Dictionary<int, int> AccidentesPorCodigoDistrito()
    {
        var tabla = Unicos(SinNulos(datos, "CodigoDistrito"));

        return ContarPor<int>(tabla, "CodigoDistrito");
    }

    
    public Dictionary<int, int> AccidentePorAnio()
    {
        var tabla = Unicos(SinNulos(datos, "Anio"));

        return ContarPor<int>(tabla, "Anio");
    }

 
    public Dictionary<(int anio, int mes), int> AccidentePorAnioAlcohol()
    {
        var tabla = Unicos(SinNulos(datos, "AnioMes"));

       
        return ContarPor<int>(tabla, "AnioMes")
            .ToDictionary(
                g => (anio: g.Key / 100, mes: g.Key % 100),
                g => g.Value
            );
    }

    
    public Dictionary<int, (string districto, int total)>
        DistrictoConMasAccidentesPorAnio()
    {
        var resultado =
            new Dictionary<int, (string districto, int total)>();

        var tabla = Unicos(SinNulos(datos, "Anio"));

        foreach (int anio in Anios(tabla))
        {
            var ranking = Ranking(
                Igual(tabla, "Anio", anio),
                "Distrito"
            );

            
            if (ranking.Count > 0)
            {
                resultado.Add(
                    anio,
                    (ranking[0].valor, ranking[0].total)
                );
            }
        }

        return resultado;
    }

    
    public Dictionary<int, int> PositivoAlcoholAnio()
    {
        var resultado = new Dictionary<int, int>();

        foreach (int anio in Anios(datos))
        {
            var tabla = Igual(datos, "Anio", anio);
            tabla = Igual(tabla, "Alcohol", true);

            resultado.Add(anio, NumeroFilas(tabla));
        }

        return resultado;
    }


    public Dictionary<int, (int finDeSemana, int entreSemana)>
        FinDeSemanaEntreSemana()
    {
        var resultado =
            new Dictionary<int, (int finDeSemana, int entreSemana)>();

        var tabla = Unicos(SinNulos(datos, "Anio"));

        foreach (int anio in Anios(tabla))
        {
            var tablaAnio = Igual(tabla, "Anio", anio);
            int finDeSemana = ContarFinDeSemana(tablaAnio);

            resultado.Add(
                anio,
                (
                    finDeSemana,
                    NumeroFilas(tablaAnio) - finDeSemana
                )
            );
        }

        return resultado;
    }

 
    public Dictionary<int, (int hora, int total)> HoraPicoPorAnio()
    {
        var resultado = new Dictionary<int, (int hora, int total)>();
        var tabla = Unicos(SinNulos(datos, "Anio"));

        foreach (int anio in Anios(tabla))
        {
            var recuentos = ContarPor<int>(
                Igual(tabla, "Anio", anio),
                "Hora"
            );

            if (recuentos.Count == 0)
            {
                continue;
            }

            var primero = recuentos
                .OrderByDescending(g => g.Value)
                .ThenBy(g => g.Key)
                .First();

            resultado.Add(anio, (primero.Key, primero.Value));
        }

        return resultado;
    }


    public Dictionary<int, (string levisidad, int total)>
        LesionesMasFreeecuentesPorAnio()
    {
        var resultado =
            new Dictionary<int, (string levisidad, int total)>();

        foreach (int anio in Anios(datos))
        {
            var ranking = Ranking(
                Igual(datos, "Anio", anio),
                "Lesividad"
            );

            if (ranking.Count > 0)
            {
                resultado.Add(
                    anio,
                    (ranking[0].valor, ranking[0].total)
                );
            }
        }

        return resultado;
    }

    public Dictionary<int, int> PeatonesPorAnio()
    {
        var resultado = new Dictionary<int, int>();

        foreach (int anio in Anios(datos))
        {
            var tabla = Igual(datos, "Anio", anio);
            tabla = Igual(tabla, "Peaton", true);

            resultado.Add(anio, NumeroFilas(tabla));
        }

        return resultado;
    }

   
    private static int NumeroFilas(DataFrame tabla)
    {
        return checked((int)tabla.Rows.Count);
    }

    private static DataFrame Filtrar(
        DataFrame tabla,
        Func<long, bool> condicion)
    {
        var mascara = new BooleanDataFrameColumn("Filtro");

        for (long i = 0; i < tabla.Rows.Count; i++)
        {
            mascara.Append(condicion(i));
        }

        return tabla.Filter(mascara);
    }


    private static DataFrame Igual(
        DataFrame tabla,
        string columna,
        object valor)
    {
        return Filtrar(
            tabla,
            i => Equals(tabla.Columns[columna][i], valor)
        );
    }

   
    private static DataFrame SinNulos(
        DataFrame tabla,
        string columna)
    {
        return Filtrar(
            tabla,
            i => tabla.Columns[columna][i] != null
        );
    }


    private static DataFrame Unicos(DataFrame tabla)
    {
        var vistos = new HashSet<string?>();

        return Filtrar(
            tabla,
            i => vistos.Add(
                (string?)tabla.Columns["Expediente"][i])
        );
    }

    
    private static Dictionary<T, int> ContarPor<T>(
        DataFrame tabla,
        string columna) where T : notnull
    {
        var resultado = new Dictionary<T, int>();
        var validos = SinNulos(tabla, columna);

        if (validos.Rows.Count == 0)
        {
            return resultado;
        }

        
        var agrupado = validos
            .GroupBy(columna)
            .Count("Registro");

        for (long i = 0; i < agrupado.Rows.Count; i++)
        {
            T clave = (T)agrupado.Columns[columna][i]!;
            int total = Convert.ToInt32(
                agrupado.Columns["Registro"][i]
            );

            resultado.Add(clave, total);
        }

        return resultado;
    }

 
    private static List<(string valor, int total)> Ranking(
        DataFrame tabla,
        string columna)
    {
        return ContarPor<string>(tabla, columna)
            .OrderByDescending(g => g.Value)
            .ThenBy(g => g.Key)
            .Select(g => (valor: g.Key, total: g.Value))
            .ToList();
    }

    private static List<int> Anios(DataFrame tabla)
    {
        return ContarPor<int>(tabla, "Anio")
            .Keys
            .OrderBy(anio => anio)
            .ToList();
    }


    private static int ContarFinDeSemana(DataFrame tabla)
    {
        var finDeSemana = Filtrar(
            tabla,
            i =>
                Equals(
                    tabla.Columns["DiaSemana"][i],
                    (int)DayOfWeek.Saturday
                )
                ||
                Equals(
                    tabla.Columns["DiaSemana"][i],
                    (int)DayOfWeek.Sunday
                )
        );

        return NumeroFilas(finDeSemana);
    }
}