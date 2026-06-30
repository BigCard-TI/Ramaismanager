using RamaisManager.Config;

namespace RamaisManager;

public partial class SettingsForm : Form
{
    public AppSettings Settings { get; private set; }

    public SettingsForm(AppSettings currentSettings)
    {
        InitializeComponent();
        Settings = new AppSettings { XmlPath = currentSettings.XmlPath };
        txtXmlPath.Text = Settings.XmlPath;
    }

    private void btnBrowse_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Selecione o arquivo Contacts.xml",
            Filter = "Arquivos XML (*.xml)|*.xml|Todos os arquivos (*.*)|*.*",
            CheckFileExists = false,
        };

        if (!string.IsNullOrWhiteSpace(txtXmlPath.Text) && File.Exists(txtXmlPath.Text))
        {
            dialog.InitialDirectory = Path.GetDirectoryName(txtXmlPath.Text);
        }

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            txtXmlPath.Text = dialog.FileName;
        }
    }

    private void btnTest_Click(object? sender, EventArgs e)
    {
        var path = txtXmlPath.Text.Trim();

        if (string.IsNullOrWhiteSpace(path))
        {
            MessageBox.Show(this, "Informe um caminho primeiro.", "Caminho vazio",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (File.Exists(path))
        {
            MessageBox.Show(this, "Arquivo encontrado com sucesso!", "OK",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            MessageBox.Show(this,
                "Arquivo não encontrado nesse caminho.\n\n" +
                "Verifique se a rede está acessível e se o caminho está correto.",
                "Não encontrado",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        var path = txtXmlPath.Text.Trim();

        if (string.IsNullOrWhiteSpace(path))
        {
            MessageBox.Show(this, "Informe um caminho válido antes de salvar.", "Caminho vazio",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Settings = new AppSettings { XmlPath = path };
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
