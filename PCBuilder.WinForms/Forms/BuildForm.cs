using PCBuilder.Data;
using PCBuilder.Models;
using PCBuilder.Services;

namespace PCBuilder.Forms;

public sealed class BuildForm : Form
{
    private readonly ComponentRepository _repo = new(); private readonly CompatibilityService _compat = new(); private readonly AppUser _user; private readonly TextBox _budget = new() { Width = 130, PlaceholderText = "Ngân sách VND" };
    private readonly ComboBox _purpose = new() { Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly RecommendationService _recommendation = new();
    private readonly DataGridView _catalog = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
    private readonly DataGridView _selected = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
    private readonly List<ComponentItem> _build = new(); private readonly Label _total = new() { AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
    private readonly TextBox _search = new() { Width = 220, PlaceholderText = "Tìm linh kiện" };
    private readonly RichTextBox _results = new() { Dock = DockStyle.Fill, ReadOnly = true };
    public BuildForm(AppUser user)
    {
        _user = user; Dock = DockStyle.Fill;
        var searchPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44 }; searchPanel.Controls.Add(_search); var search = new Button { Text = "Tìm", Width = 70 }; search.Click += (_, _) => LoadCatalog(); searchPanel.Controls.Add(search);
        var add = new Button { Text = "Thêm linh kiện đã chọn", Width = 180 }; add.Click += (_, _) => AddSelected(); searchPanel.Controls.Add(add);
        var remove = new Button { Text = "Bỏ khỏi cấu hình", Width = 140 }; remove.Click += (_, _) => RemoveSelected(); searchPanel.Controls.Add(remove);
        var check = new Button { Text = "Kiểm tra tương thích", Width = 160 }; check.Click += (_, _) => Check(); searchPanel.Controls.Add(check);
        searchPanel.Controls.Add(_budget); _purpose.Items.AddRange(new object[] { "Gaming", "Lập trình/Đồ họa", "Cân bằng" }); _purpose.SelectedIndex = 0; searchPanel.Controls.Add(_purpose); var recommend = new Button { Text = "Gợi ý ngân sách", Width = 130 }; recommend.Click += (_, _) => Recommend(); searchPanel.Controls.Add(recommend);
        var save = new Button { Text = "Lưu cấu hình", Width = 100 }; save.Click += (_, _) => SaveBuild(); searchPanel.Controls.Add(save);
        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 520 };
        split.Panel1.Controls.Add(_catalog); split.Panel2.Controls.Add(_selected);
        var bottom = new Panel { Dock = DockStyle.Bottom, Height = 190 }; bottom.Controls.Add(_results); var totalPanel = new Panel { Dock = DockStyle.Top, Height = 30 }; totalPanel.Controls.Add(_total); bottom.Controls.Add(totalPanel);
        Controls.Add(split); Controls.Add(bottom); Controls.Add(searchPanel); UiTheme.Apply(this); LoadCatalog(); RefreshBuild();
    }
    private void LoadCatalog()
    {
        try { _catalog.DataSource = _repo.Search(_search.Text); foreach (var name in new[] { "CategoryID", "BrandID", "IsActive", "CategoryCode", "Socket", "MemoryType", "TDP_W", "PowerDraw_W", "Wattage_W", "LengthMM", "MaxGPULengthMM", "MaxCoolerHeightMM", "FormFactor", "CpuBenchmarkScore", "GpuBenchmarkScore" }) if (_catalog.Columns.Contains(name)) _catalog.Columns[name].Visible = false; }
        catch (Exception ex) { MessageBox.Show("Không tải được danh mục: " + ex.Message); }
    }
    private void AddSelected() { if (_catalog.CurrentRow?.DataBoundItem is not ComponentItem c) return; if (_build.Any(x => x.ComponentID == c.ComponentID)) { MessageBox.Show("Linh kiện đã có trong cấu hình."); return; } _build.Add(c); RefreshBuild(); }
    private void RemoveSelected() { if (_selected.CurrentRow?.DataBoundItem is ComponentItem c) { _build.RemoveAll(x => x.ComponentID == c.ComponentID); RefreshBuild(); } }
    private void RefreshBuild() { _selected.DataSource = null; _selected.DataSource = _build.ToList(); foreach (var name in new[] { "CategoryID", "CategoryCode", "BrandID", "SKU", "StockQuantity", "IsActive", "Socket", "MemoryType", "TdpW", "PowerDrawW", "WattageW", "LengthMM", "MaxGpuLengthMM", "MaxCoolerHeightMM", "FormFactor", "Description", "ImageURL", "CpuBenchmarkScore", "GpuBenchmarkScore" }) if (_selected.Columns.Contains(name)) _selected.Columns[name].Visible = false; _total.Text = $"Tổng giá: {_build.Sum(x => x.Price):N0} VND"; }

