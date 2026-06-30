using System.Text.Json;

namespace RamaisManager.Config;

/// <summary>
/// Configuração da aplicação, persistida em %AppData%\RamaisManager\config.json.
/// Mantém o caminho do XML fora do código-fonte, para que possa ser alterado
/// sem precisar recompilar o programa.
/// </summary>
public class AppSettings
{
    public string XmlPath { get; set; } = string.Empty;

    private static readonly string ConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RamaisManager");

    private static readonly string ConfigFile = Path.Combine(ConfigDir, "config.json");

    /// <summary>
    /// Caminho padrão sugerido na primeira execução, caso ainda não exista configuração.
    /// Ajuste aqui apenas o valor inicial sugerido — depois disso, tudo é editado
    /// pelo painel oculto de configurações, sem precisar mexer no código.
    /// </summary>
    private const string DefaultSuggestedPath = @"\\SERVIDOR\Compartilhamento\MicroSIP\Contacts.xml";

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(ConfigFile))
            {
                var json = File.ReadAllText(ConfigFile);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings != null && !string.IsNullOrWhiteSpace(settings.XmlPath))
                {
                    return settings;
                }
            }
        }
        catch
        {
            // Config corrompida ou ilegível: cai para o padrão sugerido.
        }

        return new AppSettings { XmlPath = DefaultSuggestedPath };
    }

    public void Save()
    {
        Directory.CreateDirectory(ConfigDir);
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigFile, json);
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(XmlPath);
}
