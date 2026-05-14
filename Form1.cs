using System.ComponentModel;

namespace InternshipApplicationTracker;

/// <summary>
/// Main window of the Internship Application Tracker.
/// UI is built in code (no designer file) so the whole form is readable in one place.
/// </summary>
public class Form1 : Form
{
    // ────────────────────────────────────────────────────────────────
    //  STATE
    // ────────────────────────────────────────────────────────────────

    /// <summary>All entries in memory. The source of truth.</summary>
    private readonly List<ApplicationEntry> _applications = new();

    /// <summary>The entry currently selected in the grid, or null.</summary>
    private ApplicationEntry? _selectedEntry;

    // ────────────────────────────────────────────────────────────────
    //  CONTROLS
    // ────────────────────────────────────────────────────────────────

    private ComboBox _cmbFilter = null!;
    private TextBox  _txtCompany = null!;
    private TextBox  _txtPosition = null!;
    private DateTimePicker _dtpDate = null!;
    private ComboBox _cmbStatus = null!;
    private TextBox  _txtNotes = null!;
    private Button   _btnAdd = null!;
    private Button   _btnUpdate = null!;
    private Button   _btnDelete = null!;
    private Button   _btnClear = null!;
    private DataGridView _dgv = null!;
    private StatusStrip _statusStrip = null!;
    private ToolStripStatusLabel _lblStatusBar = null!;

    // ────────────────────────────────────────────────────────────────
    //  CONSTRUCTOR
    // ────────────────────────────────────────────────────────────────

    public Form1()
    {
        BuildUi();
        WireEvents();
        LoadFromDisk();
        RefreshGrid();
        UpdateStatusBar();
    }

    // ────────────────────────────────────────────────────────────────
    //  UI BUILD — declarative form layout in code
    // ────────────────────────────────────────────────────────────────

    private void BuildUi()
    {
        Text = "Internship Application Tracker";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(960, 560);
        MinimumSize = new Size(820, 480);
        Font = new Font("Segoe UI", 9.0F);

        // ─── Top: filter row ────────────────────────────────────────
        var lblFilter = new Label
        {
            Text = "Filter by Status:",
            AutoSize = true,
            Location = new Point(14, 16)
        };

        _cmbFilter = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(120, 12),
            Width = 160
        };
        _cmbFilter.Items.AddRange(ApplicationStatus.FilterChoices);
        _cmbFilter.SelectedItem = ApplicationStatus.All;

        // ─── Left: input form group ─────────────────────────────────
        var grpForm = new GroupBox
        {
            Text = "Application Details",
            Location = new Point(14, 46),
            Size = new Size(360, 460),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
        };

        int labelLeft = 14;
        int fieldLeft = 14;
        int width = 320;
        int y = 28;
        int gap = 8;

        // Company Name
        var lblCompany = new Label { Text = "Company Name", Location = new Point(labelLeft, y), AutoSize = true };
        y += 20;
        _txtCompany = new TextBox { Location = new Point(fieldLeft, y), Width = width };
        y += _txtCompany.Height + gap + 4;

        // Position
        var lblPosition = new Label { Text = "Position", Location = new Point(labelLeft, y), AutoSize = true };
        y += 20;
        _txtPosition = new TextBox { Location = new Point(fieldLeft, y), Width = width };
        y += _txtPosition.Height + gap + 4;

        // Application Date
        var lblDate = new Label { Text = "Application Date", Location = new Point(labelLeft, y), AutoSize = true };
        y += 20;
        _dtpDate = new DateTimePicker
        {
            Location = new Point(fieldLeft, y),
            Width = width,
            Format = DateTimePickerFormat.Short
        };
        y += _dtpDate.Height + gap + 4;