    private void Recommend()
    {
        if (!decimal.TryParse(_budget.Text, out var budget) || budget <= 0) { MessageBox.Show("Nhập ngân sách hợp lệ bằng VND, ví dụ 25000000."); return; }
        try
        {
            var purpose = _purpose.SelectedIndex switch { 0 => BuildPurpose.Gaming, 1 => BuildPurpose.Productivity, _ => BuildPurpose.Balanced };
            var candidates = _recommendation.Recommend(_repo.Search(), budget, purpose, 3);
            var chosen = candidates[0];
            _build.Clear(); _build.AddRange(chosen.Components); RefreshBuild(); Check();
            _results.AppendText("\n\n=== GỢI Ý CẤU HÌNH THEO HIỆU NĂNG/GIÁ ===\n");
            for (var i = 0; i < candidates.Count; i++)
            {
                var r = candidates[i];
                _results.AppendText($"\n#{i + 1}: {r.TotalPrice:N0} VND | Điểm hiệu năng tương đối: {r.PerformanceScore:N1} | Hiệu năng/giá: {r.ValueScore:N2}\n");
                foreach (var c in r.Components) _results.AppendText($"  - {c.CategoryName}: {c.DisplayName} ({c.Price:N0} VND)\n");
                _results.AppendText("  " + r.Notes + "\n");
            }
            _results.AppendText("\nĐã đưa cấu hình xếp hạng #1 vào bản build hiện tại. Điểm số chỉ đáng tin khi BenchmarkScores có nguồn dữ liệu đã xác minh; điểm demo không phải benchmark thực tế.\n");
        }
        catch (Exception ex) { MessageBox.Show("Không thể tạo gợi ý: " + ex.Message, "Gợi ý cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Information); }
    }

    private string? PromptForName()
    {
        using var dialog = new Form { Text = "Tên cấu hình", Width = 360, Height = 150, StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false };
        var input = new TextBox { Left = 15, Top = 15, Width = 310, Text = "Cấu hình của tôi" }; var ok = new Button { Text = "Lưu", Left = 245, Top = 50, Width = 80, DialogResult = DialogResult.OK }; dialog.Controls.Add(input); dialog.Controls.Add(ok); dialog.AcceptButton = ok; return dialog.ShowDialog(this) == DialogResult.OK ? input.Text.Trim() : null;
    }
    private void SaveBuild()
    {
        if (_build.Count == 0) { MessageBox.Show("Thêm linh kiện trước khi lưu."); return; }
        var name = PromptForName();
        if (string.IsNullOrWhiteSpace(name)) return;
        decimal? budget = decimal.TryParse(_budget.Text, out var b) && b > 0 ? b : null;
        try { var id = new BuildRepository().Save(_user.UserId, name, budget, _build); MessageBox.Show($"Đã lưu cấu hình #{id}."); }
        catch (Exception ex) { MessageBox.Show("Lưu cấu hình thất bại: " + ex.Message); }
    }

    private void Check()
    {
        _results.Clear(); foreach (var r in _compat.Check(_build)) _results.AppendText($"[{r.Severity}] {r.Message}\n\n");
        var cpu = _build.FirstOrDefault(x => x.CategoryCode == "CPU"); var gpu = _build.FirstOrDefault(x => x.CategoryCode == "GPU");
        _results.AppendText("\n" + _compat.EstimateBottleneck(cpu, gpu));
    }
}
