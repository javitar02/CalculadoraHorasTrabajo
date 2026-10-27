using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CalculadoraHorasTrabajo.Models;
using CalculadoraHorasTrabajo.Services;

namespace CalculadoraHorasTrabajo.ViewModels;

public partial class PicajeViewModel : ObservableObject
{
    private readonly CalculoService _calculoService = new();
    private readonly StorageService _storageService = new();

    [ObservableProperty]
    private string turno = "Mañana";

    [ObservableProperty]
    private DateTime fechaInicioSemana = GetLunes(DateTime.Today);

    public ObservableCollection<string> Turnos { get; } = new() { "Mañana", "Tarde" };

    public ObservableCollection<DiaSemana> Dias { get; } = new()
    {
        new() { Nombre = "Lunes", Fecha = GetLunes(DateTime.Today), Descanso = false },
        new() { Nombre = "Martes", Fecha = GetLunes(DateTime.Today).AddDays(1), Descanso = false },
        new() { Nombre = "Miércoles", Fecha = GetLunes(DateTime.Today).AddDays(2), Descanso = false },
        new() { Nombre = "Jueves", Fecha = GetLunes(DateTime.Today).AddDays(3), Descanso = false },
        new() { Nombre = "Viernes", Fecha = GetLunes(DateTime.Today).AddDays(4), Descanso = false },
        new() { Nombre = "Sábado", Fecha = GetLunes(DateTime.Today).AddDays(5), Descanso = false },
        new() { Nombre = "Domingo", Fecha = GetLunes(DateTime.Today).AddDays(6), Descanso = false },
    };

    public PicajeViewModel()
    {
        ActualizarFechas();
    }

    partial void OnFechaInicioSemanaChanged(DateTime value)
    {
        ActualizarFechas();
    }

    private void ActualizarFechas()
    {
        var lunes = GetLunes(FechaInicioSemana.Date);
        FechaInicioSemana = lunes;
        var nombres = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };
        for (int i = 0; i < Dias.Count; i++)
        {
            Dias[i].Nombre = nombres[i];
            Dias[i].Fecha = lunes.AddDays(i);
            Dias[i].Descanso = false;
        }
    }

    private static DateTime GetLunes(DateTime fecha)
    {
        var diff = ((int)fecha.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return fecha.Date.AddDays(-diff);
    }



    [RelayCommand]
    private async Task Generar()
    {
        var registros = await _storageService.CargarAsync();
        TimeOnly entrada, salida;
        if (Turno == "Mañana")
        {
            entrada = new TimeOnly(7, 0);
            salida = new TimeOnly(15, 0);
        }
        else
        {
            entrada = new TimeOnly(15, 0);
            salida = new TimeOnly(23, 0);
        }

        foreach (var dia in Dias)
        {
            if (dia.Descanso) continue;
            var horasNetas = _calculoService.CalcularHorasNetas(entrada, salida, 0);
            if (horasNetas <= 0) continue;

            var registro = new RegistroDiario
            {
                Fecha = dia.Fecha,
                HoraEntrada = entrada,
                HoraSalida = salida,
                DescansoMinutos = 0,
                HorasNetas = horasNetas
            };
            registros.Add(registro);
        }

        // Eliminar duplicados por fecha (mantener último)
        var unicos = registros
            .GroupBy(r => r.Fecha.Date)
            .Select(g => g.OrderByDescending(x => x.Id).First())
            .OrderByDescending(x => x.Fecha)
            .ToList();

        await _storageService.GuardarAsync(unicos);

        // Pasar a la siguiente semana automáticamente
        FechaInicioSemana = FechaInicioSemana.AddDays(7);
        ActualizarFechas();
        
        if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
            await Application.Current.Windows[0].Page.DisplayAlert("Hecho", "Se han generado los registros de la semana. Pasando a la siguiente semana.", "OK");
        else if (Application.Current?.MainPage != null)
            await Application.Current.MainPage.DisplayAlert("Hecho", "Se han generado los registros de la semana. Pasando a la siguiente semana.", "OK");
    }
}

public partial class DiaSemana : ObservableObject
{
    [ObservableProperty]
    private string nombre = "";

    [ObservableProperty]
    private DateTime fecha;

    [ObservableProperty]
    private bool descanso;
}
