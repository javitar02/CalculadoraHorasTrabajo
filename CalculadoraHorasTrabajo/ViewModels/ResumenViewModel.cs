using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CalculadoraHorasTrabajo.Models;
using CalculadoraHorasTrabajo.Services;

namespace CalculadoraHorasTrabajo.ViewModels;

public partial class ResumenViewModel : ObservableObject
{
    private readonly StorageService _storage = new();

    public ObservableCollection<ResumenMensual> ResumenMensual { get; } = new();

    public ResumenViewModel()
    {
        Cargar();
    }

    public async void Cargar()
    {
        var registros = await _storage.CargarAsync();
        ResumenMensual.Clear();
        var grupos = registros
            .GroupBy(r => new { r.Fecha.Year, r.Fecha.Month })
            .Select(g => new ResumenMensual
            {
                Año = g.Key.Year,
                Mes = g.Key.Month,
                TotalHoras = g.Sum(x => x.HorasNetas)
            })
            .OrderByDescending(x => x.Año)
            .ThenByDescending(x => x.Mes)
            .ToList();

        foreach (var g in grupos)
            ResumenMensual.Add(g);
    }

    [RelayCommand]
    private async Task AbrirDetalle(ResumenMensual? resumen)
    {
        if (resumen == null) return;
        var route = $"detallemes?mes={resumen.Mes}&anio={resumen.Año}";
        await Shell.Current.GoToAsync(route);
    }
}

public class ResumenMensual
{
    public int Año { get; set; }
    public int Mes { get; set; }
    public double TotalHoras { get; set; }
    public string NombreMes => new DateTime(Año, Mes, 1).ToString("MMMM yyyy");
}
