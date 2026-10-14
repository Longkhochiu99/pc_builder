using PCBuilder.Services;

namespace PCBuilder.Forms;

public sealed class TroubleshootingForm : Form
{
    private readonly TroubleshootingService _service = new(); private readonly Stack<int> _history = new(); private int _current;
    private readonly Label _question = new() { Dock = DockStyle.Top, Height = 100, Font = new Font("Segoe UI", 15, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
    private readonly FlowLayoutPanel _answers = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
    private readonly RichTextBox _diagnosis = new() { Dock = DockStyle.Fill, ReadOnly = true, Visible = false };
    private readonly Button _back = new() { Text = "Quay lại", Width = 100, Enabled = false };
    private readonly Button _restart = new() { Text = "Bắt đầu lại", Width = 110 };
    public TroubleshootingForm()
    {
        Dock = DockStyle.Fill; var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 48 }; _back.Click += (_, _) => Back(); _restart.Click += (_, _) => Restart(); buttons.Controls.Add(_back); buttons.Controls.Add(_restart);
        Controls.Add(_answers); Controls.Add(_diagnosis); Controls.Add(_question); Controls.Add(buttons); UiTheme.Apply(this); Restart();
    }
    private void Restart() { _history.Clear(); _current = _service.Root; _diagnosis.Visible = false; _answers.Visible = true; Render(); }
    private void Render()
    {
        var step = _service.Get(_current); _question.Text = step.Question; _answers.Controls.Clear(); _diagnosis.Visible = _service.IsTerminal(_current); _answers.Visible = !_diagnosis.Visible; _back.Enabled = _history.Count > 0;
        if (_service.IsTerminal(_current)) _diagnosis.Text = $"KẾT LUẬN\n{step.Diagnosis}\n\nHƯỚNG KHẮC PHỤC\n{step.Action}\n\nĐây là hướng dẫn sàng lọc ban đầu, không thay thế kiểm tra trực tiếp của kỹ thuật viên.";
        else for (int i = 0; i < step.Options.Length; i++) { int index = i; var b = new Button { Text = step.Options[i], Width = 500, Height = 46, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(4, 8, 4, 4) }; b.Click += (_, _) => { _history.Push(_current); _current = _service.Follow(_current, index); Render(); }; _answers.Controls.Add(b); }
        UiTheme.Apply(_answers);
    }
    private void Back() { if (_history.Count > 0) { _current = _history.Pop(); Render(); } }
}
