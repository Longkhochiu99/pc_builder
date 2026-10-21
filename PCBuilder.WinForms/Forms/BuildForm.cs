using PCBuilder.Data; using PCBuilder.Models; using PCBuilder.Services;
namespace PCBuilder.Forms;
public sealed partial class BuildForm : Form
{
    private readonly ComponentRepository _repo=new(); private readonly CompatibilityService _compat=new(); private readonly AppUser _user; private readonly RecommendationService _recommendation=new(); private readonly List<ComponentItem> _build=new();
    public BuildForm():this(new AppUser(0,"designer","Designer","User"),true){}
    public BuildForm(AppUser user):this(user,false){}
    private BuildForm(AppUser user,bool designMode){_user=user;InitializeComponent();if(designMode)return;_purpose.Items.AddRange(new object[]{"Gaming","Lập trình/Đồ họa","Cân bằng"});_purpose.SelectedIndex=0;UiTheme.Apply(this);LoadCatalog();RefreshBuild();}
    private void SearchClick(object? s,EventArgs e)=>LoadCatalog(); private void AddClick(object? s,EventArgs e)=>AddSelected(); private void RemoveClick(object? s,EventArgs e)=>RemoveSelected(); private void CheckClick(object? s,EventArgs e)=>Check(); private void RecommendClick(object? s,EventArgs e)=>Recommend(); private void SaveClick(object? s,EventArgs e)=>SaveBuild();
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
    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        DialogResult dialog = MessageBox.Show("Bạn có chắc chắn muốn thoát chương trình ?", "Warning", MessageBoxButtons.YesNo);
        if (dialog == DialogResult.Yes)
        {

        }
        else
        {
            e.Cancel = true;
        }
    }

}
