using PCBuilder.Data;

namespace PCBuilder.Forms;

public sealed class LoginForm : Form
{
    private readonly TextBox _username = new() { Height = 38, PlaceholderText = "Nhập tên tài khoản" };
    private readonly TextBox _password = new() { Height = 38, UseSystemPasswordChar = true, PlaceholderText = "Nhập mật khẩu" };
    private readonly Label _message = new() { AutoSize = true, ForeColor = Color.FromArgb(220, 70, 70), MaximumSize = new Size(330, 0) };

    public LoginForm()
    {
        Text = "PC Builder | Đăng nhập";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 590); Size = new Size(1000, 650);
        FormBorderStyle = FormBorderStyle.Sizable;
        BackColor = UiTheme.Background;

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(18), BackColor = UiTheme.Background };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48)); root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52));
        var hero = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Sidebar, Padding = new Padding(42) };
        var brand = new Label { Text = "PC\nBUILDER", AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 32, FontStyle.Bold), Location = new Point(38, 65) };
        var heroText = new Label { Text = "Xây dựng cấu hình PC\nthông minh hơn.", AutoSize = true, ForeColor = Color.FromArgb(222, 230, 245), Font = new Font("Segoe UI", 18, FontStyle.Bold), Location = new Point(42, 205) };
        var heroSub = new Label { Text = "Tra cứu linh kiện  •  Kiểm tra tương thích\nTối ưu hiệu năng theo ngân sách", AutoSize = true, ForeColor = Color.FromArgb(165, 180, 205), Font = new Font("Segoe UI", 11), Location = new Point(44, 290) };
        var badge = new Label { Text = ".NET 8  |  SQL SERVER", AutoSize = true, ForeColor = Color.FromArgb(190, 205, 255), Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(44, 390) };
        hero.Controls.AddRange(new Control[] { brand, heroText, heroSub, badge });

        var right = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Surface, Padding = new Padding(48, 50, 48, 30) };
        var formTitle = new Label { Text = "Chào mừng trở lại", AutoSize = true, Font = new Font("Segoe UI", 23, FontStyle.Bold), ForeColor = UiTheme.Text, Location = new Point(42, 55) };
        var subtitle = new Label { Text = "Đăng nhập để tiếp tục xây dựng cấu hình của bạn.", AutoSize = true, ForeColor = UiTheme.Muted, Location = new Point(45, 105) };
        var userLabel = new Label { Text = "TÊN TÀI KHOẢN", AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(45, 165) };
        _username.Location = new Point(45, 190); _username.Width = 340; _username.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        var passLabel = new Label { Text = "MẬT KHẨU", AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(45, 250) };
        _password.Location = new Point(45, 275); _password.Width = 340; _password.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        var login = UiTheme.MakeButton("Đăng nhập  →"); login.Location = new Point(45, 345); login.Width = 340; login.Height = 46; login.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; login.Click += Login;
        var registerText = new Label { Text = "Chưa có tài khoản?", AutoSize = true, ForeColor = UiTheme.Muted, Location = new Point(45, 415) };
        var register = new LinkLabel { Text = "Đăng ký tài khoản User", AutoSize = true, LinkColor = UiTheme.Primary, Location = new Point(165, 415), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
        register.Click += (_, _) => { using var f = new RegisterForm(); f.ShowDialog(this); };
        _message.Location = new Point(45, 465);
        var hint = new Label { Text = "Tài khoản Admin được quản lý riêng. Đăng ký mới chỉ tạo quyền User.", AutoSize = false, Height = 45, Width = 340, ForeColor = UiTheme.Muted, Location = new Point(45, 500), Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
        right.Controls.AddRange(new Control[] { formTitle, subtitle, userLabel, _username, passLabel, _password, login, registerText, register, _message, hint });
        root.Controls.Add(hero, 0, 0); root.Controls.Add(right, 1, 0); Controls.Add(root);
        AcceptButton = login;
    }

    private void Login(object? sender, EventArgs e)
    {
        _message.Text = string.Empty;
        if (string.IsNullOrWhiteSpace(_username.Text) || string.IsNullOrEmpty(_password.Text)) { _message.Text = "Vui lòng nhập tài khoản và mật khẩu."; return; }
        try
        {
            var user = new AuthRepository().Login(_username.Text, _password.Text);
            if (user is null) { _message.Text = "Thông tin đăng nhập không đúng hoặc tài khoản đã bị khóa."; return; }
            Hide(); using (var main = new MainForm(user)) main.ShowDialog(); Close();
        }
        catch (Exception ex) { MessageBox.Show("Không thể đăng nhập. Hãy kiểm tra SQL Server và connection string.\n\n" + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
