using CalculadoraHorasTrabajo.ViewModels;

namespace CalculadoraHorasTrabajo.Views;

public partial class PicajePage : ContentPage
{
    public PicajePage()
    {
        InitializeComponent();
        BindingContext = new PicajeViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PicajeViewModel vm)
        {
            // Mantener la lógica: primera semana del mes al iniciar si es la primera vez
            // pero si ya se ha avanzado, respetar el valor actual
        }
    }
}
