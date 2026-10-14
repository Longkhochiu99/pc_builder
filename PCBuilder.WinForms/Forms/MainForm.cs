using PCBuilder.Models;

namespace PCBuilder.Forms;

public sealed class MainForm : Form
{
    private readonly AppUser _user;
    private readonly Panel _content = new() { Dock = DockStyle.Fill, Padding = new Padding(28), AutoScroll = true, BackColor = UiTheme.Background };
    private readonly Panel _sidebar = new() { Dock = DockStyle.Left, Width = 242, BackColor = UiTheme.Sidebar, Padding = new Padding(16, 22, 16, 18) };
    private readonly Label _pageTitle = new() { AutoSize = true, Font = new Font("Segoe UI", 21, FontStyle.Bold), ForeColor = UiTheme.Text };

    public MainForm(AppUser user)
    {
        _user = user;
        Text = "PC Builder | Hệ thống Build PC";
        WindowState = FormWindowState.Maximized; MinimumSize = new Size(1100, 700);
        BackColor = UiTheme.Background; Font = UiTheme.BodyFont;

        var brand = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = UiTheme.Sidebar };
        var logo = new Label { Text = "▣  PC BUILDER", AutoSize = true, Location = new Point(8, 8), ForeColor = Color.White, Font = new Font("Segoe UI", 16, FontStyle.Bold) };
        var tagline = new Label { Text = "HARDWARE WORKSHOP", AutoSize = true, Location = new Point(11, 43), ForeColor = Color.FromArgb(151, 166, 193), Font = new Font("Segoe UI", 8, FontStyle.Bold) };
        brand.Controls.AddRange(new Control[] { logo, tagline });

