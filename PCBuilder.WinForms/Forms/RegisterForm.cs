using PCBuilder.Data;

namespace PCBuilder.Forms;

public sealed class RegisterForm : Form
{
    private readonly TextBox _username = new() { Width = 310, PlaceholderText = "Ví dụ: nguyenvana" };
    private readonly TextBox _fullName = new() { Width = 310, PlaceholderText = "Họ và tên" };
    private readonly TextBox _email = new() { Width = 310, PlaceholderText = "email@example.com" };
    private readonly TextBox _password = new() { Width = 310, UseSystemPasswordChar = true, PlaceholderText = "Ít nhất 8 ký tự" };
    private readonly TextBox _confirm = new() { Width = 310, UseSystemPasswordChar = true, PlaceholderText = "Nhập lại mật khẩu" };

    public RegisterForm()
    {
        Text = "PC Builder | Tạo tài khoản"; StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
        ClientSize = new Size(460, 490); BackColor = UiTheme.Surface;
        var title = new Label { Text = "Tạo tài khoản User", Font = new Font("Segoe UI", 21, FontStyle.Bold), AutoSize = true, Location = new Point(34, 24), ForeColor = UiTheme.Text };
        var subtitle = new Label { Text = "Tài khoản đăng ký mới không có quyền quản trị.", AutoSize = true, Location = new Point(37, 67), ForeColor = UiTheme.Muted };
        var table = new TableLayoutPanel { Location = new Point(36, 110), Size = new Size(390, 280), ColumnCount = 1, RowCount = 10, BackColor = UiTheme.Surface };
        AddField(table, "Tên tài khoản", _username); AddField(table, "Họ và tên", _fullName); AddField(table, "Email", _email); AddField(table, "Mật khẩu", _password); AddField(table, "Xác nhận mật khẩu", _confirm);
        var create = UiTheme.MakeButton("Tạo tài khoản"); create.Location = new Point(36, 410); create.Width = 390; create.Height = 42; create.Click += Create;
        Controls.AddRange(new Control[] { title, subtitle, table, create }); AcceptButton = create;
    }

    private static void AddField(TableLayoutPanel table, string label, TextBox input)
    {
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 24)); table.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        var l = new Label { Text = label, AutoSize = true, ForeColor = UiTheme.Text, Font = new Font("Segoe UI", 9, FontStyle.Bold), Anchor = AnchorStyles.Left };
        input.Dock = DockStyle.Fill; table.Controls.Add(l); table.Controls.Add(input);
    }

    private void Create(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_username.Text) || string.IsNullOrWhiteSpace(_fullName.Text) || string.IsNullOrWhiteSpace(_email.Text)) { MessageBox.Show("Vui lòng nhập đầy đủ thông tin."); return; }
        if (_password.Text.Length < 8) { MessageBox.Show("Mật khẩu phải có ít nhất 8 ký tự."); return; }
        if (_password.Text != _confirm.Text) { MessageBox.Show("Mật khẩu xác nhận không khớp."); return; }
        try
        {
            new AuthRepository().RegisterUser(_username.Text, _email.Text, _fullName.Text, _password.Text);
            MessageBox.Show("Đăng ký thành công! Bạn có thể đăng nhập bằng tài khoản vừa tạo.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK; Close();
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is 2601 or 2627)
        { MessageBox.Show("Tên tài khoản hoặc email đã được sử dụng.", "Không thể đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        catch (Exception ex) { MessageBox.Show("Không thể tạo tài khoản. Kiểm tra kết nối database.\n\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
