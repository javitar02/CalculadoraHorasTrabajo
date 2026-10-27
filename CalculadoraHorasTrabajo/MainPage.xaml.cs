using CalculadoraHorasTrabajo.ViewModels;

namespace CalculadoraHorasTrabajo;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }
}
