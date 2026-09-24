using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Accidentes_Madrid.Mappers;
using Accidentes_Madrid.Models;

namespace Accidentes_Madrid.Repository;

public class AccidenteRepository
{
    public async Task<List<Accidentes>> CargarAsync(
        string ruta2024,
        string ruta2025,
        string ruta2026)
    {
        var tarea2024 = LeerArchivoAsync(ruta2024);
        var tarea2025 = LeerArchivoAsync(ruta2025);
        var tarea2026 = LeerArchivoAsync(ruta2026);
        
        var listas = await Task.WhenAll(tarea2024, tarea2025, tarea2026);
        
         var accidentes = new List<Accidentes>();
            
            foreach (var lista in listas){
                accidentes.AddRange(lista);
                
            }

            return accidentes;

    }
    
    private async Task<List<Accidentes>> LeerArchivoAsync(string ruta)
    {
        var accidentes = new List<Accidentes>();
        var configuracion = new CsvConfiguration(CultureInfo.GetCultureInfo("es-ES"))
        {
            Delimiter = ";",
            HasHeaderRecord = true,
        };

        using var lector = new StreamReader(ruta);
        using var csv = new CsvReader(lector, configuracion);

        if (!await csv.ReadAsync())
        {
            throw new FormatException($"El archivo esta vario: {ruta}");
        }

        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            if (csv.Parser.Count != 19)
            {
                throw new FormatException($"Ellll arcgivo {ruta} tiene una fila sin las 19 columnas");
            }
            
            var campos = new string[19];

            for (int i = 0; i < campos.Length; i++)
            {
                campos[i] = csv.GetField(i) ?? "";
            }

            var accidente = AccidenteMapper.ToModel(campos);
            accidentes.Add(accidente);
        }
        
        return accidentes;
        
    }


}