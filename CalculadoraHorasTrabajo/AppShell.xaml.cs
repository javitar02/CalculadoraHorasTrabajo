namespace CalculadoraHorasTrabajo
{
    public partial class AppShell : Shell
    {
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("detallemes", typeof(Views.DetalleMesPage));
	}
    }
}
