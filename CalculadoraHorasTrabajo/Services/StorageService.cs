using System.Text.Json;
using CalculadoraHorasTrabajo.Models;

namespace CalculadoraHorasTrabajo.Services;

public class StorageService
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public StorageService()
    {
        var dir = FileSystem.AppDataDirectory;
        _filePath = Path.Combine(dir, "registros.json");
    }

    public async Task<List<RegistroDiario>> CargarAsync()
    {
        try
        {
            if (!File.Exists(_filePath))
                return new List<RegistroDiario>();

            var json = await File.ReadAllTextAsync(_filePath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<RegistroDiario>();

            var lista = JsonSerializer.Deserialize<List<RegistroDiario>>(json, _options);
            return lista ?? new List<RegistroDiario>();
        }
        catch
        {
            return new List<RegistroDiario>();
        }
    }

    public async Task GuardarAsync(List<RegistroDiario> registros)
    {
        try
        {
            var json = JsonSerializer.Serialize(registros, _options);
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch
        {
            // Silencioso por ahora
        }
    }
}
