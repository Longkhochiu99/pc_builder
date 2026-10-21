namespace PCBuilder.Forms;

partial class TroubleshootingForm
{
    private System.ComponentModel.IContainer? components;
    private Label _progress = null!;
    private Label _question = null!;
    private Label _pathLabel = null!;
    private FlowLayoutPanel _answers = null!;
    private RichTextBox _diagnosis = null!;
    private Button _back = null!;
    private Button _restart = null!;
    private FlowLayoutPanel bottom = null!;
    private Panel _contentHost = null!;
    private TableLayoutPanel _layout = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _layout = new TableLayoutPanel();
        _progress = new Label();
        _question = new Label();
        _pathLabel = new Label();
        _contentHost = new Panel();
        _answers = new FlowLayoutPanel();
        _diagnosis = new RichTextBox();
        bottom = new FlowLayoutPanel();
        _back = new Button();
        _restart = new Button();

        _layout.SuspendLayout();
        _contentHost.SuspendLayout();
        bottom.SuspendLayout();
        SuspendLayout();

        _layout.ColumnCount = 1;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.RowCount = 5;
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        _layout.Dock = DockStyle.Fill;
        _layout.Margin = new Padding(0);
        _layout.Padding = new Padding(20, 12, 20, 12);
        _layout.Name = "_layout";
        _layout.TabIndex = 0;

        _progress.Dock = DockStyle.Fill;
        _progress.AutoSize = false;
        _progress.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _progress.ForeColor = Color.FromArgb(72, 105, 255);
        _progress.Margin = new Padding(2, 0, 0, 0);
        _progress.Name = "_progress";
        _progress.TabIndex = 0;
        _progress.Text = "BẮT ĐẦU  •  CHỌN NHÓM SỰ CỐ";
        _progress.TextAlign = ContentAlignment.MiddleLeft;

        _question.Dock = DockStyle.Fill;
        _question.AutoSize = false;
        _question.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        _question.ForeColor = Color.FromArgb(31, 41, 55);
        _question.Margin = new Padding(0, 0, 0, 4);
        _question.Name = "_question";
        _question.Padding = new Padding(4, 0, 4, 0);
        _question.TabIndex = 1;
        _question.Text = "Đang tải cây chẩn đoán...";
        _question.TextAlign = ContentAlignment.MiddleLeft;

        _pathLabel.Dock = DockStyle.Fill;
        _pathLabel.AutoSize = false;
        _pathLabel.AutoEllipsis = true;
        _pathLabel.Font = new Font("Segoe UI", 8.5F);
        _pathLabel.ForeColor = Color.FromArgb(100, 116, 139);
        _pathLabel.Margin = new Padding(3, 0, 0, 4);
        _pathLabel.Name = "_pathLabel";
        _pathLabel.TabIndex = 2;
        _pathLabel.Text = "Lộ trình chẩn đoán sẽ hiển thị ở đây khi bạn trả lời.";
        _pathLabel.TextAlign = ContentAlignment.MiddleLeft;

        _contentHost.Dock = DockStyle.Fill;
        _contentHost.Margin = new Padding(0);
        _contentHost.Name = "_contentHost";
        _contentHost.Padding = new Padding(0);
        _contentHost.TabIndex = 3;

        _answers.AutoScroll = true;
        _answers.BackColor = Color.FromArgb(245, 247, 251);
        _answers.Dock = DockStyle.Fill;
        _answers.FlowDirection = FlowDirection.TopDown;
        _answers.Margin = new Padding(0);
        _answers.Name = "_answers";
        _answers.Padding = new Padding(8, 10, 8, 10);
        _answers.TabIndex = 0;
        _answers.WrapContents = false;

        _diagnosis.BackColor = Color.White;
        _diagnosis.BorderStyle = BorderStyle.FixedSingle;
        _diagnosis.Dock = DockStyle.Fill;
        _diagnosis.Font = new Font("Segoe UI", 10F);
        _diagnosis.ForeColor = Color.FromArgb(31, 41, 55);
        _diagnosis.Margin = new Padding(0);
        _diagnosis.Name = "_diagnosis";
        _diagnosis.ReadOnly = true;
        _diagnosis.ScrollBars = RichTextBoxScrollBars.Vertical;
        _diagnosis.TabIndex = 1;
        _diagnosis.Text = string.Empty;
        _diagnosis.Visible = false;

        _contentHost.Controls.Add(_answers);
        _contentHost.Controls.Add(_diagnosis);
        _answers.BringToFront();

        bottom.AutoSize = false;
        bottom.Dock = DockStyle.Fill;
        bottom.FlowDirection = FlowDirection.LeftToRight;
        bottom.Margin = new Padding(0);
        bottom.Name = "bottom";
        bottom.Padding = new Padding(0, 8, 0, 0);
        bottom.TabIndex = 4;
        bottom.WrapContents = false;

        _back.Enabled = false;
        _back.FlatStyle = FlatStyle.Flat;
        _back.FlatAppearance.BorderSize = 1;
        _back.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        _back.BackColor = Color.White;
        _back.ForeColor = Color.FromArgb(31, 41, 55);
        _back.Cursor = Cursors.Hand;
        _back.Margin = new Padding(0, 0, 10, 0);
        _back.Name = "_back";
        _back.Size = new Size(112, 40);
        _back.TabIndex = 0;
        _back.Text = "Quay lại";
        _back.UseVisualStyleBackColor = false;

        _restart.FlatStyle = FlatStyle.Flat;
        _restart.FlatAppearance.BorderSize = 0;
        _restart.BackColor = Color.FromArgb(72, 105, 255);
        _restart.ForeColor = Color.White;
        _restart.Cursor = Cursors.Hand;
        _restart.Margin = new Padding(0);
        _restart.Name = "_restart";
        _restart.Size = new Size(128, 40);
        _restart.TabIndex = 1;
        _restart.Text = "Bắt đầu lại";
        _restart.UseVisualStyleBackColor = false;

        bottom.Controls.Add(_back);
        bottom.Controls.Add(_restart);

        _layout.Controls.Add(_progress, 0, 0);
        _layout.Controls.Add(_question, 0, 1);
        _layout.Controls.Add(_pathLabel, 0, 2);
        _layout.Controls.Add(_contentHost, 0, 3);
        _layout.Controls.Add(bottom, 0, 4);

        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(245, 247, 251);
        ClientSize = new Size(1000, 650);
        MinimumSize = new Size(760, 520);
        Controls.Add(_layout);
        Name = "TroubleshootingForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Trợ lý chẩn đoán sự cố";

        bottom.ResumeLayout(false);
        _contentHost.ResumeLayout(false);
        _layout.ResumeLayout(false);
        ResumeLayout(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }
}
