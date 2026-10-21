using System.ComponentModel;
using PCBuilder.Services;

namespace PCBuilder.Forms;

public sealed partial class TroubleshootingForm : Form
{
    private const int ExpectedQuestionCount = 4;
    private TroubleshootingService? _service;
    private readonly Stack<int> _history = new();
    private readonly List<string> _answerTrail = new();
    private int _current;

    public TroubleshootingForm()
    {
        InitializeComponent();
        _back.Click += (_, _) => Back();
        _restart.Click += (_, _) => Restart();
        _answers.ClientSizeChanged += (_, _) => ResizeAnswerButtons();

        UiTheme.Apply(this);
        _question.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        _question.ForeColor = UiTheme.Text;
        _progress.ForeColor = UiTheme.Primary;
        _pathLabel.ForeColor = UiTheme.Muted;
        _answers.BackColor = UiTheme.Background;
        _diagnosis.Font = new Font("Segoe UI", 10F);

        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            return;

        try
        {
            _service = new TroubleshootingService();
            Restart();
        }
        catch (Exception ex)
        {
            ShowLoadError(ex);
        }
    }

    private void ShowLoadError(Exception ex)
    {
        _question.Text = "Không thể tải cây chẩn đoán sự cố.";
        _progress.Text = "KIỂM TRA DỮ LIỆU";
        _pathLabel.Text = "Hãy kiểm tra script SQL nâng cấp cây quyết định.";
        _answers.Visible = false;
        _diagnosis.Text = "Không tải được dữ liệu chẩn đoán từ SQL Server.\n\n" + ex.Message;
        _diagnosis.Visible = true;
        _diagnosis.BringToFront();
        _back.Enabled = false;
        MessageBox.Show(this,
            "Không tải được dữ liệu chẩn đoán từ SQL Server.\n\n" + ex.Message,
            "Lỗi dữ liệu chẩn đoán", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void Restart()
    {
        if (_service is null)
            return;

        _history.Clear();
        _answerTrail.Clear();
        _current = _service.Root;
        Render();
    }

    private void Render()
    {
        if (_service is null)
            return;

        var step = _service.Get(_current);
        _question.Text = step.Question;
        ClearAnswers();

        var terminal = _service.IsTerminal(_current);
        var questionNo = _service.CountQuestionNodes(_history, _current);
        _back.Enabled = _history.Count > 0;
        UpdateProgress(step, terminal, questionNo);
        UpdateBreadcrumb();

        if (terminal)
        {
            _answers.Visible = false;
            _diagnosis.Text =
                $"LỖI / TRIỆU CHỨNG\n{step.Question}\n\n" +
                $"KẾT LUẬN\n{step.Diagnosis}\n\n" +
                $"HƯỚNG KHẮC PHỤC\n{step.Action}\n\n" +
                "Đây là hướng dẫn sàng lọc ban đầu, không thay thế kiểm tra trực tiếp của kỹ thuật viên. " +
                "Nếu có mùi khét, tia lửa, pin phồng hoặc nhiệt độ bất thường, hãy tắt máy an toàn và nhờ kỹ thuật viên kiểm tra.";
            _diagnosis.Visible = true;
            _diagnosis.BringToFront();
            return;
        }

        if (step.Options.Length == 0)
        {
            _answers.Visible = false;
            _diagnosis.Text =
                $"Câu hỏi này chưa có lựa chọn đang hoạt động (NodeID: {step.NodeID}).\n\n" +
                "Có thể các kết luận ở nhánh này đang bị vô hiệu hóa hoặc dữ liệu liên kết chưa đầy đủ. " +
                "Vui lòng vào mục Quản lý sự cố để kiểm tra các nhánh, hoặc liên hệ quản trị viên.";
            _diagnosis.Visible = true;
            _diagnosis.BringToFront();
            return;
        }

        _diagnosis.Visible = false;
        _answers.Visible = true;
        _answers.BringToFront();

        for (var i = 0; i < step.Options.Length; i++)
        {
            var index = i;
            var button = new Button
            {
                Text = $"{i + 1:00}   {step.Options[i]}",
                Width = GetAnswerButtonWidth(),
                Height = 58,
                AutoSize = false,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(14, 0, 12, 0),
                Margin = new Padding(2, 0, 2, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = UiTheme.Primary,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = UiTheme.PrimaryHover;
            button.Click += (_, _) =>
            {
                if (_service is null)
                    return;

                try
                {
                    var selectedText = step.Options[index];
                    var next = _service.Follow(_current, index);
                    _history.Push(_current);
                    _answerTrail.Add(selectedText);
                    _current = next;
                    Render();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Không thể chuyển bước chẩn đoán",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            _answers.Controls.Add(button);
        }

        ResizeAnswerButtons();
        _answers.ResumeLayout(true);
    }

    private void UpdateProgress(TroubleStep step, bool terminal, int questionNo)
    {
        if (step.IsRoot)
        {
            _progress.Text = "BẮT ĐẦU  •  CHỌN NHÓM SỰ CỐ";
            return;
        }

        if (terminal)
        {
            _progress.Text = $"HOÀN TẤT  •  ĐÃ ĐI QUA {Math.Max(questionNo, ExpectedQuestionCount)}/{ExpectedQuestionCount} NODE CÂU HỎI";
            return;
        }

        _progress.Text = $"CÂU HỎI {Math.Min(questionNo, ExpectedQuestionCount)}/{ExpectedQuestionCount}  •  CÂY QUYẾT ĐỊNH";
    }

    private void UpdateBreadcrumb()
    {
        if (_answerTrail.Count == 0)
        {
            _pathLabel.Text = "Lộ trình chẩn đoán sẽ hiển thị ở đây khi bạn trả lời.";
            return;
        }

        var trail = "Lộ trình: " + string.Join("  ›  ", _answerTrail);
        const int maxLength = 235;
        _pathLabel.Text = trail.Length <= maxLength ? trail : trail[..(maxLength - 1)] + "…";
    }

    private int GetAnswerButtonWidth()
    {
        return Math.Max(260, _answers.ClientSize.Width - _answers.Padding.Horizontal - 28);
    }

    private void ResizeAnswerButtons()
    {
        if (_answers.IsDisposed)
            return;

        var width = GetAnswerButtonWidth();
        foreach (Control control in _answers.Controls)
        {
            if (control is Button button)
                button.Width = width;
        }
    }

    private void ClearAnswers()
    {
        _answers.SuspendLayout();
        foreach (Control control in _answers.Controls.Cast<Control>().ToArray())
        {
            _answers.Controls.Remove(control);
            control.Dispose();
        }
        _answers.ResumeLayout(true);
    }

    private void Back()
    {
        if (_history.Count == 0)
            return;

        _current = _history.Pop();
        if (_answerTrail.Count > 0)
            _answerTrail.RemoveAt(_answerTrail.Count - 1);
        Render();
    }
}
