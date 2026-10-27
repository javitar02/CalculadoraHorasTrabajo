using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CalculadoraHorasTrabajo.Models;
using CalculadoraHorasTrabajo.Services;

namespace CalculadoraHorasTrabajo.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly CalculoService _calculoService = new();
    private readonly StorageService _storageService = new();

    [ObservableProperty]
    private DateTime fecha = DateTime.Today;

    [ObservableProperty]
    private TimeOnly horaEntrada = TimeOnly.FromTimeSpan(new TimeSpan(8, 0, 0));

    [ObservableProperty]
    private TimeOnly horaSalida = TimeOnly.FromTimeSpan(new TimeSpan(17, 0, 0));

    [ObservableProperty]
    private string descansoMinutosTexto = "60";

    [ObservableProperty]
    private int mesSeleccionado = DateTime.Today.Month;

    [ObservableProperty]
    private int añoSeleccionado = DateTime.Today.Year;

    [ObservableProperty]
    private string totalHorasMes = "0.00 h";

    public ObservableCollection<RegistroDiario> Registros { get; } = new();
    public ObservableCollection<RegistroDiario> RegistrosFiltrados { get; } = new();

    public List<int> Meses { get; } = Enumerable.Range(1, 12).ToList();
    public List<int> Años { get; } = Enumerable.Range(DateTime.Today.Year - 5, 6).ToList();

    public MainViewModel()
    {
        _ = CargarRegistrosAsync();
    }

    private async Task CargarRegistrosAsync()
    {
        var lista = await _storageService.CargarAsync();
        Registros.Clear();
        foreach (var r in lista.OrderByDescending(x => x.Fecha))
            Registros.Add(r);
        AplicarFiltro();
    }

    [RelayCommand]
    private async Task Guardar()
    {
        if (!int.TryParse(DescansoMinutosTexto, out int descansoMin))
            descansoMin = 0;

        if (!_calculoService.EsValido(HoraEntrada, HoraSalida, descansoMin))
        {
            if (Application.Current?.MainPage != null)
                await Application.Current.MainPage.DisplayAlert("Error", "Revisa las horas: salida/entrada o descanso inválidos.", "OK");
            return;
        }

        var horasNetas = _calculoService.CalcularHorasNetas(HoraEntrada, HoraSalida, descansoMin);

        var registro = new RegistroDiario
        {
            Fecha = Fecha.Date,
            HoraEntrada = HoraEntrada,
            HoraSalida = HoraSalida,
            DescansoMinutos = descansoMin,
            HorasNetas = horasNetas
        };

        Registros.Insert(0, registro);
        await _storageService.GuardarAsync(Registros.ToList());
        AplicarFiltro();
    }

    [RelayCommand]
    private async Task Eliminar(RegistroDiario? registro)
    {
        if (registro == null) return;
        if (Registros.Contains(registro))
        {
            Registros.Remove(registro);
            await _storageService.GuardarAsync(Registros.ToList());
            AplicarFiltro();
        }
    }

    [RelayCommand]
    private void Filtrar()
    {
        AplicarFiltro();
    }

    partial void OnMesSeleccionadoChanged(int value) => AplicarFiltro();
    partial void OnAñoSeleccionadoChanged(int value) => AplicarFiltro();

    private void AplicarFiltro()
    {
        RegistrosFiltrados.Clear();
        var filtrados = Registros
            .Where(r => r.Fecha.Month == MesSeleccionado && r.Fecha.Year == AñoSeleccionado)
            .OrderByDescending(r => r.Fecha)
            .ToList();

        foreach (var r in filtrados)
            RegistrosFiltrados.Add(r);

        var total = filtrados.Sum(r => r.HorasNetas);
        TotalHorasMes = $"{total:0.00} h";
    }
}
