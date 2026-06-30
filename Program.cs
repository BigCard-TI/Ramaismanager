using RamaisManager.Security;

namespace RamaisManager;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        Application.ThreadException += (s, e) =>
        {
            MessageBox.Show(
                $"Ocorreu um erro inesperado:\n\n{e.Exception.Message}",
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        };

        var connectionString = ConnectionStringVault.Load();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            MessageBox.Show(
                "A conexão com o banco de dados ainda não está configurada nesta máquina.\n\n" +
                "Pressione Ctrl+Shift+Alt+C na tela de login para configurá-la.",
                "Configuração necessária",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        using var loginForm = new LoginForm(connectionString ?? string.Empty);
        if (loginForm.ShowDialog() != DialogResult.OK || loginForm.OperadorLogado == null)
        {
            return; // usuário cancelou ou não autenticou
        }

        Application.Run(new MainForm(loginForm.OperadorLogado));
    }
}
