using RamaisManager.Security;
using RamaisManager.Services;

namespace RamaisManager;

public partial class LoginForm : Form
{
    private readonly string _connectionString;
    private readonly AuthService _authService = new();
    private readonly LoginThrottle _throttle = new();

    public OperadorAutenticado? OperadorLogado { get; private set; }

    public LoginForm(string connectionString)
    {
        InitializeComponent();
        _connectionString = connectionString;

        // Atalho oculto também disponível na tela de login, para o
        // configurar
        // a connection string antes mesmo de logar (ex: primeira instalação).
        this.KeyDown += (s, e) =>
        {
            if (e.Control && e.Shift && e.Alt && e.KeyCode == Keys.C)
            {
                AbrirConfiguracoesDeConexao();
                e.Handled = true;
            }
        };

        this.Shown += (s, e) => txtCodigo.Focus();
    }

    private void txtSenha_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            btnEntrar_Click(sender, e);
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    private void btnEntrar_Click(object? sender, EventArgs e)
    {
        lblErro.Text = "";

        if (_throttle.EstaBloqueado(out var tempoRestante))
        {
            lblErro.Text = $"Muitas tentativas incorretas. Tente novamente em {Math.Ceiling(tempoRestante.TotalMinutes)} min.";
            return;
        }

        var codigo = txtCodigo.Text.Trim();
        var senha = txtSenha.Text.Trim();

        if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(senha))
        {
            lblErro.Text = "Informe código e senha.";
            return;
        }

        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            lblErro.Text = "Conexão com o banco não configurada.\nPressione Ctrl+Shift+Alt+C.";
            return;
        }

        Cursor = Cursors.WaitCursor;
        btnEntrar.Enabled = false;
        try
        {
            var operador = _authService.Login(_connectionString, codigo, senha);

            if (operador == null)
            {
                _throttle.RegistrarFalha();
                var restantes = _throttle.TentativasRestantes;
                lblErro.Text = restantes > 0
                    ? $"Código ou senha inválidos. Tentativas restantes: {restantes}."
                    : "Código ou senha inválidos. Acesso temporariamente bloqueado.";
                txtSenha.Clear();
                txtSenha.Focus();
                return;
            }

            _throttle.RegistrarSucesso();
            OperadorLogado = operador;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (DbConnectionException ex)
        {
            lblErro.Text = ex.Message;
        }
        finally
        {
            Cursor = Cursors.Default;
            btnEntrar.Enabled = true;
        }
    }

    private void btnCancelar_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void AbrirConfiguracoesDeConexao()
    {
        using var form = new DbSettingsForm();
        form.ShowDialog(this);
        // Após salvar, é necessário reabrir o programa para recarregar a connection
        // string nesta tela (mantém o fluxo simples e evita estado parcialmente trocado).
        if (form.FoiSalvo)
        {
            MessageBox.Show(this,
                "Configuração salva. Feche e abra o programa novamente para aplicar.",
                "Configuração salva",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
