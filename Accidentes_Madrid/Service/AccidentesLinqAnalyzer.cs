  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Accidentes_Madrid.Models;
  
  namespace Accidentes_Madrid.Service;
  
  public class AccidentesLinqAnalyzer : IAccidentesAnalyzer
  {
      private readonly List<Accidentes> accidentes;
  
      public AccidentesLinqAnalyzer(List<Accidentes> accidentes)
      {
          this.accidentes = accidentes.ToList();
          
      }
  
      public int TotalAccidentes()
      {
          return accidentes
              .Select(a => a.numExpediente)
              .Distinct()
              .Count();
      }
  
      public List<(string districto, int total)> TopCinco()
      {
          return accidentes
              .DistinctBy(a => a.numExpediente)
              .Where(a => a.districto != null)
              .GroupBy(a => a.districto)
              .Select(g => (districto: g.Key, total: g.Count()))
              .OrderByDescending(g => g.total)
              .ThenBy(g => g.districto)
              .Take(5)
              .ToList();
      }
  
      public Dictionary<string, int> AccidentesPorTipo()
      {
          return accidentes
              .Where(a => a.tipoAccidente != null)
              .DistinctBy(a => a.numExpediente)
              .GroupBy(a => a.tipoAccidente!)
              .ToDictionary(g => g.Key, g => g.Count());
      }
  
      public Dictionary<string, int> AccidentesPorMeteorologia()
      {
          return accidentes
              .Where(a => a.estadoMeteorologico != null)
              .DistinctBy(a => a.numExpediente)
              .GroupBy(a => a.estadoMeteorologico!)
              .ToDictionary(g => g.Key, g => g.Count());
      }
  
      public Dictionary<Sexo, int> PersonaPorSexo()
      {
          return accidentes
              .GroupBy(a => a.sexo)
              .ToDictionary(g => g.Key, g=> g.Count());
      }
  
      public Dictionary<string, int> PersonaPorNombre()
      {
          return accidentes
              .Where(a => a.rangoEdad != null)
              .GroupBy(a => a.rangoEdad)
              .ToDictionary(g => g.Key, g => g.Count());
      }
  
      public int PositivoAlchol()
      {
          return accidentes
              .Count(a => a.positivoAlcohol == true);
      }
  
      public int PositivoDroga()
      {
          return accidentes
              .Count(a => a.positivoDroga == true);
      }
  
      public Dictionary<DayOfWeek, int> AccidentePorDiaSemana()
      {
          return accidentes
              .Where(a => a.fecha.HasValue)
              .DistinctBy(a => a.numExpediente)
              .GroupBy(a => a.fecha.Value.DayOfWeek)
              .ToDictionary(g => g.Key, g => g.Count());
      }
  
      public Dictionary<int, int> AccidentePorMes()
      {
          return accidentes
              .Where(a => a.fecha.HasValue)
              .DistinctBy(a => a.numExpediente)
              .GroupBy(a => a.fecha.Value.Month)
              .ToDictionary(g => g.Key, g => g.Count());
      }
  
      public (int hora, int total)? HorasConMasAccidentes()
      {
          var resultado = accidentes
              .DistinctBy(a => a.numExpediente)
              .GroupBy(a => a.hora.Hour)
              .Select(g => (hora: g.Key, total: g.Count()))
              .OrderByDescending(g => g.total)
              .ThenBy(g => g.hora)
              .ToList();
  
          return resultado.Count == 0 ? null : resultado[0];
      }
  
      public List<(string lesividad, int total)> LesionesMasFrecuentes()
      {
          return accidentes
              .Where(a => a.lesividad != null)
              .GroupBy(a => a.lesividad)
              .Select(g => (lesividad: g.Key, total: g.Count()))
              .OrderByDescending(g => g.total)
              .ThenBy(g => g.lesividad)
              .ToList();
      }
  
      public (string tipoVehiculo, int total)? TipoVehiculoMasImplicado()
      {
          var resultado = accidentes
              .Where(a => !string.IsNullOrWhiteSpace(a.tipoVehiculo))
              .GroupBy(a => a.tipoVehiculo)
              .Select(g => (tipoVehiculo: g.Key, total: g.Count()))
              .OrderByDescending(g => g.total)
              .ThenBy(g => g.tipoVehiculo)
              .ToList();
          
          return resultado.Count == 0 ? null : resultado[0];
      }
  
      public int AccidentesConPeatones()
      {
          return accidentes
              .Where(EsPeaton)
              .Select(a => a.numExpediente)
              .Distinct()
              .Count();
      }
  
      public (double porcentajeHombre, double porcentajeMujeres)? ProporcionHombreMujer()
      {
          int hombre = accidentes.Count(a => a.sexo == Sexo.Hombre);
          int mujer = accidentes.Count(a => a.sexo == Sexo.Mujer);
  
          int total = hombre + mujer;
  
          if (total == 0)
          {
              return null;
          }
  
          return (
              porcentajeHombre: hombre * 100.0 / total,
              porcentajeMujeres: mujer * 100.0 / total
          );
      }
  
      public List<(string districto, int total)> DistrictoConMasPeatones()
      {
          return accidentes
              .Where(EsPeaton)
              .Where(a => a.districto != null)
              .GroupBy(a => a.districto)
              .Select(g => (districto: g.Key, total: g.Count()))
              .OrderByDescending(g => g.total)
              .ThenBy((g => g.districto))
              .ToList();
      }
  
      public (int finDeSemana, int EntreSemana) FinSemanaEntreSemana()
      {
          var recuentos = accidentes
              .Where(a => a.fecha.HasValue)
              .DistinctBy(a => a.numExpediente)
              .GroupBy(a => EsFinDeSemana(a))
              .ToDictionary(g => g.Key, g => g.Count());
          return (
              finDeSemana: recuentos.GetValueOrDefault(true),
              EntreSemana: recuentos.GetValueOrDefault(false)
          );
      }
  
      public double MediaAccidentePorDia()
      {
          var fechas = accidentes
              .Where(a => a.fecha.HasValue)
              .DistinctBy(a => a.numExpediente)
              .Select(a => a.fecha.Value)
              .ToList();
  
          if (fechas.Count == 0)
          {
              return 0;
          }
  
          int primerDia = fechas.Min(a => a.DayNumber);
          int ultimoDia = fechas.Max(a => a.DayNumber);
          int numeroDias = ultimoDia - primerDia + 1;
          return (double)fechas.Count / numeroDias;
      }
  
      // Exige ambos positivos en el mismo registro y cuenta expedientes únicos.
      public int AccidentePorAlcoholDrogras()
      {
          return accidentes
              .Where(a => a.positivoAlcohol == true && a.positivoDroga == true)
              .Select(a => a.numExpediente)
              .Distinct()
              .Count();
      }
  
      public List<(string rangoEdad, int total)> RangoEdadPeatones()
      {
          return accidentes
              .Where(EsPeaton)
              .Where(a => a.rangoEdad != null)
              .GroupBy(a => a.rangoEdad)
              .Select(g => (rangoEdad: g.Key, total: g.Count()))  
              .OrderByDescending(g => g.total)
              .ThenBy(g => g.rangoEdad)
              .ToList();
      }
  
      public List<(string districto, int total)> DistrictoMasAlcohol()
      {
          return accidentes
              .Where(a => a.positivoAlcohol == true)
              .Where(a => a.districto != null)
              .GroupBy(a => a.districto)
              .Select(g => (distrcto: g.Key, total: g.Count()))
              .OrderByDescending(g => g.total)
              .ThenBy(g => g.distrcto)
              .ToList();
      }
  
      public Dictionary<int, int> AccidentePorAnio()
      {
          return accidentes
              .Where(a => a.fecha.HasValue)
              .DistinctBy(a => a.numExpediente)
              .Select(a => a.fecha.Value)
              .GroupBy(a => a.Year)
              .ToDictionary(g => g.Key, g => g.Count());
      }
  
      // Se conserva el nombre de la interfaz: esta consulta agrupa por año y mes, sin filtrar alcohol.
      public Dictionary<(int anio, int mes), int> AccidentePorAnioAlcohol()
      {
          return accidentes
              .Where(a => a.fecha.HasValue)
              .DistinctBy(a => a.numExpediente)
              .Select(a => a.fecha.Value)
              .GroupBy(a => (anio: a.Year, mes: a.Month))
              .ToDictionary(g => g.Key, g => g.Count());
      }
  
      public Dictionary<int, (string districto, int total)> DistrictoConMasAccidentesPorAnio()
      {
          var grupo = accidentes
              .Where(a => a.fecha.HasValue)
              .DistinctBy(a => a.numExpediente)
              .Where(a => a.districto != null)
              .GroupBy(a => a.fecha.Value.Year)
              .ToArray();
  
          return grupo
              .AsParallel()
              .Select(anio => (
                  anio: anio.Key,
                  resultado: anio
                      .GroupBy(a => a.districto)
                      .Select(g => (districto: g.Key, total: g.Count()))
                      .OrderByDescending(g => g.total)
                      .ThenBy(g => g.districto)
                      .First()
              ))
              .OrderBy(g => g.anio)
              .ToDictionary(g => g.anio, g => g.resultado);
      }
  
      public Dictionary<int, int> PositivoAlcoholAnio()
      {
          return accidentes
              .Where(a => a.fecha.HasValue)
              .GroupBy(a => a.fecha.Value.Year)
              .OrderBy(g => g.Key)
              .ToDictionary(g => g.Key, g => g.Count(a => a.positivoAlcohol == true));
      }
  
      public Dictionary<int, (int finDeSemana, int entreSemana)> FinDeSemanaEntreSemana()
      {
          return accidentes
              .Where(a => a.fecha.HasValue)
              .DistinctBy(a => a.numExpediente)
              .GroupBy(a => a.fecha.Value.Year)
              .OrderBy(g => g.Key)
              .ToDictionary(g => g.Key, g => (
                  finDeSemana: g.Count(a => EsFinDeSemana(a)),
                  entreSemana: g.Count(a => !EsFinDeSemana(a))
              ));
      }
  
      public Dictionary<int, (string levisidad, int total)> LesionesMasFreeecuentesPorAnio()
      {
          var grupo = accidentes
              .Where(a => a.fecha.HasValue && a.lesividad != null)
              .GroupBy(a => a.fecha.Value.Year)
              .ToArray();
          return grupo
              .AsParallel()
              .Select(anio =>(
                  anio: anio.Key,
                  resultado: anio
                      .GroupBy(a => a.lesividad)
                      .Select(g => (levisidad: g.Key, total: g.Count()))
                      .OrderByDescending(g => g.total)
                      .ThenBy(g => g.levisidad)
                      .First()
                  ))
              .OrderBy(g => g.anio)
              .ToDictionary(g => g.anio, g => g.resultado);
      }
  
      public Dictionary<int, int> PeatonesPorAnio()
      {
          return accidentes
              .Where(a => a.fecha.HasValue)
              .GroupBy(a => a.fecha.Value.Year)
              .OrderBy(g => g.Key)
              .ToDictionary(
                  g => g.Key,
                  g => g.Count(EsPeaton)
                  );
      }
  
      public Dictionary<int, int> AccidentesPorCodigoDistrito()
      {
          return accidentes
              .Where(a => a.codDistricto.HasValue)
              .DistinctBy(a => a.numExpediente)
              .GroupBy(a => a.codDistricto.Value)
              .ToDictionary(g => g.Key, g => g.Count());
  
      }
  
      public Dictionary<int, (int hora, int total)> HoraPicoPorAnio()
      {
          var grupo = accidentes
              .Where(a => a.fecha.HasValue)
              .DistinctBy(a => a.numExpediente)
              .GroupBy(a => a.fecha.Value.Year)
              .ToArray();
          
          return grupo
              .AsParallel()
              .Select(anio =>
                  (
                      anio: anio.Key,
                      resultado: anio
                      .GroupBy(a => a.hora.Hour)
                      .Select(g => (hora: g.Key, total: g.Count()))
                      .OrderByDescending(g => g.total)
                      .ThenBy(g => g.hora)
                      .First()
                      ))
              .OrderBy(g => g.anio)
              .ToDictionary(g => g.anio, g => g.resultado);
      }
  
      private static bool EsPeaton(Accidentes accidente)
      {
          return accidente.tipoPersona == TipoPersona.Peaton
                 || accidente.tipoPersona == TipoPersona.PeatonAtropelladoSc;
      }
  
  
      private static bool EsFinDeSemana(Accidentes accidente)
      {
          return accidente.fecha.HasValue
                 && (accidente.fecha.Value.DayOfWeek == DayOfWeek.Saturday
                     || accidente.fecha.Value.DayOfWeek == DayOfWeek.Sunday);
      }
  }
