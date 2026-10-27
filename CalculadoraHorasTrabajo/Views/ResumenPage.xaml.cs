using CalculadoraHorasTrabajo.ViewModels;

namespace CalculadoraHorasTrabajo.Views;

public partial class ResumenPage : ContentPage
{
    public ResumenPage()
    {
        InitializeComponent();
        BindingContext = new ResumenViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ResumenViewModel vm)
            vm.Cargar();
    }
}
