using PCBuilder.Data;
using PCBuilder.Models;
using System.Data;

namespace PCBuilder.Forms;

public sealed class ComponentsForm : Form
{
    private readonly AppUser _user;
    private readonly bool _adminMode;
    private readonly ComponentRepository _repo = new();
    private readonly TextBox _search = new() { Width = 220, PlaceholderText = "Tên linh kiện / hãng / SKU" };
    private readonly ComboBox _category = new() { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false };
    public ComponentsForm(AppUser user, bool adminMode = false)
    {
        _user = user; _adminMode = adminMode && user.IsAdmin; Dock = DockStyle.Fill;
        var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, WrapContents = false, Padding = new Padding(0, 4, 0, 4) };
        top.Controls.Add(_search); top.Controls.Add(_category);
        var search = new Button { Text = "Tìm kiếm", Width = 90 }; search.Click += (_, _) => LoadData(); top.Controls.Add(search);
        if (_adminMode)
        {
            var add = new Button { Text = "Thêm", Width = 75 }; add.Click += (_, _) => Edit(null); top.Controls.Add(add);
            var edit = new Button { Text = "Sửa", Width = 75 }; edit.Click += (_, _) => Edit(Selected()); top.Controls.Add(edit);
            var del = new Button { Text = "Ngừng kinh doanh", Width = 140 }; del.Click += (_, _) => Delete(); top.Controls.Add(del);
        }
        Controls.Add(_grid); Controls.Add(top);
        try { var categories = _repo.GetCategories(); var all = categories.NewRow(); all["CategoryID"] = DBNull.Value; all["CategoryName"] = "Tất cả danh mục"; categories.Rows.InsertAt(all, 0); _category.DataSource = categories; _category.DisplayMember = "CategoryName"; _category.ValueMember = "CategoryID"; } catch { }
        UiTheme.Apply(this);
        LoadData();
    }
    private int? CategoryId() { if (_category.SelectedValue is DataRowView) return null; return int.TryParse(Convert.ToString(_category.SelectedValue), out var id) && id > 0 ? id : null; }
    private void LoadData()
    {
        try { _grid.DataSource = _repo.Search(_search.Text, CategoryId(), _adminMode); foreach (var name in new[] { "CategoryID", "BrandID", "IsActive", "CategoryCode", "Socket", "MemoryType", "TDP_W", "PowerDraw_W", "Wattage_W", "LengthMM", "MaxGPULengthMM", "MaxCoolerHeightMM", "FormFactor" }) if (_grid.Columns.Contains(name)) _grid.Columns[name].Visible = false; }
        catch (Exception ex) { MessageBox.Show("Không tải được linh kiện: " + ex.Message); }
    }
    private ComponentItem? Selected() => _grid.CurrentRow?.DataBoundItem as ComponentItem;
    private void Edit(ComponentItem? item)
    {
        try
        {
            using var form = new ComponentEditForm(_repo, item); if (form.ShowDialog(this) == DialogResult.OK) LoadData();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }
    private void Delete()
    {
        var item = Selected(); if (item is null) { MessageBox.Show("Chọn linh kiện trước."); return; }
        if (MessageBox.Show($"Ngừng kinh doanh {item.DisplayName}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        try { _repo.Delete(item.ComponentID); LoadData(); } catch (Exception ex) { MessageBox.Show("Không thể cập nhật: " + ex.Message); }
    }
}

internal sealed class ComponentEditForm : Form
{
    private readonly ComponentRepository _repo; private readonly ComponentItem? _item;
    private readonly ComboBox _category = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 250 };
    private readonly ComboBox _brand = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 250 };
    private readonly TextBox _model = new() { Width = 250 }; private readonly TextBox _sku = new() { Width = 250 };
    private readonly NumericUpDown _price = new() { Width = 250, Maximum = 1000000000, DecimalPlaces = 2, ThousandsSeparator = true };
    private readonly NumericUpDown _stock = new() { Width = 250, Maximum = 1000000 };
    private readonly CheckBox _active = new() { Text = "Đang kinh doanh", Checked = true, AutoSize = true };
    public ComponentEditForm(ComponentRepository repo, ComponentItem? item)
    {
        _repo = repo; _item = item; Text = item is null ? "Thêm linh kiện" : "Sửa linh kiện"; Width = 470; Height = 390; StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false;
        var t = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(16), ColumnCount = 2, RowCount = 8, AutoScroll = true }; t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115)); t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        AddRow(t, 0, "Danh mục", _category); AddRow(t, 1, "Hãng", _brand); AddRow(t, 2, "Tên model", _model); AddRow(t, 3, "SKU", _sku); AddRow(t, 4, "Giá (VND)", _price); AddRow(t, 5, "Tồn kho", _stock); AddRow(t, 6, "Trạng thái", _active);
        var save = new Button { Text = "Lưu", Width = 100, Height = 32 }; save.Click += Save; t.Controls.Add(save, 1, 7); Controls.Add(t); UiTheme.Apply(this);
        _category.DataSource = _repo.GetCategories(); _category.DisplayMember = "CategoryName"; _category.ValueMember = "CategoryID";
        _brand.DataSource = _repo.GetBrands(); _brand.DisplayMember = "BrandName"; _brand.ValueMember = "BrandID";
        if (item is not null) { _category.SelectedValue = item.CategoryID; _brand.SelectedValue = item.BrandID; _model.Text = item.ModelName; _sku.Text = item.SKU ?? ""; _price.Value = Math.Min(_price.Maximum, item.Price); _stock.Value = item.StockQuantity; _active.Checked = item.IsActive; }
    }
    private static void AddRow(TableLayoutPanel t, int row, string label, Control control) { t.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row); t.Controls.Add(control, 1, row); }
    private void Save(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_model.Text) || _category.SelectedValue is null || _brand.SelectedValue is null) { MessageBox.Show("Nhập tên model, chọn danh mục và hãng."); return; }
        var c = new ComponentItem { ComponentID = _item?.ComponentID ?? 0, CategoryID = Convert.ToInt32(_category.SelectedValue), BrandID = Convert.ToInt32(_brand.SelectedValue), ModelName = _model.Text.Trim(), SKU = string.IsNullOrWhiteSpace(_sku.Text) ? null : _sku.Text.Trim(), Price = _price.Value, StockQuantity = (int)_stock.Value, IsActive = _active.Checked };
        try { _repo.Save(c); DialogResult = DialogResult.OK; Close(); } catch (Exception ex) { MessageBox.Show("Lưu thất bại: " + ex.Message); }
    }
}
