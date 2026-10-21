using PCBuilder.Data;
using PCBuilder.Models;

namespace PCBuilder.Forms;

/// <summary>Admin CRUD screen for troubleshooting results and their recovery instructions.</summary>
public sealed class TroubleshootingManagementForm : Form
{
    private readonly AppUser _user;
    private readonly TroubleshootingRepository _repository = new();
    private readonly TextBox _search = new();
    private readonly ComboBox _category = new();
    private readonly DataGridView _grid = new();
    private readonly Label _summary = new();
    private readonly Button _searchButton = new();
    private readonly Button _addButton = new();
    private readonly Button _editButton = new();
    private readonly Button _deleteButton = new();
    private readonly Button _refreshButton = new();

    public TroubleshootingManagementForm(AppUser user)
    {
        _user = user;
        if (!_user.IsAdmin)
            throw new UnauthorizedAccessException("Chỉ tài khoản admin mới được quản lý sự cố.");

        InitializeLayout();
        try
        {
            var categories = _repository.GetCategories();
            categories.Insert(0, new TroubleshootingCategory(0, "Tất cả nhóm"));
            _category.DataSource = categories;
            _category.DisplayMember = nameof(TroubleshootingCategory.Name);
            _category.ValueMember = nameof(TroubleshootingCategory.NodeID);
            _category.SelectedIndexChanged += (_, _) => LoadData();
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Không tải được danh sách sự cố: " + ex.Message,
                "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void InitializeLayout()
    {
        Text = "Quản lý sự cố";
        Dock = DockStyle.Fill;
        BackColor = UiTheme.Background;
        Font = UiTheme.BodyFont;
        MinimumSize = new Size(760, 500);

        var heading = new Label
        {
            Text = "Quản lý sự cố và cách khắc phục",
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(4, 8, 0, 0),
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = UiTheme.Text
        };

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 54,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(0, 6, 0, 4),
            BackColor = UiTheme.Background
        };

        _search.Width = 245;
        _search.PlaceholderText = "Tìm tên lỗi, kết luận, cách khắc phục";
        _search.Margin = new Padding(3, 4, 8, 4);
        _search.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; LoadData(); } };

        _category.Width = 215;
        _category.DropDownStyle = ComboBoxStyle.DropDownList;
        _category.Margin = new Padding(0, 4, 8, 4);

