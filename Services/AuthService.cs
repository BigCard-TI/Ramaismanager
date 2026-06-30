using Microsoft.Data.SqlClient;
using RamaisManager.Security;

namespace RamaisManager.Services;

public class OperadorAutenticado
{
    public string Codigo { get; init; } = string.Empty;
    public string Nome { get; init; } = string.Empty;

    /// <summary>
    /// Apenas o primeiro nome, extraído de Nome (ex.: "RENAN SOUZA" → "Renan").
    /// Usado para exibição amigável na tela principal.
    /// </summary>
    public string PrimeiroNome
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Nome)) return Codigo;

            var primeiro = Nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];

            // Deixa só a primeira letra maiúscula, resto minúsculo (caso o banco
            // guarde tudo em caixa alta, ex.: "RENAN" → "Renan").
            return primeiro.Length > 1
                ? char.ToUpper(primeiro[0]) + primeiro[1..].ToLower()
                : primeiro.ToUpper();
        }
    }
}

public class DbConnectionException : Exception
{
    public DbConnectionException(string message, Exception? inner = null) : base(message, inner) { }
}

/// <summary>
/// Valida o login do operador contra a tabela USUARIOS (CODIGO, SENHA) no SQL Server.
/// A senha em USUARIOS.SENHA está cifrada com o mesmo cifrador numérico posicional
/// usado em outros sistemas internos (não é hash — é reversível, mas aqui comparamos
/// a versão cifrada para evitar manipular a senha em texto puro além do necessário).
/// </summary>
public class AuthService
{
    public OperadorAutenticado? Login(string connectionString, string codigo, string senhaDigitada)
    {
        if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(senhaDigitada))
            return null;

        string senhaCifrada;
        try
        {
            senhaCifrada = SenhaCipher.Encode(senhaDigitada.Trim());
        }
        catch (ArgumentException)
        {
            // Senha não é numérica de até 6 dígitos: já sabemos que não vai bater.
            return null;
        }

        try
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            const string sql = "SELECT TOP 1 CODIGO, NOME FROM USUARIOS WHERE CODIGO = @codigo AND SENHA = @senha";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@codigo", codigo.Trim());
            cmd.Parameters.AddWithValue("@senha", senhaCifrada);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new OperadorAutenticado
                {
                    Codigo = reader["CODIGO"].ToString() ?? codigo.Trim(),
                    Nome = reader["NOME"].ToString() ?? string.Empty,
                };
            }

            return null;
        }
        catch (SqlException ex)
        {
            throw new DbConnectionException(
                "Não foi possível conectar ao banco de dados para validar o login.\n\n" +
                "Verifique a connection string em Configurações (Ctrl+Shift+Alt+C) e se a rede/VPN está ativa.",
                ex);
        }
    }

    public static bool TestConnection(string connectionString, out string? erro)
    {
        try
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            erro = null;
            return true;
        }
        catch (Exception ex)
        {
            erro = ex.Message;
            return false;
        }
    }
}