        // Status
        var lblStatus = new Label { Text = "Status", Location = new Point(labelLeft, y), AutoSize = true };
        y += 20;
        _cmbStatus = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(fieldLeft, y),
            Width = width
        };
        _cmbStatus.Items.AddRange(ApplicationStatus.Selectable);
        _cmbStatus.SelectedItem = ApplicationStatus.Applied;
        y += _cmbStatus.Height + gap + 4;

        // Notes
        var lblNotes = new Label { Text = "Notes", Location = new Point(labelLeft, y), AutoSize = true };
        y += 20;
        _txtNotes = new TextBox
        {
            Location = new Point(fieldLeft, y),
            Width = width,
            Height = 80,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            AcceptsReturn = true
        };
        y += _txtNotes.Height + gap + 12;

        // Buttons row 1: Add / Update / Delete
        _btnAdd = new Button
        {
            Text = "Add",
            Location = new Point(fieldLeft, y),
            Width = 100,
            Height = 30
        };

        _btnUpdate = new Button
        {
            Text = "Update Selected",
            Location = new Point(fieldLeft + 108, y),
            Width = 130,
            Height = 30,
            Enabled = false
        };

        _btnDelete = new Button
        {
            Text = "Delete",
            Location = new Point(fieldLeft + 246, y),
            Width = 74,
            Height = 30,
            Enabled = false
        };
        y += 36;

        // Buttons row 2: Clear Form
        _btnClear = new Button
        {
            Text = "Clear Form",
            Location = new Point(fieldLeft, y),
            Width = 100,
            Height = 28
        };

        grpForm.Controls.AddRange(new Control[]
        {
            lblCompany, _txtCompany,
            lblPosition, _txtPosition,
            lblDate, _dtpDate,
            lblStatus, _cmbStatus,
            lblNotes, _txtNotes,
            _btnAdd, _btnUpdate, _btnDelete,
            _btnClear
        });

        // ─── Right: data grid ──────────────────────────────────────
        _dgv = new DataGridView
        {
            Location = new Point(384, 46),
            Size = new Size(560, 460),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            BackgroundColor = SystemColors.Window,
            BorderStyle = BorderStyle.FixedSingle,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        _dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Company",
            DataPropertyName = nameof(ApplicationEntry.CompanyName),
            FillWeight = 25
        });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Position",
            DataPropertyName = nameof(ApplicationEntry.Position),
            FillWeight = 25
        });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Date",
            DataPropertyName = nameof(ApplicationEntry.ApplicationDate),
            FillWeight = 16,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" }
        });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Status",
            DataPropertyName = nameof(ApplicationEntry.Status),
            FillWeight = 14
        });
        _dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Notes",
            DataPropertyName = nameof(ApplicationEntry.Notes),
            FillWeight = 20
        });

        // ─── Bottom: status strip ──────────────────────────────────
        _lblStatusBar = new ToolStripStatusLabel
        {
            Text = "Ready.",
            Spring = true,
            TextAlign = ContentAlignment.MiddleLeft
        };
        _statusStrip = new StatusStrip();
        _statusStrip.Items.Add(_lblStatusBar);

        Controls.Add(lblFilter);
        Controls.Add(_cmbFilter);
        Controls.Add(grpForm);
        Controls.Add(_dgv);
        Controls.Add(_statusStrip);
    }

    // ────────────────────────────────────────────────────────────────
    //  EVENT WIRING — single place to see what triggers what
    // ────────────────────────────────────────────────────────────────

    private void WireEvents()
    {
        _btnAdd.Click       += OnAddClicked;
        _btnUpdate.Click    += OnUpdateClicked;
        _btnDelete.Click    += OnDeleteClicked;
        _btnClear.Click     += OnClearClicked;
        _cmbFilter.SelectedIndexChanged += OnFilterChanged;
        _dgv.SelectionChanged          += OnGridSelectionChanged;
    }

    // ────────────────────────────────────────────────────────────────
    //  EVENT HANDLERS
    // ────────────────────────────────────────────────────────────────

    private void OnAddClicked(object? sender, EventArgs e)
    {
        if (!ValidateInputs(out string? error))
        {
            MessageBox.Show(error, "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var entry = new ApplicationEntry
        {
            CompanyName     = _txtCompany.Text.Trim(),
            Position        = _txtPosition.Text.Trim(),
            ApplicationDate = _dtpDate.Value.Date,
            Status          = _cmbStatus.SelectedItem?.ToString() ?? ApplicationStatus.Applied,
            Notes           = _txtNotes.Text.Trim()
        };

        _applications.Add(entry);
        SaveToDisk();
        RefreshGrid();
        ClearForm();
        UpdateStatusBar($"Added: {entry.CompanyName} — {entry.Position}");
    }

    private void OnUpdateClicked(object? sender, EventArgs e)
    {
        if (_selectedEntry is null)
        {
            MessageBox.Show("Select a row first.", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!ValidateInputs(out string? error))
        {
            MessageBox.Show(error, "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Mutate the existing entry (the grid is bound to the same object).
        _selectedEntry.CompanyName     = _txtCompany.Text.Trim();
        _selectedEntry.Position        = _txtPosition.Text.Trim();
        _selectedEntry.ApplicationDate = _dtpDate.Value.Date;
        _selectedEntry.Status          = _cmbStatus.SelectedItem?.ToString() ?? ApplicationStatus.Applied;
        _selectedEntry.Notes           = _txtNotes.Text.Trim();

        SaveToDisk();
        RefreshGrid();
        UpdateStatusBar($"Updated: {_selectedEntry.CompanyName} — {_selectedEntry.Position}");
    }

    private void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_selectedEntry is null)
        {
            MessageBox.Show("Select a row first.", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show(
            $"Delete the application for \"{_selectedEntry.CompanyName} — {_selectedEntry.Position}\"?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        var removed = _selectedEntry;
        _applications.Remove(removed);
        _selectedEntry = null;

        SaveToDisk();
        RefreshGrid();
        ClearForm();
        UpdateStatusBar($"Deleted: {removed.CompanyName} — {removed.Position}");
    }

    private void OnClearClicked(object? sender, EventArgs e)
    {
        ClearForm();
        _dgv.ClearSelection();
        UpdateStatusBar("Form cleared.");
    }

    private void OnFilterChanged(object? sender, EventArgs e)
    {
        RefreshGrid();
    }

    private void OnGridSelectionChanged(object? sender, EventArgs e)
    {
        // Pull the actual ApplicationEntry behind the selected row.
        if (_dgv.SelectedRows.Count == 0)
        {
            _selectedEntry = null;
            _btnUpdate.Enabled = false;
            _btnDelete.Enabled = false;
            return;
        }

        var bound = _dgv.SelectedRows[0].DataBoundItem as ApplicationEntry;
        if (bound is null)
        {
            _selectedEntry = null;
            _btnUpdate.Enabled = false;
            _btnDelete.Enabled = false;
            return;
        }

        _selectedEntry = bound;
        _btnUpdate.Enabled = true;
        _btnDelete.Enabled = true;

        // Populate the form with the selected entry's values
        _txtCompany.Text  = bound.CompanyName;
        _txtPosition.Text = bound.Position;
        _dtpDate.Value    = bound.ApplicationDate.Date;
        _cmbStatus.SelectedItem = ApplicationStatus.Selectable.Contains(bound.Status)
            ? bound.Status
            : ApplicationStatus.Applied;
        _txtNotes.Text    = bound.Notes;
    }

    // ────────────────────────────────────────────────────────────────
    //  HELPERS
    // ────────────────────────────────────────────────────────────────

    private bool ValidateInputs(out string? error)
    {
        if (string.IsNullOrWhiteSpace(_txtCompany.Text))
        {
            error = "Company name is required.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(_txtPosition.Text))
        {
            error = "Position is required.";
            return false;
        }
        if (_cmbStatus.SelectedItem is null)
        {
            error = "Pick a status.";
            return false;
        }
        error = null;
        return true;
    }

    private void ClearForm()
    {
        _txtCompany.Text = string.Empty;
        _txtPosition.Text = string.Empty;
        _dtpDate.Value = DateTime.Today;
        _cmbStatus.SelectedItem = ApplicationStatus.Applied;
        _txtNotes.Text = string.Empty;
        _selectedEntry = null;
        _btnUpdate.Enabled = false;
        _btnDelete.Enabled = false;
    }

    /// <summary>
    /// Rebinds the grid to either the full list or a status-filtered view.
    /// The grid holds references to the same ApplicationEntry objects,
    /// so editing through "Update Selected" mutates the real entry.
    /// </summary>
    private void RefreshGrid()
    {
        string filter = _cmbFilter.SelectedItem?.ToString() ?? ApplicationStatus.All;

        List<ApplicationEntry> view = filter == ApplicationStatus.All
            ? _applications
            : _applications.Where(a => a.Status == filter).ToList();

        // Order by date descending — newest at the top is the useful default.
        view = view.OrderByDescending(a => a.ApplicationDate).ToList();

        // Clear binding first to avoid stale references and flicker.
        _dgv.DataSource = null;
        _dgv.DataSource = new BindingList<ApplicationEntry>(view);

        UpdateStatusBar();
    }

    private void LoadFromDisk()
    {
        var entries = ApplicationStorage.Load(out string? error);
        _applications.Clear();
        _applications.AddRange(entries);

        if (error is not null)
        {
            MessageBox.Show(error, "Load warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void SaveToDisk()
    {
        if (!ApplicationStorage.Save(_applications, out string? error))
        {
            MessageBox.Show(error, "Save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdateStatusBar(string? message = null)
    {
        int total = _applications.Count;
        string filter = _cmbFilter.SelectedItem?.ToString() ?? ApplicationStatus.All;
        int shown = filter == ApplicationStatus.All
            ? total
            : _applications.Count(a => a.Status == filter);

        string baseInfo = filter == ApplicationStatus.All
            ? $"{total} application{(total == 1 ? "" : "s")}"
            : $"{shown} of {total} (filter: {filter})";

        _lblStatusBar.Text = message is null
            ? $"{baseInfo}  ·  file: {ApplicationStorage.FileName}"
            : $"{baseInfo}  ·  {message}";
    }
}
