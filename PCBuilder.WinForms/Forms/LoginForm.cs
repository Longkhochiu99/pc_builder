using PCBuilder.Data;
namespace PCBuilder.Forms;
public sealed partial class LoginForm : Form
{
    public LoginForm() { InitializeComponent(); }
    private void Login(object? sender, EventArgs e)
    {
        _message.Text = string.Empty;
        if (string.IsNullOrWhiteSpace(_username.Text) || string.IsNullOrEmpty(_password.Text)) { _message.Text = "Vui lòng nhập tài khoản và mật khẩu."; return; }
        try { var user = new AuthRepository().Login(_username.Text, _password.Text); if (user is null) { _message.Text = "Thông tin đăng nhập không đúng hoặc tài khoản đã bị khóa."; return; } Hide(); using (var main = new MainForm(user)) main.ShowDialog(); Close(); }
        catch (Exception ex) { MessageBox.Show("Không thể đăng nhập. Hãy kiểm tra SQL Server và connection string.\n\n" + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
    private void OpenRegister(object? sender, EventArgs e) { using var f = new RegisterForm(); f.ShowDialog(this); }
}
