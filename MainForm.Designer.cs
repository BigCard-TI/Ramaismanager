namespace RamaisManager;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null!;
    private DataGridView dgvRamais = null!;
    private Button btnAdd = null!;
    private Button btnRemove = null!;
    private Button btnSave = null!;
    private Button btnReload = null!;
    private TextBox txtSearch = null!;
    private Label lblSearch = null!;
    private Label lblStatus = null!;
    private Label lblPath = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel statusLabel = null!;

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
        dgvRamais = new DataGridView();
        btnAdd = new Button();
        btnRemove = new Button();
        btnSave = new Button();
        btnReload = new Button();
        txtSearch = new TextBox();
        lblSearch = new Label();
        lblPath = new Label();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();

        ((System.ComponentModel.ISupportInitialize)dgvRamais).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();

        // lblSearch
        lblSearch.AutoSize = true;
        lblSearch.Location = new Point(12, 15);
        lblSearch.Text = "Buscar:";

        // txtSearch
        txtSearch.Location = new Point(65, 12);
        txtSearch.Width = 280;
        txtSearch.PlaceholderText = "Nome ou número do ramal...";
        txtSearch.TextChanged += txtSearch_TextChanged;

        // btnAdd
        btnAdd.Text = "Adicionar Ramal";
        btnAdd.Location = new Point(360, 10);
        btnAdd.Size = new Size(130, 30);
        btnAdd.Click += btnAdd_Click;

        // btnRemove
        btnRemove.Text = "Remover Selecionado";
        btnRemove.Location = new Point(500, 10);
        btnRemove.Size = new Size(150, 30);
        btnRemove.Click += btnRemove_Click;

        // btnReload
        btnReload.Text = "Recarregar";
        btnReload.Location = new Point(660, 10);
        btnReload.Size = new Size(110, 30);
        btnReload.Click += btnReload_Click;

        // btnSave
        btnSave.Text = "Salvar Alterações";
        btnSave.Location = new Point(780, 10);
        btnSave.Size = new Size(150, 30);
        btnSave.BackColor = Color.FromArgb(46, 125, 50);
        btnSave.ForeColor = Color.White;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Click += btnSave_Click;

        // dgvRamais
        dgvRamais.Location = new Point(12, 50);
        dgvRamais.Size = new Size(918, 480);
        dgvRamais.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvRamais.AllowUserToAddRows = false;
        dgvRamais.AllowUserToDeleteRows = false;
        dgvRamais.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvRamais.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvRamais.MultiSelect = false;
        dgvRamais.RowHeadersVisible = false;
        dgvRamais.BackgroundColor = Color.White;
        dgvRamais.BorderStyle = BorderStyle.Fixed3D;
        dgvRamais.AllowUserToOrderColumns = false;
        dgvRamais.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
        dgvRamais.ShowCellToolTips = true;
        dgvRamais.ShowRowErrors = true;
        dgvRamais.ShowCellErrors = true;

        // lblPath
        lblPath.AutoSize = true;
        lblPath.Location = new Point(12, 540);
        lblPath.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblPath.ForeColor = Color.Gray;
        lblPath.Font = new Font(lblPath.Font.FontFamily, 8);
        lblPath.Text = "Carregando configuração...";

        // statusStrip
        statusLabel.Text = "Pronto";
        statusStrip.Items.Add(statusLabel);
        statusStrip.Dock = DockStyle.Bottom;

        // MainForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(942, 593);
        Controls.Add(lblSearch);
        Controls.Add(txtSearch);
        Controls.Add(btnAdd);
        Controls.Add(btnRemove);
        Controls.Add(btnReload);
        Controls.Add(btnSave);
        Controls.Add(dgvRamais);
        Controls.Add(lblPath);
        Controls.Add(statusStrip);
        MinimumSize = new Size(700, 400);
        StartPosition = FormStartPosition.CenterScreen;
        KeyPreview = true;

        ((System.ComponentModel.ISupportInitialize)dgvRamais).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
