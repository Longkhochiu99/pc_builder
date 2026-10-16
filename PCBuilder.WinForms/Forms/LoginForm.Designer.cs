namespace PCBuilder.Forms;
partial class LoginForm
{
    private System.ComponentModel.IContainer? components;

    private TableLayoutPanel root=null!; private Panel hero=null!; private Label brand=null!; private Label heroText=null!; private Label heroSub=null!; private Label badge=null!; private Panel right=null!; private Label formTitle=null!; private Label subtitle=null!; private Label userLabel=null!; private Label passLabel=null!; private Button login=null!; private Label registerText=null!; private LinkLabel register=null!; private Label hint=null!;
    private TextBox _username = null!; private TextBox _password = null!; private Label _message = null!;
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container(); _username = new TextBox(); _password = new TextBox(); _message = new Label();
        root=new TableLayoutPanel(); hero=new Panel(); brand=new Label(); heroText=new Label(); heroSub=new Label(); badge=new Label(); right=new Panel(); formTitle=new Label(); subtitle=new Label(); userLabel=new Label(); passLabel=new Label(); login=new Button(); registerText=new Label(); register=new LinkLabel(); hint=new Label();
        SuspendLayout(); Text = "PC Builder | Đăng nhập"; StartPosition = FormStartPosition.CenterScreen; MinimumSize = new Size(900,590); Size = new Size(1000,650); BackColor = UiTheme.Background; Font = UiTheme.BodyFont;
        root.Dock=DockStyle.Fill; root.ColumnCount=2; root.RowCount=1; root.Padding=new Padding(18); root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,48)); root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,52));
        hero.Dock=DockStyle.Fill; hero.BackColor=UiTheme.Sidebar; hero.Padding=new Padding(42);
        brand.Text="PC\nBUILDER"; brand.AutoSize=true; brand.ForeColor=Color.White; brand.Font=new Font("Segoe UI",32,FontStyle.Bold); brand.Location=new Point(38,65);
        heroText.Text="Xây dựng cấu hình PC\nthông minh hơn."; heroText.AutoSize=true; heroText.ForeColor=Color.FromArgb(222,230,245); heroText.Font=new Font("Segoe UI",18,FontStyle.Bold); heroText.Location=new Point(42,205);
        heroSub.Text="Tra cứu linh kiện  •  Kiểm tra tương thích\nTối ưu hiệu năng theo ngân sách"; heroSub.AutoSize=true; heroSub.ForeColor=Color.FromArgb(165,180,205); heroSub.Font=new Font("Segoe UI",11); heroSub.Location=new Point(44,290);
        badge.Text=".NET 8  |  SQL SERVER"; badge.AutoSize=true; badge.ForeColor=Color.FromArgb(190,205,255); badge.Font=new Font("Segoe UI",9,FontStyle.Bold); badge.Location=new Point(44,390); hero.Controls.AddRange(new Control[]{brand,heroText,heroSub,badge});
        right.Dock=DockStyle.Fill; right.BackColor=UiTheme.Surface; right.Padding=new Padding(48,50,48,30);
        formTitle.Text="Chào mừng trở lại"; formTitle.AutoSize=true; formTitle.Font=new Font("Segoe UI",23,FontStyle.Bold); formTitle.ForeColor=UiTheme.Text; formTitle.Location=new Point(42,55);
        subtitle.Text="Đăng nhập để tiếp tục xây dựng cấu hình của bạn."; subtitle.AutoSize=true; subtitle.ForeColor=UiTheme.Muted; subtitle.Location=new Point(45,105);
        userLabel.Text="TÊN TÀI KHOẢN"; userLabel.AutoSize=true; userLabel.Font=new Font("Segoe UI",9,FontStyle.Bold); userLabel.Location=new Point(45,165);
        _username.Location=new Point(45,190); _username.Width=340; _username.Height=38; _username.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right; _username.PlaceholderText="Nhập tên tài khoản";
        passLabel.Text="MẬT KHẨU"; passLabel.AutoSize=true; passLabel.Font=new Font("Segoe UI",9,FontStyle.Bold); passLabel.Location=new Point(45,250);
        _password.Location=new Point(45,275); _password.Width=340; _password.Height=38; _password.UseSystemPasswordChar=true; _password.PlaceholderText="Nhập mật khẩu"; _password.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        login.Text="Đăng nhập"; login.Location=new Point(45,345); login.Width=340; login.Height=46; login.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right; login.Click+=Login;
        registerText.Text="Chưa có tài khoản?"; registerText.AutoSize=true; registerText.ForeColor=UiTheme.Muted; registerText.Location=new Point(45,415);
        register.Text="Đăng ký tài khoản User"; register.AutoSize=true; register.LinkColor=UiTheme.Primary; register.Location=new Point(165,415); register.Font=new Font("Segoe UI",9.5F,FontStyle.Bold); register.Click+=OpenRegister;
        _message.Location=new Point(45,465); _message.AutoSize=true; _message.ForeColor=Color.FromArgb(220,70,70); _message.MaximumSize=new Size(330,0);
        hint.Text="Tài khoản Admin được quản lý riêng. Đăng ký mới chỉ tạo quyền User."; hint.AutoSize=false; hint.Height=45; hint.Width=340; hint.ForeColor=UiTheme.Muted; hint.Location=new Point(45,500); hint.Anchor=AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;
        right.Controls.AddRange(new Control[]{formTitle,subtitle,userLabel,_username,passLabel,_password,login,registerText,register,_message,hint}); root.Controls.Add(hero,0,0); root.Controls.Add(right,1,0); Controls.Add(root); AcceptButton=login; ResumeLayout(false);
    }
    protected override void Dispose(bool disposing) { if(disposing) components?.Dispose(); base.Dispose(disposing); }
}
