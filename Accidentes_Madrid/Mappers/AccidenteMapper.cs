using System.Globalization;
using Accidentes_Madrid.Models;

namespace Accidentes_Madrid.Mappers;

public static class AccidenteMapper
{
    private static readonly CultureInfo cultura = CultureInfo.GetCultureInfo("es-ES");
    public static Accidentes ToModel(string[] cammpos)
    {
        return new Accidentes
        {
            numExpediente = cammpos[0].Trim(),
            fecha = DateOnly.ParseExact(cammpos[1].Trim(), "dd/MM/yyyy", cultura),
            hora = TimeOnly.Parse(cammpos[2].Trim(),cultura),
            localidad = TextoOpcional(cammpos[3].Trim()),
            numero = cammpos[4].Trim(),
            codDistricto = int.Parse(cammpos[5].Trim()),
            districto = cammpos[6].Trim(),
            tipoAccidente = TextoOpcional(cammpos[7].Trim()),
            estadoMeteorologico = TextoOpcional(cammpos[8].Trim()),
            tipoVehiculo = TextoOpcional(cammpos[9].Trim()),
            tipoPersona = ConvertirTipoPersona(cammpos[10].Trim()),
            rangoEdad = cammpos[11].Trim(),
            sexo = ConvertirSexo(cammpos[12].Trim()),
            codLesividad = EnteroOpcional(cammpos[13].Trim()),
            lesividad = TextoOpcional(cammpos[14].Trim()),
            cooordenadaXUtm = DecimalOpcional(cammpos[15].Trim()),
            coordenadaYUtm = DecimalOpcional(cammpos[16].Trim()),
            positivoAlcohol = ConvertirAlcohol(cammpos[17].Trim()),
            positivoDroga = ConvertirDrogas(cammpos[18].Trim()),
        };
    }

    private static string? TextoOpcional(string texto)
    {
        return string.IsNullOrEmpty(texto) ? null : texto.Trim();
    }
    
    private static int? EnteroOpcional(string texto)
    {
        return string.IsNullOrEmpty(texto) ? null : int.Parse(texto.Trim());
    }

    private static double? DecimalOpcional(string texto)
    {
        return string.IsNullOrEmpty(texto) ? null : double.Parse(texto.Trim(),NumberStyles.Float, cultura);
    }

    private static Sexo ConvertirSexo(string texto)
    {
        return texto.Trim() switch
        {
            "Hombre" => Sexo.Hombre,
            "Mujer" => Sexo.Mujer,
            "Desconocido" => Sexo.Desconocido
        };
    }

    private static TipoPersona ConvertirTipoPersona(string texto)
    {
        return texto.Trim() switch
        {
            "Conductor" => TipoPersona.Conductor,
            "Pasajero" => TipoPersona.Pasajero,
            "Peatón" => TipoPersona.Peaton,
            "Peatón (atropello sc)" => TipoPersona.PeatonAtropelladoSc,

        };
    }

    private static bool? ConvertirAlcohol(string texto)
    {
        return texto.Trim() switch
        {

            "S" => true,
            "N" => false,
            "" => null
        };
    }

    private static bool? ConvertirDrogas(string text)
    {
        return text.Trim() switch
        {
            "1" => true,
            "2" => false,
            "" => null
        };
    }

}