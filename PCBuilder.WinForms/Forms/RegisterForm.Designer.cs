namespace PCBuilder.Forms;
partial class RegisterForm
{
    private System.ComponentModel.IContainer? components;

    private Label title=null!; private Label subtitle=null!; private TableLayoutPanel table=null!; private Button create=null!; private Label lblUsername=null!,lblFullName=null!,lblEmail=null!,lblPassword=null!,lblConfirm=null!;
    private TextBox _username=null!,_fullName=null!,_email=null!,_password=null!,_confirm=null!;
    private void InitializeComponent()
    {
        _username = new TextBox();
        _fullName = new TextBox();
        _email = new TextBox();
        _password = new TextBox();
        _confirm = new TextBox();
        title = new Label();
        subtitle = new Label();
        table = new TableLayoutPanel();
        lblUsername = new Label();
        lblFullName = new Label();
        lblEmail = new Label();
        lblPassword = new Label();
        lblConfirm = new Label();
        create = new Button();
        table.SuspendLayout();
        SuspendLayout();
        // 
        // _username
        // 
        _username.Dock = DockStyle.Fill;
        _username.Location = new Point(3, 27);
        _username.Name = "_username";
        _username.PlaceholderText = "Ví dụ: nguyenvana";
        _username.Size = new Size(384, 27);
        _username.TabIndex = 1;
        // 
        // _fullName
        // 
        _fullName.Dock = DockStyle.Fill;
        _fullName.Location = new Point(3, 85);
        _fullName.Name = "_fullName";
        _fullName.PlaceholderText = "Họ và tên";
        _fullName.Size = new Size(384, 27);
        _fullName.TabIndex = 3;
        // 
        // _email
        // 
        _email.Dock = DockStyle.Fill;
        _email.Location = new Point(3, 143);
        _email.Name = "_email";
        _email.PlaceholderText = "email@example.com";
        _email.Size = new Size(384, 27);
        _email.TabIndex = 5;
        // 
        // _password
        // 
        _password.Dock = DockStyle.Fill;
        _password.Location = new Point(3, 201);
        _password.Name = "_password";
        _password.PlaceholderText = "Ít nhất 8 ký tự";
        _password.Size = new Size(384, 27);
        _password.TabIndex = 7;
        _password.UseSystemPasswordChar = true;
        // 
        // _confirm
        // 
        _confirm.Dock = DockStyle.Fill;
        _confirm.Location = new Point(3, 259);
        _confirm.Name = "_confirm";
        _confirm.PlaceholderText = "Nhập lại mật khẩu";
        _confirm.Size = new Size(384, 27);
        _confirm.TabIndex = 9;
        _confirm.UseSystemPasswordChar = true;
        // 
        // title
        // 
        title.AutoSize = true;
        title.Font = new Font("Segoe UI", 21F, FontStyle.Bold);
        title.ForeColor = Color.FromArgb(31, 41, 55);
        title.Location = new Point(34, 24);
        title.Name = "title";
        title.Size = new Size(329, 47);
        title.TabIndex = 0;
        title.Text = "Tạo tài khoản User";
        // 
        // subtitle
        // 
        subtitle.AutoSize = true;
        subtitle.ForeColor = Color.FromArgb(107, 114, 128);
        subtitle.Location = new Point(37, 67);
        subtitle.Name = "subtitle";
        subtitle.Size = new Size(324, 20);
        subtitle.TabIndex = 1;
        subtitle.Text = "Tài khoản đăng ký mới không có quyền quản trị.";
        // 
        // table
        // 
        table.BackColor = Color.White;
        table.ColumnCount = 1;
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        table.Controls.Add(lblUsername);
        table.Controls.Add(_username);
        table.Controls.Add(lblFullName);
        table.Controls.Add(_fullName);
        table.Controls.Add(lblEmail);
        table.Controls.Add(_email);
        table.Controls.Add(lblPassword);
        table.Controls.Add(_password);
        table.Controls.Add(lblConfirm);
        table.Controls.Add(_confirm);
        table.Location = new Point(36, 110);
        table.Name = "table";
        table.RowCount = 10;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        table.Size = new Size(390, 280);
        table.TabIndex = 2;
        // 
        // lblUsername
        // 
        lblUsername.Anchor = AnchorStyles.Left;
        lblUsername.AutoSize = true;
        lblUsername.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblUsername.ForeColor = Color.FromArgb(31, 41, 55);
        lblUsername.Location = new Point(3, 2);
        lblUsername.Name = "lblUsername";
        lblUsername.Size = new Size(103, 20);
        lblUsername.TabIndex = 0;
        lblUsername.Text = "Tên tài khoản";
        // 
        // lblFullName
        // 
        lblFullName.Anchor = AnchorStyles.Left;
        lblFullName.AutoSize = true;
        lblFullName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblFullName.ForeColor = Color.FromArgb(31, 41, 55);
        lblFullName.Location = new Point(3, 60);
        lblFullName.Name = "lblFullName";
        lblFullName.Size = new Size(76, 20);
        lblFullName.TabIndex = 2;
        lblFullName.Text = "Họ và tên";
        // 
        // lblEmail
        // 
        lblEmail.Anchor = AnchorStyles.Left;
        lblEmail.AutoSize = true;
        lblEmail.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblEmail.ForeColor = Color.FromArgb(31, 41, 55);
        lblEmail.Location = new Point(3, 118);
        lblEmail.Name = "lblEmail";
        lblEmail.Size = new Size(47, 20);
        lblEmail.TabIndex = 4;
        lblEmail.Text = "Email";
        // 
        // lblPassword
        // 
        lblPassword.Anchor = AnchorStyles.Left;
        lblPassword.AutoSize = true;
        lblPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPassword.ForeColor = Color.FromArgb(31, 41, 55);
        lblPassword.Location = new Point(3, 176);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new Size(75, 20);
        lblPassword.TabIndex = 6;
        lblPassword.Text = "Mật khẩu";
        // 
        // lblConfirm
        // 
        lblConfirm.Anchor = AnchorStyles.Left;
        lblConfirm.AutoSize = true;
        lblConfirm.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblConfirm.ForeColor = Color.FromArgb(31, 41, 55);
        lblConfirm.Location = new Point(3, 234);
        lblConfirm.Name = "lblConfirm";
        lblConfirm.Size = new Size(143, 20);
        lblConfirm.TabIndex = 8;
        lblConfirm.Text = "Xác nhận mật khẩu";
        // 
        // create
        // 
        create.Location = new Point(36, 410);
        create.Name = "create";
        create.Size = new Size(390, 42);
        create.TabIndex = 3;
        create.Text = "Tạo tài khoản";
        create.Click += Create;
        // 
        // RegisterForm
        // 
        AcceptButton = create;
        BackColor = Color.White;
        ClientSize = new Size(460, 490);
        Controls.Add(title);
        Controls.Add(subtitle);
        Controls.Add(table);
        Controls.Add(create);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "RegisterForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "PC Builder | Tạo tài khoản";
        table.ResumeLayout(false);
        table.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
    protected override void Dispose(bool disposing) { if(disposing) components?.Dispose(); base.Dispose(disposing); }
}
