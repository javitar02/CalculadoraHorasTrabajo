using CalculadoraHorasTrabajo.Models;
using CalculadoraHorasTrabajo.Services;
using System.Globalization;

namespace CalculadoraHorasTrabajo.Views;

[QueryProperty(nameof(Mes), "mes")]
[QueryProperty(nameof(Año), "anio")]
public partial class DetalleMesPage : ContentPage
{
    private int _mes;
    private int _anio;

    public int Mes
    {
        get => _mes;
        set { _mes = value; Cargar(); }
    }

    public int Año
    {
        get => _anio;
        set { _anio = value; Cargar(); }
    }

    private readonly StorageService _storage = new();

    public DetalleMesPage()
    {
        InitializeComponent();
    }

    private async void Cargar()
    {
        if (_mes == 0 || _anio == 0) return;
        TituloMes.Text = $"Detalle de {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(_mes)} {_anio}";
        var registros = await _storage.CargarAsync();
        var filtrados = registros.Where(r => r.Fecha.Month == _mes && r.Fecha.Year == _anio)
            .OrderBy(r => r.Fecha)
            .ToList();

        DiariasCollection.ItemsSource = filtrados;

        // Semanal simple: agrupar por semana ISO aproximado
        var semanales = filtrados
            .GroupBy(r => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(r.Fecha, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
            .Select(g => new
            {
                Rango = $"Semana {g.Key}",
                TotalHoras = g.Sum(x => x.HorasNetas)
            })
            .ToList();

        SemanalesCollection.ItemsSource = semanales;
    }
}
