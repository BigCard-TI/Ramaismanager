namespace RamaisManager;

partial class LoginForm
{
    private System.ComponentModel.IContainer components = null!;
    private Label lblTitle = null!;
    private Label lblCodigo = null!;
    private TextBox txtCodigo = null!;
    private Label lblSenha = null!;
    private TextBox txtSenha = null!;
    private Button btnEntrar = null!;
    private Button btnCancelar = null!;
    private Label lblErro = null!;
    private Label lblHintConfig = null!;

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
        lblCodigo = new Label();
        txtCodigo = new TextBox();
        lblSenha = new Label();
        txtSenha = new TextBox();
        btnEntrar = new Button();
        btnCancelar = new Button();
        lblErro = new Label();
        lblHintConfig = new Label();

        SuspendLayout();

        // lblTitle
        lblTitle.Text = "Gerenciador de Ramais";
        lblTitle.Font = new Font(lblTitle.Font.FontFamily, 13, FontStyle.Bold);
        lblTitle.Location = new Point(20, 20);
        lblTitle.AutoSize = true;

        // lblCodigo
        lblCodigo.Text = "Código do operador:";
        lblCodigo.Location = new Point(20, 65);
        lblCodigo.AutoSize = true;

        // txtCodigo
        txtCodigo.Location = new Point(20, 88);
        txtCodigo.Width = 280;
        txtCodigo.Font = new Font(txtCodigo.Font.FontFamily, 11);

        // lblSenha
        lblSenha.Text = "Senha:";
        lblSenha.Location = new Point(20, 122);
        lblSenha.AutoSize = true;

        // txtSenha
        txtSenha.Location = new Point(20, 145);
        txtSenha.Width = 280;
        txtSenha.Font = new Font(txtSenha.Font.FontFamily, 11);
        txtSenha.UseSystemPasswordChar = true;
        txtSenha.MaxLength = 6;
        txtSenha.KeyDown += txtSenha_KeyDown;

        // lblErro
        lblErro.Location = new Point(20, 178);
        lblErro.Size = new Size(280, 40);
        lblErro.ForeColor = Color.FromArgb(183, 28, 28);
        lblErro.Text = "";

        // btnEntrar
        btnEntrar.Text = "Entrar";
        btnEntrar.Location = new Point(20, 225);
        btnEntrar.Size = new Size(135, 32);
        btnEntrar.BackColor = Color.FromArgb(46, 125, 50);
        btnEntrar.ForeColor = Color.White;
        btnEntrar.FlatStyle = FlatStyle.Flat;
        btnEntrar.Click += btnEntrar_Click;

        // btnCancelar
        btnCancelar.Text = "Cancelar";
        btnCancelar.Location = new Point(165, 225);
        btnCancelar.Size = new Size(135, 32);
        btnCancelar.Click += btnCancelar_Click;

        // lblHintConfig
        lblHintConfig.ForeColor = Color.Gray;
        lblHintConfig.Font = new Font(lblHintConfig.Font.FontFamily, 8);
        lblHintConfig.Location = new Point(20, 268);
        lblHintConfig.AutoSize = true;

        // LoginForm
        ClientSize = new Size(322, 300);
        Controls.Add(lblTitle);
        Controls.Add(lblCodigo);
        Controls.Add(txtCodigo);
        Controls.Add(lblSenha);
        Controls.Add(txtSenha);
        Controls.Add(lblErro);
        Controls.Add(btnEntrar);
        Controls.Add(btnCancelar);
        Controls.Add(lblHintConfig);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Login - Acesso à Lista de Ramais";
        AcceptButton = btnEntrar;
        CancelButton = btnCancelar;
        KeyPreview = true;

        ResumeLayout(false);
        PerformLayout();
    }
}
