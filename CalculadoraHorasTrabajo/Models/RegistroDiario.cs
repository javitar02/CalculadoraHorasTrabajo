using System.Text.Json.Serialization;

namespace CalculadoraHorasTrabajo.Models;

public class RegistroDiario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Fecha { get; set; } = DateTime.Today;
    public TimeOnly HoraEntrada { get; set; } = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);
    public TimeOnly HoraSalida { get; set; } = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);
    public int DescansoMinutos { get; set; } = 0;
    public double HorasNetas { get; set; } = 0;
}
