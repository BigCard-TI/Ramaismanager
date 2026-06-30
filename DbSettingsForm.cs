using Microsoft.Data.SqlClient;
using RamaisManager.Security;
using RamaisManager.Services;

namespace RamaisManager;

public partial class DbSettingsForm : Form
{
    public bool FoiSalvo { get; private set; } = false;

    public DbSettingsForm()
    {
        InitializeComponent();
        CarregarConfiguracaoAtual();
    }

    private void CarregarConfiguracaoAtual()
    {
        var connString = ConnectionStringVault.Load();
        if (string.IsNullOrWhiteSpace(connString)) return;

        try
        {
            var builder = new SqlConnectionStringBuilder(connString);
            txtServidor.Text = builder.DataSource;
            txtBanco.Text = builder.InitialCatalog;
            chkWindowsAuth.Checked = builder.IntegratedSecurity;

            if (!builder.IntegratedSecurity)
            {
                txtUsuario.Text = builder.UserID;
                txtSenha.Text = builder.Password;
            }
        }
        catch
        {
            // Connection string em formato não reconhecido pelo builder: cai pro modo avançado.
            chkAvancado.Checked = true;
            txtConnString.Text = connString;
        }
    }

    private void chkWindowsAuth_CheckedChanged(object? sender, EventArgs e)
    {
        var usaSql = !chkWindowsAuth.Checked;
        txtUsuario.Enabled = usaSql;
        txtSenha.Enabled = usaSql;
    }

    private void chkAvancado_CheckedChanged(object? sender, EventArgs e)
    {
        var avancado = chkAvancado.Checked;

        lblConnString.Visible = avancado;
        txtConnString.Visible = avancado;

        txtServidor.Enabled = !avancado;
        txtBanco.Enabled = !avancado;
        chkWindowsAuth.Enabled = !avancado;
        txtUsuario.Enabled = !avancado && !chkWindowsAuth.Checked;
        txtSenha.Enabled = !avancado && !chkWindowsAuth.Checked;
    }

    private string MontarConnectionString()
    {
        if (chkAvancado.Checked)
        {
            return txtConnString.Text.Trim();
        }

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = txtServidor.Text.Trim(),
            InitialCatalog = txtBanco.Text.Trim(),
            IntegratedSecurity = chkWindowsAuth.Checked,
            TrustServerCertificate = true,
            ConnectTimeout = 8,
        };

        if (!chkWindowsAuth.Checked)
        {
            builder.UserID = txtUsuario.Text.Trim();
            builder.Password = txtSenha.Text;
        }

        return builder.ConnectionString;
    }

    private void btnTestar_Click(object? sender, EventArgs e)
    {
        var connString = MontarConnectionString();

        if (string.IsNullOrWhiteSpace(connString))
        {
            lblResultado.ForeColor = Color.FromArgb(183, 28, 28);
            lblResultado.Text = "Preencha os dados de conexão primeiro.";
            return;
        }

        Cursor = Cursors.WaitCursor;
        lblResultado.Text = "Testando...";
        Application.DoEvents();

        var ok = AuthService.TestConnection(connString, out var erro);

        Cursor = Cursors.Default;

        if (ok)
        {
            lblResultado.ForeColor = Color.FromArgb(46, 125, 50);
            lblResultado.Text = "Conexão estabelecida com sucesso!";
        }
        else
        {
            lblResultado.ForeColor = Color.FromArgb(183, 28, 28);
            lblResultado.Text = $"Falha ao conectar: {erro}";
        }
    }

    private void btnSalvar_Click(object? sender, EventArgs e)
    {
        var connString = MontarConnectionString();

        if (string.IsNullOrWhiteSpace(connString))
        {
            MessageBox.Show(this, "Preencha os dados de conexão antes de salvar.", "Dados incompletos",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        ConnectionStringVault.Save(connString);
        FoiSalvo = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancelar_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
