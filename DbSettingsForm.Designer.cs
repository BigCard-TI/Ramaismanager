namespace RamaisManager;

partial class DbSettingsForm
{
    private System.ComponentModel.IContainer components = null!;
    private Label lblTitle = null!;
    private Label lblServidor = null!;
    private TextBox txtServidor = null!;
    private Label lblBanco = null!;
    private TextBox txtBanco = null!;
    private CheckBox chkWindowsAuth = null!;
    private Label lblUsuario = null!;
    private TextBox txtUsuario = null!;
    private Label lblSenha = null!;
    private TextBox txtSenha = null!;
    private CheckBox chkAvancado = null!;
    private Label lblConnString = null!;
    private TextBox txtConnString = null!;
    private Button btnTestar = null!;
    private Button btnSalvar = null!;
    private Button btnCancelar = null!;
    private Label lblResultado = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblTitle = new Label();
        lblServidor = new Label();
        txtServidor = new TextBox();
        lblBanco = new Label();
        txtBanco = new TextBox();
        chkWindowsAuth = new CheckBox();
        lblUsuario = new Label();
        txtUsuario = new TextBox();
        lblSenha = new Label();
        txtSenha = new TextBox();
        chkAvancado = new CheckBox();
        lblConnString = new Label();
        txtConnString = new TextBox();
        btnTestar = new Button();
        btnSalvar = new Button();
        btnCancelar = new Button();
        lblResultado = new Label();

        SuspendLayout();

        // lblTitle
        lblTitle.Text = "Configuração de Conexão com o Banco (acesso restrito)";
        lblTitle.Font = new Font(lblTitle.Font.FontFamily, 10.5F, FontStyle.Bold);
        lblTitle.Location = new Point(15, 15);
        lblTitle.AutoSize = true;

        // lblServidor
        lblServidor.Text = "Servidor:";
        lblServidor.Location = new Point(15, 55);
        lblServidor.AutoSize = true;

        // txtServidor
        txtServidor.Location = new Point(15, 75);
        txtServidor.Width = 460;
        txtServidor.PlaceholderText = @"NOMESERVIDOR\SQLEXPRESS ou endereço IP";

        // lblBanco
        lblBanco.Text = "Banco de dados:";
        lblBanco.Location = new Point(15, 105);
        lblBanco.AutoSize = true;

        // txtBanco
        txtBanco.Location = new Point(15, 125);
        txtBanco.Width = 460;
        txtBanco.PlaceholderText = "NomeDoBanco";

        // chkWindowsAuth
        chkWindowsAuth.Text = "Usar autenticação do Windows (Integrated Security)";
        chkWindowsAuth.Location = new Point(15, 155);
        chkWindowsAuth.AutoSize = true;
        chkWindowsAuth.CheckedChanged += chkWindowsAuth_CheckedChanged;

        // lblUsuario
        lblUsuario.Text = "Usuário SQL:";
        lblUsuario.Location = new Point(15, 182);
        lblUsuario.AutoSize = true;

        // txtUsuario
        txtUsuario.Location = new Point(15, 202);
        txtUsuario.Width = 225;

        // lblSenha
        lblSenha.Text = "Senha SQL:";
        lblSenha.Location = new Point(250, 182);
        lblSenha.AutoSize = true;

        // txtSenha
        txtSenha.Location = new Point(250, 202);
        txtSenha.Width = 225;
        txtSenha.UseSystemPasswordChar = true;

        // chkAvancado
        chkAvancado.Text = "Modo avançado (informar connection string manualmente)";
        chkAvancado.Location = new Point(15, 235);
        chkAvancado.AutoSize = true;
        chkAvancado.CheckedChanged += chkAvancado_CheckedChanged;

        // lblConnString
        lblConnString.Text = "Connection string:";
        lblConnString.Location = new Point(15, 262);
        lblConnString.AutoSize = true;
        lblConnString.Visible = false;

        // txtConnString
        txtConnString.Location = new Point(15, 282);
        txtConnString.Width = 460;
        txtConnString.Visible = false;
        txtConnString.PlaceholderText = "Server=...;Database=...;User Id=...;Password=...;";

        // lblResultado
        lblResultado.Location = new Point(15, 312);
        lblResultado.Size = new Size(460, 35);
        lblResultado.Text = "";

        // btnTestar
        btnTestar.Text = "Testar conexão";
        btnTestar.Location = new Point(15, 355);
        btnTestar.Size = new Size(130, 32);
        btnTestar.Click += btnTestar_Click;

        // btnSalvar
        btnSalvar.Text = "Salvar";
        btnSalvar.Location = new Point(290, 355);
        btnSalvar.Size = new Size(95, 32);
        btnSalvar.BackColor = Color.FromArgb(46, 125, 50);
        btnSalvar.ForeColor = Color.White;
        btnSalvar.FlatStyle = FlatStyle.Flat;
        btnSalvar.Click += btnSalvar_Click;

        // btnCancelar
        btnCancelar.Text = "Cancelar";
        btnCancelar.Location = new Point(390, 355);
        btnCancelar.Size = new Size(90, 32);
        btnCancelar.Click += btnCancelar_Click;

        // DbSettingsForm
        ClientSize = new Size(495, 405);
        Controls.Add(lblTitle);
        Controls.Add(lblServidor);
        Controls.Add(txtServidor);
        Controls.Add(lblBanco);
        Controls.Add(txtBanco);
        Controls.Add(chkWindowsAuth);
        Controls.Add(lblUsuario);
        Controls.Add(txtUsuario);
        Controls.Add(lblSenha);
        Controls.Add(txtSenha);
        Controls.Add(chkAvancado);
        Controls.Add(lblConnString);
        Controls.Add(txtConnString);
        Controls.Add(lblResultado);
        Controls.Add(btnTestar);
        Controls.Add(btnSalvar);
        Controls.Add(btnCancelar);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Configuração de Banco de Dados";

        ResumeLayout(false);
        PerformLayout();
    }
}
