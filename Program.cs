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

        Application.Run(new MainForm());
    }
}
