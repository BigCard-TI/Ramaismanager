namespace RamaisManager;

partial class SettingsForm
{
    private System.ComponentModel.IContainer components = null!;
    private Label lblTitle = null!;
    private Label lblXmlPath = null!;
    private TextBox txtXmlPath = null!;
    private Button btnBrowse = null!;
    private Button btnTest = null!;
    private Button btnSave = null!;
    private Button btnCancel = null!;
    private Label lblHint = null!;

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
        lblXmlPath = new Label();
        txtXmlPath = new TextBox();
        btnBrowse = new Button();
        btnTest = new Button();
        btnSave = new Button();
        btnCancel = new Button();
        lblHint = new Label();

        SuspendLayout();

        // lblTitle
        lblTitle.Text = "Configurações (acesso restrito)";
        lblTitle.Font = new Font(lblTitle.Font.FontFamily, 11, FontStyle.Bold);
        lblTitle.Location = new Point(15, 15);
        lblTitle.AutoSize = true;

        // lblXmlPath
        lblXmlPath.Text = "Caminho do arquivo Contacts.xml (rede):";
        lblXmlPath.Location = new Point(15, 55);
        lblXmlPath.AutoSize = true;

        // txtXmlPath
        txtXmlPath.Location = new Point(15, 78);
        txtXmlPath.Width = 430;
        txtXmlPath.PlaceholderText = @"\\servidor\Interno\MicroSIP\Contacts.xml";

        // btnBrowse
        btnBrowse.Text = "...";
        btnBrowse.Location = new Point(450, 76);
        btnBrowse.Size = new Size(35, 25);
        btnBrowse.Click += btnBrowse_Click;

        // lblHint
        lblHint.Text = "Dica: pode ser um caminho UNC (\\\\servidor\\pasta\\arquivo.xml)\nou uma unidade de rede mapeada (Z:\\pasta\\arquivo.xml).";
        lblHint.ForeColor = Color.Gray;
        lblHint.Font = new Font(lblHint.Font.FontFamily, 8);
        lblHint.Location = new Point(15, 108);
        lblHint.AutoSize = true;

        // btnTest
        btnTest.Text = "Testar caminho";
        btnTest.Location = new Point(15, 150);
        btnTest.Size = new Size(120, 30);
        btnTest.Click += btnTest_Click;

        // btnSave
        btnSave.Text = "Salvar";
        btnSave.Location = new Point(290, 150);
        btnSave.Size = new Size(95, 30);
        btnSave.BackColor = Color.FromArgb(46, 125, 50);
        btnSave.ForeColor = Color.White;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Click += btnSave_Click;

        // btnCancel
        btnCancel.Text = "Cancelar";
        btnCancel.Location = new Point(390, 150);
        btnCancel.Size = new Size(95, 30);
        btnCancel.Click += btnCancel_Click;

        // SettingsForm
        ClientSize = new Size(498, 198);
        Controls.Add(lblTitle);
        Controls.Add(lblXmlPath);
        Controls.Add(txtXmlPath);
        Controls.Add(btnBrowse);
        Controls.Add(lblHint);
        Controls.Add(btnTest);
        Controls.Add(btnSave);
        Controls.Add(btnCancel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Configurações - Gerenciador de Ramais";

        ResumeLayout(false);
        PerformLayout();
    }
}
