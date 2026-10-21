namespace PCBuilder.Forms;
partial class MainForm
{
 private System.ComponentModel.IContainer? components; private Panel _content=null!,_sidebar=null!; private Label _pageTitle=null!,_userName=null!,_role=null!,_hello=null!; private FlowLayoutPanel _nav=null!; private Button _logout=null!;

    private Panel brand=null!; private Label logo=null!; private Label tagline=null!; private Panel userPanel=null!; private Panel topbar=null!; private Panel main=null!;
    private void InitializeComponent()
    {
        _content = new Panel();
        _sidebar = new Panel();
        _nav = new FlowLayoutPanel();
        brand = new Panel();
        logo = new Label();
        tagline = new Label();
        userPanel = new Panel();
        _logout = new Button();
        _role = new Label();
        _userName = new Label();
        _pageTitle = new Label();
        _hello = new Label();
        topbar = new Panel();
        main = new Panel();
        _sidebar.SuspendLayout();
        brand.SuspendLayout();
        userPanel.SuspendLayout();
        topbar.SuspendLayout();
        main.SuspendLayout();
        SuspendLayout();
        // 
        // _content
        // 
        _content.AutoScroll = true;
        _content.BackColor = Color.FromArgb(245, 247, 251);
        _content.Dock = DockStyle.Fill;
        _content.Location = new Point(0, 74);
        _content.Name = "_content";
        _content.Padding = new Padding(28);
        _content.Size = new Size(840, 579);
        _content.TabIndex = 0;
        // 
        // _sidebar
        // 
        _sidebar.BackColor = Color.FromArgb(19, 29, 48);
        _sidebar.Controls.Add(_nav);
        _sidebar.Controls.Add(brand);
        _sidebar.Controls.Add(userPanel);
        _sidebar.Dock = DockStyle.Left;
        _sidebar.Location = new Point(0, 0);
        _sidebar.Name = "_sidebar";
        _sidebar.Padding = new Padding(16, 22, 16, 18);
        _sidebar.Size = new Size(242, 653);
        _sidebar.TabIndex = 1;
        // 
        // _nav
        // 
        _nav.AutoScroll = true;
        _nav.BackColor = Color.FromArgb(19, 29, 48);
        _nav.Dock = DockStyle.Fill;
        _nav.FlowDirection = FlowDirection.TopDown;
        _nav.Location = new Point(16, 122);
        _nav.Name = "_nav";
        _nav.Padding = new Padding(0, 12, 0, 0);
        _nav.Size = new Size(210, 409);
        _nav.TabIndex = 0;
        _nav.WrapContents = false;
        // 
        // brand
        // 
        brand.BackColor = Color.FromArgb(19, 29, 48);
        brand.Controls.Add(logo);
        brand.Controls.Add(tagline);
        brand.Dock = DockStyle.Top;
        brand.Location = new Point(16, 22);
        brand.Name = "brand";
        brand.Size = new Size(210, 100);
        brand.TabIndex = 1;
        // 
        // logo
        // 
        logo.AutoSize = true;
        logo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        logo.ForeColor = Color.White;
        logo.Location = new Point(8, 8);
        logo.Name = "logo";
        logo.Size = new Size(170, 37);
        logo.TabIndex = 0;
        logo.Text = "PC BUILDER";
        // 
        // tagline
        // 
        tagline.AutoSize = true;
        tagline.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        tagline.ForeColor = Color.FromArgb(151, 166, 193);
        tagline.Location = new Point(11, 43);
        tagline.Name = "tagline";
        tagline.Size = new Size(175, 19);
        tagline.TabIndex = 1;
        tagline.Text = "HARDWARE WORKSHOP";
        // 
        // userPanel
        // 
        userPanel.BackColor = Color.FromArgb(25, 38, 61);
        userPanel.Controls.Add(_logout);
        userPanel.Controls.Add(_role);
        userPanel.Controls.Add(_userName);
        userPanel.Dock = DockStyle.Bottom;
        userPanel.Location = new Point(16, 531);
        userPanel.Name = "userPanel";
        userPanel.Padding = new Padding(10);
        userPanel.Size = new Size(210, 104);
        userPanel.TabIndex = 2;
        // 
        // _logout
        // 
        _logout.BackColor = Color.FromArgb(39, 54, 79);
        _logout.Cursor = Cursors.Hand;
        _logout.Dock = DockStyle.Bottom;
        _logout.FlatAppearance.BorderSize = 0;
        _logout.FlatStyle = FlatStyle.Flat;
        _logout.ForeColor = Color.White;
        _logout.Location = new Point(10, 60);
        _logout.Name = "_logout";
        _logout.Size = new Size(190, 34);
        _logout.TabIndex = 0;
        _logout.Text = "Đăng xuất";
        _logout.TextAlign = ContentAlignment.MiddleLeft;
        _logout.UseVisualStyleBackColor = false;
        _logout.Click += _logout_Click;
        // 
        // _role
        // 
        _role.Dock = DockStyle.Top;
        _role.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        _role.ForeColor = Color.FromArgb(157, 174, 203);
        _role.Location = new Point(10, 34);
        _role.Name = "_role";
        _role.Size = new Size(190, 22);
        _role.TabIndex = 1;
        // 
        // _userName
        // 
        _userName.Dock = DockStyle.Top;
        _userName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _userName.ForeColor = Color.White;
        _userName.Location = new Point(10, 10);
        _userName.Name = "_userName";
        _userName.Size = new Size(190, 24);
        _userName.TabIndex = 2;
        // 
        // _pageTitle
        // 
        _pageTitle.AutoSize = true;
        _pageTitle.Font = new Font("Segoe UI", 21F, FontStyle.Bold);
        _pageTitle.ForeColor = Color.FromArgb(31, 41, 55);
        _pageTitle.Location = new Point(0, 0);
        _pageTitle.Name = "_pageTitle";
        _pageTitle.Size = new Size(198, 47);
        _pageTitle.TabIndex = 0;
        _pageTitle.Text = "Tổng quan";
        // 
        // _hello
        // 
        _hello.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _hello.AutoSize = true;
        _hello.ForeColor = Color.FromArgb(107, 114, 128);
        _hello.Location = new Point(1400, 8);
        _hello.Name = "_hello";
        _hello.Size = new Size(0, 21);
        _hello.TabIndex = 1;
        // 
        // topbar
        // 
        topbar.BackColor = Color.White;
        topbar.Controls.Add(_pageTitle);
        topbar.Controls.Add(_hello);
        topbar.Dock = DockStyle.Top;
        topbar.Location = new Point(0, 0);
        topbar.Name = "topbar";
        topbar.Padding = new Padding(28, 20, 28, 12);
        topbar.Size = new Size(840, 74);
        topbar.TabIndex = 1;
        // 
        // main
        // 
        main.BackColor = Color.FromArgb(245, 247, 251);
        main.Controls.Add(_content);
        main.Controls.Add(topbar);
        main.Dock = DockStyle.Fill;
        main.Location = new Point(242, 0);
        main.Name = "main";
        main.Size = new Size(840, 653);
        main.TabIndex = 0;
        // 
        // MainForm
        // 
        BackColor = Color.FromArgb(245, 247, 251);
        ClientSize = new Size(1082, 653);
        Controls.Add(main);
        Controls.Add(_sidebar);
        Font = new Font("Segoe UI", 9.5F);
        MinimumSize = new Size(1100, 700);
        Name = "MainForm";
        Text = "PC Builder | Hệ thống Build PC";
        WindowState = FormWindowState.Maximized;
        FormClosing += MainForm_FormClosing;
        _sidebar.ResumeLayout(false);
        brand.ResumeLayout(false);
        brand.PerformLayout();
        userPanel.ResumeLayout(false);
        topbar.ResumeLayout(false);
        topbar.PerformLayout();
        main.ResumeLayout(false);
        ResumeLayout(false);
    }
    protected override void Dispose(bool disposing){if(disposing)components?.Dispose();base.Dispose(disposing);}
}