        var nav = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, BackColor = UiTheme.Sidebar, Padding = new Padding(0, 12, 0, 0) };
        AddNav(nav, "⌂   Tổng quan", ShowDashboard);
        AddNav(nav, "▦   Tra cứu linh kiện", () => ShowChild(new ComponentsForm(_user)));
        AddNav(nav, "⚙   Build PC & gợi ý", () => ShowChild(new BuildForm(_user)));
        AddNav(nav, "✚   Chẩn đoán sự cố", () => ShowChild(new TroubleshootingForm()));
        if (_user.IsAdmin) AddNav(nav, "✎   Quản lý linh kiện", () => ShowChild(new ComponentsForm(_user, true)));

        var userPanel = new Panel { Dock = DockStyle.Bottom, Height = 104, BackColor = Color.FromArgb(25, 38, 61), Padding = new Padding(10) };
        var userName = new Label { Text = _user.FullName, AutoSize = false, Height = 24, Dock = DockStyle.Top, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        var role = new Label { Text = _user.IsAdmin ? "ADMINISTRATOR" : "USER ACCOUNT", AutoSize = false, Height = 22, Dock = DockStyle.Top, ForeColor = Color.FromArgb(157, 174, 203), Font = new Font("Segoe UI", 8, FontStyle.Bold) };
        var logout = new Button { Text = "⇥   Đăng xuất", Dock = DockStyle.Bottom, Height = 34, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(39, 54, 79), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand };
        logout.FlatAppearance.BorderSize = 0; logout.Click += (_, _) => Close();
        userPanel.Controls.Add(logout); userPanel.Controls.Add(role); userPanel.Controls.Add(userName);
        _sidebar.Controls.Add(nav); _sidebar.Controls.Add(brand); _sidebar.Controls.Add(userPanel);

        var topbar = new Panel { Dock = DockStyle.Top, Height = 74, BackColor = Color.White, Padding = new Padding(28, 20, 28, 12) };
        _pageTitle.Text = "Tổng quan"; _pageTitle.Location = new Point(0, 0); topbar.Controls.Add(_pageTitle);
        var hello = new Label { Text = $"Xin chào, {_user.FullName}", AutoSize = true, ForeColor = UiTheme.Muted, Anchor = AnchorStyles.Top | AnchorStyles.Right, Location = new Point(760, 8) }; topbar.Controls.Add(hello);
        var main = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Background };
        main.Controls.Add(_content); main.Controls.Add(topbar);
        Controls.Add(main); Controls.Add(_sidebar);
        ShowDashboard();
    }

    private void AddNav(Control parent, string text, Action action)
    {
        var b = new Button { Text = text, Width = 206, Height = 46, TextAlign = ContentAlignment.MiddleLeft, FlatStyle = FlatStyle.Flat, BackColor = UiTheme.Sidebar, ForeColor = Color.FromArgb(213, 222, 238), Font = new Font("Segoe UI", 10), Margin = new Padding(0, 3, 0, 3), Padding = new Padding(12, 0, 4, 0), Cursor = Cursors.Hand };
        b.FlatAppearance.BorderSize = 0; b.FlatAppearance.MouseOverBackColor = Color.FromArgb(38, 54, 82); b.Click += (_, _) => action(); parent.Controls.Add(b);
    }

    private void ShowChild(Form form)
    {
        _content.SuspendLayout(); _content.Controls.Clear();
        form.TopLevel = false; form.FormBorderStyle = FormBorderStyle.None; form.Dock = DockStyle.Fill;
        _content.Controls.Add(form); form.Show(); _content.ResumeLayout();
        _pageTitle.Text = form switch { ComponentsForm => "Tra cứu linh kiện", BuildForm => "Build PC & gợi ý cấu hình", TroubleshootingForm => "Trợ lý chẩn đoán sự cố", _ => form.Text };
    }

    private void ShowDashboard()
    {
        _content.Controls.Clear(); _pageTitle.Text = "Tổng quan";
        var page = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, BackColor = UiTheme.Background, Padding = new Padding(0, 4, 0, 20) };
        var welcome = new Panel { Width = 940, Height = 150, BackColor = UiTheme.Sidebar, Padding = new Padding(28), Margin = new Padding(0, 0, 0, 20) };
        welcome.Controls.Add(new Label { Text = "Build PC theo cách thông minh.", AutoSize = true, Font = new Font("Segoe UI", 23, FontStyle.Bold), ForeColor = Color.White, Location = new Point(25, 24) });
        welcome.Controls.Add(new Label { Text = "So sánh linh kiện, kiểm tra tương thích và tìm cấu hình phù hợp ngân sách.", AutoSize = true, ForeColor = Color.FromArgb(194, 207, 228), Location = new Point(28, 72), Font = new Font("Segoe UI", 10.5F) });
        var goBuild = UiTheme.MakeButton("Bắt đầu build PC  →"); goBuild.Location = new Point(28, 100); goBuild.Height = 36; goBuild.Click += (_, _) => ShowChild(new BuildForm(_user)); welcome.Controls.Add(goBuild);
        var cards = new FlowLayoutPanel { Width = 960, Height = 150, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, BackColor = UiTheme.Background, Margin = new Padding(0, 0, 0, 18) };
        cards.Controls.Add(MakeInfoCard("01", "Tra cứu linh kiện", "Tìm theo tên, hãng và danh mục.", Color.FromArgb(72, 105, 255)));
        cards.Controls.Add(MakeInfoCard("02", "Kiểm tra tương thích", "Phát hiện xung đột phần cứng có dữ liệu.", Color.FromArgb(14, 165, 150)));
        cards.Controls.Add(MakeInfoCard("03", "Tối ưu ngân sách", "Xếp hạng cấu hình theo điểm hiệu năng/giá.", Color.FromArgb(234, 145, 52)));
        var section = new Label { Text = "CÔNG CỤ CỦA BẠN", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = UiTheme.Muted, Margin = new Padding(0, 8, 0, 12) };
        var actions = new FlowLayoutPanel { Width = 940, Height = 120, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, BackColor = UiTheme.Background };
        actions.Controls.Add(MakeActionCard("Tra cứu linh kiện", "Mở danh mục phần cứng", () => ShowChild(new ComponentsForm(_user))));
        actions.Controls.Add(MakeActionCard("Gợi ý cấu hình", "Nhập ngân sách và mục đích", () => ShowChild(new BuildForm(_user))));
        actions.Controls.Add(MakeActionCard("Chẩn đoán sự cố", "Theo cây câu hỏi từng bước", () => ShowChild(new TroubleshootingForm())));
        page.Controls.Add(welcome); page.Controls.Add(cards); page.Controls.Add(section); page.Controls.Add(actions);
        var note = new Label { Text = "Lưu ý: kết quả gợi ý phụ thuộc dữ liệu linh kiện và benchmark trong database. Dữ liệu demo chỉ phục vụ thử nghiệm.", AutoSize = true, MaximumSize = new Size(900, 0), ForeColor = UiTheme.Muted, Margin = new Padding(0, 20, 0, 0) }; page.Controls.Add(note);
        _content.Controls.Add(page);
    }

    private static Panel MakeInfoCard(string number, string title, string description, Color accent)
    {
        var p = UiTheme.MakeCard(); p.Size = new Size(292, 132);
        p.Controls.Add(new Label { Text = number, AutoSize = true, ForeColor = accent, Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(18, 12) });
        p.Controls.Add(new Label { Text = title, AutoSize = true, ForeColor = UiTheme.Text, Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(18, 39) });
        p.Controls.Add(new Label { Text = description, AutoSize = false, Size = new Size(245, 40), ForeColor = UiTheme.Muted, Location = new Point(18, 68) });
        return p;
    }

    private static Panel MakeActionCard(string title, string description, Action action)
    {
        var p = UiTheme.MakeCard(); p.Size = new Size(292, 100); p.Cursor = Cursors.Hand;
        p.Controls.Add(new Label { Text = title, AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = UiTheme.Text, Location = new Point(18, 15) });
        p.Controls.Add(new Label { Text = description, AutoSize = true, ForeColor = UiTheme.Muted, Location = new Point(18, 44) });
        p.Click += (_, _) => action(); foreach (Control c in p.Controls) c.Click += (_, _) => action();
        return p;
    }
}