        ConfigureButton(_searchButton, "Tìm kiếm", 95, LoadData);
        ConfigureButton(_addButton, "Thêm", 75, AddIssue);
        ConfigureButton(_editButton, "Sửa", 70, EditIssue);
        ConfigureButton(_deleteButton, "Xóa", 70, DeleteIssue);
        ConfigureButton(_refreshButton, "Làm mới", 85, LoadData);
        toolbar.Controls.AddRange(new Control[] { _search, _category, _searchButton, _addButton, _editButton, _deleteButton, _refreshButton });

        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AutoGenerateColumns = true;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        _grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = false;
        _grid.RowHeadersVisible = false;
        _grid.CellDoubleClick += (_, _) => EditIssue();

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 32, BackColor = UiTheme.Background };
        _summary.Dock = DockStyle.Fill;
        _summary.TextAlign = ContentAlignment.MiddleLeft;
        _summary.ForeColor = UiTheme.Muted;
        footer.Controls.Add(_summary);

        Controls.Add(_grid);
        Controls.Add(footer);
        Controls.Add(toolbar);
        Controls.Add(heading);
        UiTheme.Apply(this);
    }

    private static void ConfigureButton(Button button, string text, int width, Action action)
    {
        button.Text = text;
        button.Width = width;
        button.Height = 36;
        button.Margin = new Padding(0, 1, 8, 1);
        button.Click += (_, _) => action();
    }

    private int? SelectedCategoryId()
    {
        if (_category.SelectedValue is null) return null;
        var id = Convert.ToInt32(_category.SelectedValue);
        return id > 0 ? id : null;
    }

    private void LoadData()
    {
        try
        {
            var issues = _repository.Search(_search.Text, SelectedCategoryId());
            _grid.DataSource = issues;
            SetColumnHeader(nameof(TroubleshootingIssue.NodeID), "Mã");
            SetColumnHeader(nameof(TroubleshootingIssue.CategoryName), "Nhóm");
            SetColumnHeader(nameof(TroubleshootingIssue.Question), "Lỗi / triệu chứng");
            SetColumnHeader(nameof(TroubleshootingIssue.Diagnosis), "Kết luận");
            SetColumnHeader(nameof(TroubleshootingIssue.Action), "Cách khắc phục");
            SetColumnHeader(nameof(TroubleshootingIssue.IsActive), "Đang sử dụng");
            if (_grid.Columns.Contains(nameof(TroubleshootingIssue.CategoryNodeID)))
                _grid.Columns[nameof(TroubleshootingIssue.CategoryNodeID)]!.Visible = false;
            if (_grid.Columns.Contains(nameof(TroubleshootingIssue.NodeID)))
                _grid.Columns[nameof(TroubleshootingIssue.NodeID)]!.FillWeight = 45;
            if (_grid.Columns.Contains(nameof(TroubleshootingIssue.CategoryName)))
                _grid.Columns[nameof(TroubleshootingIssue.CategoryName)]!.FillWeight = 100;
            if (_grid.Columns.Contains(nameof(TroubleshootingIssue.Question)))
                _grid.Columns[nameof(TroubleshootingIssue.Question)]!.FillWeight = 145;
            if (_grid.Columns.Contains(nameof(TroubleshootingIssue.Diagnosis)))
                _grid.Columns[nameof(TroubleshootingIssue.Diagnosis)]!.FillWeight = 150;
            if (_grid.Columns.Contains(nameof(TroubleshootingIssue.Action)))
                _grid.Columns[nameof(TroubleshootingIssue.Action)]!.FillWeight = 230;
            _summary.Text = $"Tìm thấy {issues.Count} lỗi. Chọn một dòng để sửa hoặc xóa; nhấp đúp để sửa nhanh.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Không tải được danh sách sự cố: " + ex.Message,
                "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SetColumnHeader(string name, string header)
    {
        if (_grid.Columns.Contains(name)) _grid.Columns[name]!.HeaderText = header;
    }

    private TroubleshootingIssue? SelectedIssue() => _grid.CurrentRow?.DataBoundItem as TroubleshootingIssue;

    private void AddIssue() => Edit(null);

    private void EditIssue()
    {
        var issue = SelectedIssue();
        if (issue is null)
        {
            MessageBox.Show(this, "Vui lòng chọn một lỗi cần sửa.", "Chưa chọn lỗi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        Edit(issue);
    }

    private void Edit(TroubleshootingIssue? issue)
    {
        try
        {
            using var dialog = new TroubleshootingIssueEditForm(_repository, issue);
            if (dialog.ShowDialog(this) == DialogResult.OK) LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Không mở được biểu mẫu sự cố: " + ex.Message,
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DeleteIssue()
    {
        var issue = SelectedIssue();
        if (issue is null)
        {
            MessageBox.Show(this, "Vui lòng chọn một lỗi cần xóa.", "Chưa chọn lỗi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show(this,
                $"Bạn có chắc muốn xóa lỗi '{issue.Question}' khỏi nhóm '{issue.CategoryName}'?\nThao tác này không thể hoàn tác.",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        try
        {
            _repository.Delete(issue.NodeID);
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Không thể xóa lỗi: " + ex.Message,
                "Lỗi cập nhật", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

internal sealed class TroubleshootingIssueEditForm : Form
{
    private readonly TroubleshootingRepository _repository;
    private readonly TroubleshootingIssue? _issue;
    private readonly ComboBox _category = new();
    private readonly TextBox _question = new();
    private readonly TextBox _diagnosis = new();
    private readonly TextBox _action = new();
    private readonly CheckBox _active = new();

    public TroubleshootingIssueEditForm(TroubleshootingRepository repository, TroubleshootingIssue? issue)
    {
        _repository = repository;
        _issue = issue;
        InitializeLayout();
        var categories = _repository.GetCategories();
        _category.DataSource = categories;
        _category.DisplayMember = nameof(TroubleshootingCategory.Name);
        _category.ValueMember = nameof(TroubleshootingCategory.NodeID);

        if (_issue is not null)
        {
            _category.SelectedValue = _issue.CategoryNodeID;
            _question.Text = _issue.Question;
            _diagnosis.Text = _issue.Diagnosis;
            _action.Text = _issue.Action;
            _active.Checked = _issue.IsActive;
        }
        else
        {
            _active.Checked = true;
        }

        if (categories.Count == 0)
            throw new InvalidOperationException("Không có nhóm sự cố khả dụng. Hãy kiểm tra dữ liệu hoặc chạy file Upgrade_Troubleshooting_DecisionTree_Depth.sql sau khi đã có cấu trúc bảng.");
    }

    private void InitializeLayout()
    {
        Text = _issue is null ? "Thêm lỗi / cách khắc phục" : "Sửa lỗi / cách khắc phục";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(690, 540);
        Size = new Size(800, 650);
        BackColor = UiTheme.Background;
        Font = UiTheme.BodyFont;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(18),
            ColumnCount = 2,
            RowCount = 6,
            BackColor = UiTheme.Background
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 115));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        _category.Dock = DockStyle.Fill;
        _category.DropDownStyle = ComboBoxStyle.DropDownList;
        _question.Dock = DockStyle.Fill;
        _question.MaxLength = 300;
        _diagnosis.Dock = DockStyle.Fill;
        _diagnosis.Multiline = true;
        _diagnosis.ScrollBars = ScrollBars.Vertical;
        _diagnosis.MaxLength = 2000;
        _action.Dock = DockStyle.Fill;
        _action.Multiline = true;
        _action.ScrollBars = ScrollBars.Vertical;
        _action.MaxLength = 4000;
        _active.Text = "Đang sử dụng trong cây chẩn đoán";
        _active.Dock = DockStyle.Fill;

        AddRow(layout, "Nhóm sự cố", _category, 0);
        AddRow(layout, "Lỗi / triệu chứng", _question, 1);
        AddRow(layout, "Kết luận", _diagnosis, 2);
        AddRow(layout, "Cách khắc phục", _action, 3);
        layout.Controls.Add(_active, 1, 4);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            BackColor = UiTheme.Background,
            Padding = new Padding(0, 5, 0, 0)
        };
        var save = UiTheme.MakeButton("Lưu");
        save.Width = 100;
        save.Click += (_, _) => SaveIssue();
        var cancel = UiTheme.MakeButton("Hủy", true);
        cancel.Width = 100;
        cancel.DialogResult = DialogResult.Cancel;
        buttons.Controls.Add(save);
        buttons.Controls.Add(cancel);
        layout.Controls.Add(buttons, 1, 5);
        Controls.Add(layout);
        AcceptButton = save;
        CancelButton = cancel;
        UiTheme.Apply(this);
    }

    private static void AddRow(TableLayoutPanel layout, string labelText, Control input, int row)
    {
        var label = new Label
        {
            Text = labelText,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft,
            Padding = new Padding(0, 6, 6, 0),
            ForeColor = UiTheme.Text
        };
        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(input, 1, row);
    }

    private void SaveIssue()
    {
        if (_category.SelectedValue is null || !int.TryParse(Convert.ToString(_category.SelectedValue), out var categoryId) || categoryId <= 0)
        {
            MessageBox.Show(this, "Hãy chọn nhóm sự cố.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(_question.Text) || string.IsNullOrWhiteSpace(_diagnosis.Text) || string.IsNullOrWhiteSpace(_action.Text))
        {
            MessageBox.Show(this, "Tên lỗi, kết luận và hướng khắc phục đều là thông tin bắt buộc.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedCategory = (TroubleshootingCategory)_category.SelectedItem!;
        var issue = new TroubleshootingIssue(
            _issue?.NodeID ?? 0,
            categoryId,
            selectedCategory.Name,
            _question.Text.Trim(),
            _diagnosis.Text.Trim(),
            _action.Text.Trim(),
            _active.Checked);

        try
        {
            _repository.Save(issue);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Không lưu được lỗi: " + ex.Message, "Lỗi cập nhật", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
