using CalculadoraHorasTrabajo.Models;

namespace CalculadoraHorasTrabajo.Services;

public class CalculoService
{
    public double CalcularHorasNetas(TimeOnly entrada, TimeOnly salida, int descansoMinutos)
    {
        if (salida <= entrada)
        {
            // Cruce de medianoche
            var totalMinutos = ((salida.AddHours(24) - entrada).TotalMinutes) - descansoMinutos;
            if (totalMinutos < 0) return 0;
            return Math.Round(totalMinutos / 60.0, 2);
        }
        else
        {
            var totalMinutos = (salida - entrada).TotalMinutes - descansoMinutos;
            if (totalMinutos < 0) return 0;
            return Math.Round(totalMinutos / 60.0, 2);
        }
    }

    public bool EsValido(TimeOnly entrada, TimeOnly salida, int descansoMinutos)
    {
        if (descansoMinutos < 0) return false;
        var horas = CalcularHorasNetas(entrada, salida, descansoMinutos);
        return horas > 0;
    }
}
