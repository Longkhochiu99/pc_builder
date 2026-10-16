using PCBuilder.Data;
namespace PCBuilder.Forms;
public sealed partial class RegisterForm : Form
{
    public RegisterForm() { InitializeComponent(); }
    private void Create(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_username.Text) || string.IsNullOrWhiteSpace(_fullName.Text) || string.IsNullOrWhiteSpace(_email.Text)) { MessageBox.Show("Vui lòng nhập đầy đủ thông tin."); return; }
        if (_password.Text.Length < 8) { MessageBox.Show("Mật khẩu phải có ít nhất 8 ký tự."); return; }
        if (_password.Text != _confirm.Text) { MessageBox.Show("Mật khẩu xác nhận không khớp."); return; }
        try { new AuthRepository().RegisterUser(_username.Text, _email.Text, _fullName.Text, _password.Text); MessageBox.Show("Đăng ký thành công! Bạn có thể đăng nhập bằng tài khoản vừa tạo.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information); DialogResult=DialogResult.OK; Close(); }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is 2601 or 2627) { MessageBox.Show("Tên tài khoản hoặc email đã được sử dụng.", "Không thể đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        catch (Exception ex) { MessageBox.Show("Không thể tạo tài khoản. Kiểm tra kết nối database.\n\n"+ex.Message,"Lỗi",MessageBoxButtons.OK,MessageBoxIcon.Error); }
    }
}
