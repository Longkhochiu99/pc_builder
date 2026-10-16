namespace PCBuilder.Forms;
partial class RegisterForm
{
    private System.ComponentModel.IContainer? components;

    private Label title=null!; private Label subtitle=null!; private TableLayoutPanel table=null!; private Button create=null!; private Label lblUsername=null!,lblFullName=null!,lblEmail=null!,lblPassword=null!,lblConfirm=null!;
    private TextBox _username=null!,_fullName=null!,_email=null!,_password=null!,_confirm=null!;
    private void InitializeComponent()
    {
        components=new System.ComponentModel.Container(); _username=new TextBox(); _fullName=new TextBox(); _email=new TextBox(); _password=new TextBox(); _confirm=new TextBox();
        title=new Label(); subtitle=new Label(); table=new TableLayoutPanel(); create=new Button(); lblUsername=new Label(); lblFullName=new Label(); lblEmail=new Label(); lblPassword=new Label(); lblConfirm=new Label();
        SuspendLayout(); Text="PC Builder | Tạo tài khoản"; StartPosition=FormStartPosition.CenterParent; FormBorderStyle=FormBorderStyle.FixedDialog; MaximizeBox=false; MinimizeBox=false; ClientSize=new Size(460,490); BackColor=UiTheme.Surface;
        title.Text="Tạo tài khoản User"; title.Font=new Font("Segoe UI",21,FontStyle.Bold); title.AutoSize=true; title.Location=new Point(34,24); title.ForeColor=UiTheme.Text;
        subtitle.Text="Tài khoản đăng ký mới không có quyền quản trị."; subtitle.AutoSize=true; subtitle.Location=new Point(37,67); subtitle.ForeColor=UiTheme.Muted;
        table.Location=new Point(36,110); table.Size=new Size(390,280); table.ColumnCount=1; table.RowCount=10; table.BackColor=UiTheme.Surface;
        
        lblUsername.Text="Tên tài khoản"; lblFullName.Text="Họ và tên"; lblEmail.Text="Email"; lblPassword.Text="Mật khẩu"; lblConfirm.Text="Xác nhận mật khẩu";
        lblUsername.AutoSize=true;lblFullName.AutoSize=true;lblEmail.AutoSize=true;lblPassword.AutoSize=true;lblConfirm.AutoSize=true;
        lblUsername.ForeColor=UiTheme.Text;lblFullName.ForeColor=UiTheme.Text;lblEmail.ForeColor=UiTheme.Text;lblPassword.ForeColor=UiTheme.Text;lblConfirm.ForeColor=UiTheme.Text;
        lblUsername.Font=new Font("Segoe UI",9,FontStyle.Bold);lblFullName.Font=new Font("Segoe UI",9,FontStyle.Bold);lblEmail.Font=new Font("Segoe UI",9,FontStyle.Bold);lblPassword.Font=new Font("Segoe UI",9,FontStyle.Bold);lblConfirm.Font=new Font("Segoe UI",9,FontStyle.Bold);
        lblUsername.Anchor=AnchorStyles.Left;lblFullName.Anchor=AnchorStyles.Left;lblEmail.Anchor=AnchorStyles.Left;lblPassword.Anchor=AnchorStyles.Left;lblConfirm.Anchor=AnchorStyles.Left;
        _username.Dock=DockStyle.Fill;_fullName.Dock=DockStyle.Fill;_email.Dock=DockStyle.Fill;_password.Dock=DockStyle.Fill;_confirm.Dock=DockStyle.Fill;
        _username.PlaceholderText="Ví dụ: nguyenvana";_fullName.PlaceholderText="Họ và tên";_email.PlaceholderText="email@example.com";_password.PlaceholderText="Ít nhất 8 ký tự";_confirm.PlaceholderText="Nhập lại mật khẩu";_password.UseSystemPasswordChar=true;_confirm.UseSystemPasswordChar=true;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute,24));table.RowStyles.Add(new RowStyle(SizeType.Absolute,34));table.Controls.Add(lblUsername);table.Controls.Add(_username);
        table.RowStyles.Add(new RowStyle(SizeType.Absolute,24));table.RowStyles.Add(new RowStyle(SizeType.Absolute,34));table.Controls.Add(lblFullName);table.Controls.Add(_fullName);
        table.RowStyles.Add(new RowStyle(SizeType.Absolute,24));table.RowStyles.Add(new RowStyle(SizeType.Absolute,34));table.Controls.Add(lblEmail);table.Controls.Add(_email);
        table.RowStyles.Add(new RowStyle(SizeType.Absolute,24));table.RowStyles.Add(new RowStyle(SizeType.Absolute,34));table.Controls.Add(lblPassword);table.Controls.Add(_password);
        table.RowStyles.Add(new RowStyle(SizeType.Absolute,24));table.RowStyles.Add(new RowStyle(SizeType.Absolute,34));table.Controls.Add(lblConfirm);table.Controls.Add(_confirm);
        create.Text="Tạo tài khoản"; create.Location=new Point(36,410); create.Width=390; create.Height=42; create.Click+=Create;
        Controls.AddRange(new Control[]{title,subtitle,table,create}); AcceptButton=create; ResumeLayout(false);
    }
    protected override void Dispose(bool disposing) { if(disposing) components?.Dispose(); base.Dispose(disposing); }
}
