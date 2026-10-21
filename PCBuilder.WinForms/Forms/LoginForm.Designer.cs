namespace PCBuilder.Forms;

partial class LoginForm
{
    private System.ComponentModel.IContainer? components;

    private TableLayoutPanel root = null!;
    private Panel hero = null!;
    private Label heroBadge = null!;
    private Label brand = null!;
    private Label heroText = null!;
    private Label heroSub = null!;
    private Label feat1 = null!;
    private Label feat2 = null!;
    private Label feat3 = null!;
    private Label badge = null!;

    private Panel right = null!;
    private Panel formCard = null!;
    private Label tagLabel = null!;
    private Label formTitle = null!;
    private Label subtitle = null!;
    private Label userLabel = null!;
    private TextBox _username = null!;
    private Label passLabel = null!;
    private TextBox _password = null!;
    private CheckBox _showPassword = null!;
    private Label _message = null!;
    private Button login = null!;
    private Label registerText = null!;
    private LinkLabel register = null!;
    private Panel hintPanel = null!;
    private Label hint = null!;

    private void InitializeComponent()
    {
        root = new TableLayoutPanel();
        hero = new Panel();
        heroBadge = new Label();
        brand = new Label();
        heroText = new Label();
        heroSub = new Label();
        feat1 = new Label();
        feat2 = new Label();
        feat3 = new Label();
        badge = new Label();

        right = new Panel();
        formCard = new Panel();
        tagLabel = new Label();
        formTitle = new Label();
        subtitle = new Label();
        userLabel = new Label();
        _username = new TextBox();
        passLabel = new Label();
        _password = new TextBox();
        _showPassword = new CheckBox();
        _message = new Label();
        login = new Button();
        registerText = new Label();
        register = new LinkLabel();
        hintPanel = new Panel();
        hint = new Label();

        root.SuspendLayout();
        hero.SuspendLayout();
        right.SuspendLayout();
        formCard.SuspendLayout();
        hintPanel.SuspendLayout();
        SuspendLayout();

        // 
        // root
        // 
        root.ColumnCount = 2;
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57F));
        root.Controls.Add(hero, 0, 0);
        root.Controls.Add(right, 1, 0);
        root.Dock = DockStyle.Fill;
        root.Location = new Point(0, 0);
        root.Margin = new Padding(0);
        root.Name = "root";
        root.RowCount = 1;
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        root.Size = new Size(1000, 640);
        root.TabIndex = 0;

        // 
        // hero
        // 
        hero.BackColor = Color.FromArgb(15, 23, 42);
        hero.Controls.Add(heroBadge);
        hero.Controls.Add(brand);
        hero.Controls.Add(heroText);
        hero.Controls.Add(heroSub);
        hero.Controls.Add(feat1);
        hero.Controls.Add(feat2);
        hero.Controls.Add(feat3);
        hero.Controls.Add(badge);
        hero.Dock = DockStyle.Fill;
        hero.Location = new Point(0, 0);
        hero.Margin = new Padding(0);
        hero.Name = "hero";
        hero.Padding = new Padding(40);
        hero.Size = new Size(430, 640);
        hero.TabIndex = 0;

        // 
        // heroBadge
        // 
        heroBadge.AutoSize = true;
        heroBadge.BackColor = Color.Transparent;
        heroBadge.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        heroBadge.ForeColor = Color.FromArgb(147, 197, 253);
        heroBadge.Location = new Point(40, 48);
        heroBadge.Name = "heroBadge";
        heroBadge.Size = new Size(185, 19);
        heroBadge.TabIndex = 0;
        heroBadge.Text = "[ ⚡ PC BUILDER SYSTEM ]";

        // 
        // brand
        // 
        brand.AutoSize = true;
        brand.BackColor = Color.Transparent;
        brand.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
        brand.ForeColor = Color.White;
        brand.Location = new Point(36, 78);
        brand.Name = "brand";
        brand.Size = new Size(286, 62);
        brand.TabIndex = 1;
        brand.Text = "PC BUILDER";

        // 
        // heroText
        // 
        heroText.AutoSize = true;
        heroText.BackColor = Color.Transparent;
        heroText.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        heroText.ForeColor = Color.FromArgb(226, 232, 240);
        heroText.Location = new Point(38, 150);
        heroText.Name = "heroText";
        heroText.Size = new Size(260, 74);
        heroText.TabIndex = 2;
        heroText.Text = "Kiến tạo cỗ máy\nmơ ước của bạn.";

        // 
        // heroSub
        // 
        heroSub.BackColor = Color.Transparent;
        heroSub.Font = new Font("Segoe UI", 9.5F);
        heroSub.ForeColor = Color.FromArgb(148, 163, 184);
        heroSub.Location = new Point(40, 235);
        heroSub.Name = "heroSub";
        heroSub.Size = new Size(350, 48);
        heroSub.TabIndex = 3;
        heroSub.Text = "Nền tảng toàn diện hỗ trợ tra cứu linh kiện, tự động kiểm tra tương thích và tối ưu ngân sách.";

        // 
        // feat1
        // 
        feat1.BackColor = Color.Transparent;
        feat1.Font = new Font("Segoe UI", 9F);
        feat1.ForeColor = Color.FromArgb(203, 213, 225);
        feat1.Location = new Point(40, 300);
        feat1.Name = "feat1";
        feat1.Size = new Size(350, 44);
        feat1.TabIndex = 4;
        feat1.Text = "✦  Tương thích chuẩn xác\n    Kiểm tra Socket, RAM, GPU, PSU tức thì.";

        // 
        // feat2
        // 
        feat2.BackColor = Color.Transparent;
        feat2.Font = new Font("Segoe UI", 9F);
        feat2.ForeColor = Color.FromArgb(203, 213, 225);
        feat2.Location = new Point(40, 355);
        feat2.Name = "feat2";
        feat2.Size = new Size(350, 44);
        feat2.TabIndex = 5;
        feat2.Text = "✦  Gợi ý cấu hình tối ưu ngân sách\n    Thuật toán Beam Search thông minh theo mục đích.";

        // 
        // feat3
        // 
        feat3.BackColor = Color.Transparent;
        feat3.Font = new Font("Segoe UI", 9F);
        feat3.ForeColor = Color.FromArgb(203, 213, 225);
        feat3.Location = new Point(40, 410);
        feat3.Name = "feat3";
        feat3.Size = new Size(350, 44);
        feat3.TabIndex = 6;
        feat3.Text = "✦  Chẩn đoán sự cố phần cứng\n    Hỗ trợ từng bước xác định và xử lý lỗi kỹ thuật.";

        // 
        // badge
        // 
        badge.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        badge.AutoSize = true;
        badge.BackColor = Color.Transparent;
        badge.Font = new Font("Segoe UI", 8.5F);
        badge.ForeColor = Color.FromArgb(100, 116, 139);
        badge.Location = new Point(40, 580);
        badge.Name = "badge";
        badge.Size = new Size(245, 19);
        badge.TabIndex = 7;
        badge.Text = "Phiên bản 2.0  •  .NET 8  •  SQL Server";

        // 
        // right
        // 
        right.BackColor = Color.White;
        right.Controls.Add(formCard);
        right.Dock = DockStyle.Fill;
        right.Location = new Point(430, 0);
        right.Margin = new Padding(0);
        right.Name = "right";
        right.Size = new Size(570, 640);
        right.TabIndex = 1;

        // 
        // formCard
        // 
        formCard.BackColor = Color.White;
        formCard.Controls.Add(tagLabel);
        formCard.Controls.Add(formTitle);
        formCard.Controls.Add(subtitle);
        formCard.Controls.Add(userLabel);
        formCard.Controls.Add(_username);
        formCard.Controls.Add(passLabel);
        formCard.Controls.Add(_password);
        formCard.Controls.Add(_showPassword);
        formCard.Controls.Add(_message);
        formCard.Controls.Add(login);
        formCard.Controls.Add(registerText);
        formCard.Controls.Add(register);
        formCard.Controls.Add(hintPanel);
        formCard.Location = new Point(70, 35);
        formCard.Name = "formCard";
        formCard.Size = new Size(430, 570);
        formCard.TabIndex = 0;

        // 
        // tagLabel
        // 
        tagLabel.AutoSize = true;
        tagLabel.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        tagLabel.ForeColor = Color.FromArgb(37, 99, 235);
        tagLabel.Location = new Point(0, 8);
        tagLabel.Name = "tagLabel";
        tagLabel.Size = new Size(215, 19);
        tagLabel.TabIndex = 0;
        tagLabel.Text = "HỆ THỐNG QUẢN LÝ & BUILD PC";

        // 
        // formTitle
        // 
        formTitle.AutoSize = true;
        formTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
        formTitle.ForeColor = Color.FromArgb(15, 23, 42);
        formTitle.Location = new Point(-2, 30);
        formTitle.Name = "formTitle";
        formTitle.Size = new Size(348, 50);
        formTitle.TabIndex = 1;
        formTitle.Text = "Chào mừng trở lại!";

        // 
        // subtitle
        // 
        subtitle.AutoSize = true;
        subtitle.Font = new Font("Segoe UI", 9.5F);
        subtitle.ForeColor = Color.FromArgb(100, 116, 139);
        subtitle.Location = new Point(0, 84);
        subtitle.Name = "subtitle";
        subtitle.Size = new Size(330, 21);
        subtitle.TabIndex = 2;
        subtitle.Text = "Đăng nhập để tiếp tục xây dựng cấu hình của bạn.";

        // 
        // userLabel
        // 
        userLabel.AutoSize = true;
        userLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        userLabel.ForeColor = Color.FromArgb(71, 85, 105);
        userLabel.Location = new Point(0, 126);
        userLabel.Name = "userLabel";
        userLabel.Size = new Size(117, 19);
        userLabel.TabIndex = 3;
        userLabel.Text = "TÊN TÀI KHOẢN";

        // 
        // _username
        // 
        _username.BackColor = Color.FromArgb(248, 250, 252);
        _username.BorderStyle = BorderStyle.FixedSingle;
        _username.Font = new Font("Segoe UI", 10.5F);
        _username.ForeColor = Color.FromArgb(15, 23, 42);
        _username.Location = new Point(0, 149);
        _username.Name = "_username";
        _username.PlaceholderText = "Nhập tên tài khoản của bạn...";
        _username.Size = new Size(430, 31);
        _username.TabIndex = 4;

        // 
        // passLabel
        // 
        passLabel.AutoSize = true;
        passLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        passLabel.ForeColor = Color.FromArgb(71, 85, 105);
        passLabel.Location = new Point(0, 198);
        passLabel.Name = "passLabel";
        passLabel.Size = new Size(81, 19);
        passLabel.TabIndex = 5;
        passLabel.Text = "MẬT KHẨU";

        // 
        // _password
        // 
        _password.BackColor = Color.FromArgb(248, 250, 252);
        _password.BorderStyle = BorderStyle.FixedSingle;
        _password.Font = new Font("Segoe UI", 10.5F);
        _password.ForeColor = Color.FromArgb(15, 23, 42);
        _password.Location = new Point(0, 221);
        _password.Name = "_password";
        _password.PlaceholderText = "Nhập mật khẩu của bạn...";
        _password.Size = new Size(430, 31);
        _password.TabIndex = 6;
        _password.UseSystemPasswordChar = true;

        // 
        // _showPassword
        // 
        _showPassword.AutoSize = true;
        _showPassword.Cursor = Cursors.Hand;
        _showPassword.Font = new Font("Segoe UI", 9F);
        _showPassword.ForeColor = Color.FromArgb(100, 116, 139);
        _showPassword.Location = new Point(0, 262);
        _showPassword.Name = "_showPassword";
        _showPassword.Size = new Size(149, 24);
        _showPassword.TabIndex = 7;
        _showPassword.Text = "Hiển thị mật khẩu";
        _showPassword.UseVisualStyleBackColor = true;

        // 
        // _message
        // 
        _message.AutoSize = true;
        _message.Font = new Font("Segoe UI", 9F);
        _message.ForeColor = Color.FromArgb(220, 38, 38);
        _message.Location = new Point(0, 292);
        _message.MaximumSize = new Size(430, 0);
        _message.Name = "_message";
        _message.Size = new Size(0, 20);
        _message.TabIndex = 8;

        // 
        // login
        // 
        login.BackColor = Color.FromArgb(37, 99, 235);
        login.Cursor = Cursors.Hand;
        login.FlatAppearance.BorderSize = 0;
        login.FlatStyle = FlatStyle.Flat;
        login.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        login.ForeColor = Color.White;
        login.Location = new Point(0, 326);
        login.Name = "login";
        login.Size = new Size(430, 46);
        login.TabIndex = 9;
        login.Text = "ĐĂNG NHẬP   ➔";
        login.UseVisualStyleBackColor = false;
        login.Click += Login;

        // 
        // registerText
        // 
        registerText.AutoSize = true;
        registerText.Font = new Font("Segoe UI", 9.5F);
        registerText.ForeColor = Color.FromArgb(100, 116, 139);
        registerText.Location = new Point(0, 388);
        registerText.Name = "registerText";
        registerText.Size = new Size(144, 21);
        registerText.TabIndex = 10;
        registerText.Text = "Chưa có tài khoản?";

        // 
        // register
        // 
        register.AutoSize = true;
        register.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        register.LinkColor = Color.FromArgb(37, 99, 235);
        register.ActiveLinkColor = Color.FromArgb(29, 78, 216);
        register.Cursor = Cursors.Hand;
        register.Location = new Point(148, 388);
        register.Name = "register";
        register.Size = new Size(198, 21);
        register.TabIndex = 11;
        register.TabStop = true;
        register.Text = "Đăng ký tài khoản User";
        register.Click += OpenRegister;

        // 
        // hintPanel
        // 
        hintPanel.BackColor = Color.FromArgb(248, 250, 252);
        hintPanel.Controls.Add(hint);
        hintPanel.Location = new Point(0, 435);
        hintPanel.Name = "hintPanel";
        hintPanel.Padding = new Padding(14, 10, 14, 10);
        hintPanel.Size = new Size(430, 68);
        hintPanel.TabIndex = 12;

        // 
        // hint
        // 
        hint.BackColor = Color.Transparent;
        hint.Dock = DockStyle.Fill;
        hint.Font = new Font("Segoe UI", 8.5F);
        hint.ForeColor = Color.FromArgb(100, 116, 139);
        hint.Location = new Point(14, 10);
        hint.Name = "hint";
        hint.Size = new Size(402, 48);
        hint.TabIndex = 0;
        hint.Text = "💡 Lưu ý: Tài khoản Admin được quản lý nội bộ. Tài khoản đăng ký mới sẽ mặc định có vai trò User để tra cứu và lưu cấu hình PC.";

        // 
        // LoginForm
        // 
        AcceptButton = login;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(245, 247, 251);
        ClientSize = new Size(1000, 640);
        Controls.Add(root);
        MinimumSize = new Size(920, 600);
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "PC Builder Pro — Đăng nhập hệ thống";
        FormClosing += LoginForm_FormClosing;
        root.ResumeLayout(false);
        hero.ResumeLayout(false);
        hero.PerformLayout();
        right.ResumeLayout(false);
        formCard.ResumeLayout(false);
        formCard.PerformLayout();
        hintPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }
}
