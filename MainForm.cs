using System.ComponentModel;
using RamaisManager.Config;
using RamaisManager.Models;
using RamaisManager.Services;

namespace RamaisManager;

public partial class MainForm : Form
{
    private readonly RamalXmlService _xmlService = new();
    private AppSettings _settings;
    private BindingList<Ramal> _ramais = new();
    private BindingSource _bindingSource = new();
    private bool _hasUnsavedChanges = false;
    private readonly OperadorAutenticado _operador;

    public MainForm(OperadorAutenticado operador)
    {
        InitializeComponent();

        _operador = operador;
        _settings = AppSettings.Load();

        SetupGrid();

        this.Text = $"Gerenciador de Ramais - MicroSIP — Operador: {_operador.Codigo}";
        this.KeyDown += MainForm_KeyDown;
        this.FormClosing += MainForm_FormClosing;
        this.Load += MainForm_Load;
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        UpdatePathLabel();

        if (!_settings.IsConfigured || !File.Exists(_settings.XmlPath))
        {
            MessageBox.Show(this,
                "O caminho do arquivo de ramais ainda não está configurado corretamente.\n\n" +
                "Pressione Ctrl+Shift+Alt+C para abrir as configurações e definir o caminho do arquivo.",
                "Configuração necessária",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        LoadRamais();
    }

    private void SetupGrid()
    {
        dgvRamais.AutoGenerateColumns = false;
        dgvRamais.DataSource = _bindingSource;

        dgvRamais.Columns.Clear();
        dgvRamais.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colNumber",
            HeaderText = "Ramal",
            DataPropertyName = "Number",
            FillWeight = 25,
        });
        dgvRamais.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colName",
            HeaderText = "Identificação (Nome)",
            DataPropertyName = "Name",
            FillWeight = 75,
        });

        dgvRamais.CellValueChanged += (s, e) => MarkDirty();
        dgvRamais.UserDeletedRow += (s, e) => { MarkDirty(); dgvRamais.Invalidate(); };

        // Aviso de duplicação de ramal:
        // - CellValidating: bloqueia sair da célula "Ramal" se o valor digitado
        //   já existir em outro contato, mostrando um erro junto à célula.
        // - CellFormatting: pinta em vermelho qualquer célula "Ramal" cujo número
        //   esteja duplicado em algum lugar da lista (cobre o caso de duas linhas
        //   já existentes ficarem iguais por edição indireta, reordenação, etc.).
        dgvRamais.CellValidating += dgvRamais_CellValidating;
        dgvRamais.CellFormatting += dgvRamais_CellFormatting;
        dgvRamais.CellEndEdit += (s, e) =>
        {
            dgvRamais.Invalidate();
            if (dgvRamais.Rows[e.RowIndex].DataBoundItem is Ramal ramal)
            {
                ramal.AlteradoPor = _operador.Codigo;
                ramal.AlteradoEm = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        };
        dgvRamais.DataError += (s, e) => e.ThrowException = false;
    }

    /// <summary>
    /// Impede que o usuário saia da célula "Ramal" se o número digitado
    /// já estiver em uso por outro contato da lista.
    /// </summary>
    private void dgvRamais_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        if (dgvRamais.Columns[e.ColumnIndex].Name != "colNumber") return;

        var novoValor = e.FormattedValue?.ToString()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(novoValor)) return; // erro de "vazio" já é pego na validação de salvar

        var linhaAtual = dgvRamais.Rows[e.RowIndex].DataBoundItem as Ramal;

        var duplicado = _ramais.Any(r =>
            r != linhaAtual &&
            string.Equals(r.Number.Trim(), novoValor, StringComparison.OrdinalIgnoreCase));

        if (duplicado)
        {
            dgvRamais.Rows[e.RowIndex].ErrorText =
                $"O ramal {novoValor} já está em uso por outro contato.";
            e.Cancel = true;
        }
        else
        {
            dgvRamais.Rows[e.RowIndex].ErrorText = string.Empty;
        }
    }

    /// <summary>
    /// Pinta em vermelho claro qualquer célula de número de ramal que esteja
    /// duplicada em qualquer lugar da lista, mesmo fora da edição atual.
    /// </summary>
    private void dgvRamais_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (dgvRamais.Columns[e.ColumnIndex].Name != "colNumber") return;
        if (e.RowIndex < 0 || e.RowIndex >= dgvRamais.Rows.Count) return;

        if (dgvRamais.Rows[e.RowIndex].DataBoundItem is not Ramal ramalAtual) return;

        var numero = ramalAtual.Number?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(numero)) return;

        var ocorrencias = _ramais.Count(r =>
            string.Equals(r.Number.Trim(), numero, StringComparison.OrdinalIgnoreCase));

        var cell = dgvRamais.Rows[e.RowIndex].Cells[e.ColumnIndex];

        if (ocorrencias > 1)
        {
            cell.Style.BackColor = Color.FromArgb(255, 205, 210);
            cell.Style.ForeColor = Color.FromArgb(183, 28, 28);
            cell.ToolTipText = $"Atenção: o ramal {numero} está duplicado em {ocorrencias} contatos.";
        }
        else
        {
            cell.Style.BackColor = dgvRamais.DefaultCellStyle.BackColor;
            cell.Style.ForeColor = dgvRamais.DefaultCellStyle.ForeColor;
            cell.ToolTipText = string.Empty;
        }
    }

    private void LoadRamais()
    {
        try
        {
            statusLabel.Text = "Carregando...";
            Application.DoEvents();

            var list = _xmlService.Load(_settings.XmlPath);
            _ramais = new BindingList<Ramal>(list.OrderBy(r => SafeNumber(r.Number)).ToList());
            _bindingSource.DataSource = _ramais;
            dgvRamais.DataSource = _bindingSource;

            _hasUnsavedChanges = false;
            statusLabel.Text = $"{_ramais.Count} ramal(is) carregado(s).";
            ApplyFilter();
        }
        catch (FileNotFoundException ex)
        {
            statusLabel.Text = "Arquivo não encontrado.";
            MessageBox.Show(this, ex.Message, "Arquivo não encontrado",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (RamalFileLockedException ex)
        {
            statusLabel.Text = "Arquivo em uso.";
            MessageBox.Show(this, ex.Message, "Arquivo em uso",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            statusLabel.Text = "Erro ao carregar.";
            MessageBox.Show(this, $"Erro inesperado ao carregar o arquivo:\n\n{ex.Message}",
                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static int SafeNumber(string number)
        => int.TryParse(number, out var n) ? n : int.MaxValue;

    private void MarkDirty()
    {
        _hasUnsavedChanges = true;
        statusLabel.Text = "Alterações não salvas.";
    }

    private void btnAdd_Click(object? sender, EventArgs e)
    {
        var novo = new Ramal
        {
            Name = "NOVO RAMAL",
            Number = SugerirProximoNumero(),
            AlteradoPor = _operador.Codigo,
            AlteradoEm = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        };
        _ramais.Add(novo);
        MarkDirty();

        dgvRamais.ClearSelection();
        var rowIndex = _ramais.IndexOf(novo);
        if (rowIndex >= 0 && rowIndex < dgvRamais.Rows.Count)
        {
            dgvRamais.Rows[rowIndex].Selected = true;
            dgvRamais.FirstDisplayedScrollingRowIndex = rowIndex;
            dgvRamais.CurrentCell = dgvRamais.Rows[rowIndex].Cells["colName"];
            dgvRamais.BeginEdit(true);
        }
    }

    private string SugerirProximoNumero()
    {
        var maior = _ramais
            .Select(r => SafeNumber(r.Number))
            .Where(n => n != int.MaxValue)
            .DefaultIfEmpty(1000)
            .Max();
        return (maior + 1).ToString();
    }

    private void btnRemove_Click(object? sender, EventArgs e)
    {
        if (dgvRamais.CurrentRow?.DataBoundItem is not Ramal selecionado)
        {
            MessageBox.Show(this, "Selecione um ramal para remover.", "Nenhum ramal selecionado",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show(this,
            $"Remover o ramal \"{selecionado.Number} - {selecionado.Name}\"?",
            "Confirmar remoção",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            _ramais.Remove(selecionado);
            MarkDirty();
        }
    }

    private void btnReload_Click(object? sender, EventArgs e)
    {
        if (_hasUnsavedChanges)
        {
            var confirm = MessageBox.Show(this,
                "Existem alterações não salvas que serão perdidas. Deseja recarregar mesmo assim?",
                "Alterações não salvas",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;
        }

        LoadRamais();
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        SaveRamais();
    }

    private void SaveRamais(bool forceOverwrite = false)
    {
        // Validações básicas antes de salvar
        var erros = ValidarRamais();
        if (erros.Count > 0)
        {
            MessageBox.Show(this,
                "Corrija os seguintes problemas antes de salvar:\n\n" + string.Join("\n", erros),
                "Não foi possível salvar",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            statusLabel.Text = "Salvando...";
            Application.DoEvents();

            _xmlService.Save(_settings.XmlPath, _ramais.ToList(), forceOverwrite);

            _hasUnsavedChanges = false;
            statusLabel.Text = $"Salvo com sucesso às {DateTime.Now:HH:mm:ss}.";
        }
        catch (RamalConflictException ex)
        {
            var retry = MessageBox.Show(this,
                ex.Message + "\n\nDeseja salvar mesmo assim, sobrescrevendo a alteração de outra pessoa?",
                "Conflito detectado",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (retry == DialogResult.Yes)
            {
                SaveRamais(forceOverwrite: true);
            }
            else
            {
                statusLabel.Text = "Salvamento cancelado. Recarregue a lista para ver as mudanças mais recentes.";
            }
        }
        catch (RamalFileLockedException ex)
        {
            statusLabel.Text = "Arquivo em uso.";
            MessageBox.Show(this, ex.Message, "Arquivo em uso",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            statusLabel.Text = "Erro ao salvar.";
            MessageBox.Show(this, $"Erro inesperado ao salvar o arquivo:\n\n{ex.Message}",
                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private List<string> ValidarRamais()
    {
        var erros = new List<string>();

        var semNumero = _ramais.Where(r => string.IsNullOrWhiteSpace(r.Number)).ToList();
        if (semNumero.Count > 0)
            erros.Add($"• {semNumero.Count} ramal(is) sem número preenchido.");

        var semNome = _ramais.Where(r => string.IsNullOrWhiteSpace(r.Name)).ToList();
        if (semNome.Count > 0)
            erros.Add($"• {semNome.Count} ramal(is) sem identificação preenchida.");

        var duplicados = _ramais
            .Where(r => !string.IsNullOrWhiteSpace(r.Number))
            .GroupBy(r => r.Number.Trim())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicados.Count > 0)
            erros.Add($"• Números de ramal duplicados: {string.Join(", ", duplicados)}");

        return erros;
    }

    private void txtSearch_TextChanged(object? sender, EventArgs e) => ApplyFilter();

    private void ApplyFilter()
    {
        var termo = txtSearch.Text.Trim();

        if (string.IsNullOrEmpty(termo))
        {
            _bindingSource.DataSource = _ramais;
            return;
        }

        var filtrados = _ramais
            .Where(r =>
                r.Name.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                r.Number.Contains(termo, StringComparison.OrdinalIgnoreCase))
            .ToList();

        _bindingSource.DataSource = new BindingList<Ramal>(filtrados);
    }

    private void UpdatePathLabel()
    {
        lblPath.Text = $"Arquivo: {_settings.XmlPath}";
    }

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        // Atalho oculto para configurações do XML: Ctrl+Shift+Alt+C
        if (e.Control && e.Shift && e.Alt && e.KeyCode == Keys.C)
        {
            AbrirConfiguracoes();
            e.Handled = true;
        }

        // Atalho oculto para configurações do banco de dados: Ctrl+Shift+Alt+B
        if (e.Control && e.Shift && e.Alt && e.KeyCode == Keys.B)
        {
            using var dbForm = new DbSettingsForm();
            dbForm.ShowDialog(this);
            e.Handled = true;
        }
    }

    private void AbrirConfiguracoes()
    {
        using var form = new SettingsForm(_settings);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            _settings = form.Settings;
            _settings.Save();
            UpdatePathLabel();
            LoadRamais();
        }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_hasUnsavedChanges)
        {
            var confirm = MessageBox.Show(this,
                "Existem alterações não salvas. Deseja sair mesmo assim?",
                "Alterações não salvas",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }
    }
}
