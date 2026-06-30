using System.Security.Cryptography;
using Microsoft.Win32;

namespace RamaisManager.Security;

/// <summary>
/// Persiste a connection string do SQL Server no registro do Windows (HKCU),
/// protegida com DPAPI (escopo CurrentUser) — mesmo padrão usado no MapaMaquinas
/// e no DbMapper. Como o DPAPI usa o perfil do usuário do Windows logado, o valor
/// só pode ser decifrado pela mesma conta/máquina que o gravou.
/// </summary>
public static class ConnectionStringVault
{
    private const string RegistryPath = @"Software\BigCard\RamaisManager";
    private const string ValueName = "DbConnectionProtected";

    public static void Save(string connectionString)
    {
        var plainBytes = System.Text.Encoding.UTF8.GetBytes(connectionString);
        var protectedBytes = ProtectedData.Protect(plainBytes, optionalEntropy: null, DataProtectionScope.CurrentUser);

        using var key = Registry.CurrentUser.CreateSubKey(RegistryPath);
        key.SetValue(ValueName, Convert.ToBase64String(protectedBytes), RegistryValueKind.String);
    }

    public static string? Load()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
        var stored = key?.GetValue(ValueName) as string;
        if (string.IsNullOrWhiteSpace(stored)) return null;

        try
        {
            var protectedBytes = Convert.FromBase64String(stored);
            var plainBytes = ProtectedData.Unprotect(protectedBytes, optionalEntropy: null, DataProtectionScope.CurrentUser);
            return System.Text.Encoding.UTF8.GetString(plainBytes);
        }
        catch (CryptographicException)
        {
            // Valor corrompido, ou gravado por outro usuário/máquina (DPAPI é por perfil).
            return null;
        }
    }

    public static bool IsConfigured()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
        return !string.IsNullOrWhiteSpace(key?.GetValue(ValueName) as string);
    }

    public static void Clear()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryPath, writable: true);
        key?.DeleteValue(ValueName, throwOnMissingValue: false);
    }
}
