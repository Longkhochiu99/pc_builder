using PCBuilder.Data;

namespace PCBuilder.Forms;

public sealed partial class LoginForm : Form
{
    private bool _isOpeningMainForm = false;
    private bool _isLoggingOut = false;
    private bool _isExiting = false;

    public LoginForm()
    {
        InitializeComponent();
        SetupCustomEvents();
    }

    private void SetupCustomEvents()
    {
        _showPassword.CheckedChanged += (s, e) =>
        {
            _password.UseSystemPasswordChar = !_showPassword.Checked;
        };

        login.MouseEnter += (s, e) =>
        {
            login.BackColor = Color.FromArgb(29, 78, 216); // #1D4ED8
        };
        login.MouseLeave += (s, e) =>
        {
            login.BackColor = Color.FromArgb(37, 99, 235); // #2563EB
        };

        hero.Paint += Hero_Paint;
        right.Resize += (s, e) => CenterFormCard();
        CenterFormCard();
    }

    private void CenterFormCard()
    {
        if (formCard == null || right == null) return;
        formCard.Location = new Point(
            Math.Max(30, (right.Width - formCard.Width) / 2),
            Math.Max(25, (right.Height - formCard.Height) / 2)
        );
    }

    private void Hero_Paint(object? sender, PaintEventArgs e)
    {
        using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
            hero.ClientRectangle,
            Color.FromArgb(15, 23, 42),
            Color.FromArgb(30, 58, 138),
            55F);
        e.Graphics.FillRectangle(brush, hero.ClientRectangle);

        using var orb1 = new SolidBrush(Color.FromArgb(22, 96, 165, 250));
        e.Graphics.FillEllipse(orb1, hero.Width - 130, -50, 240, 240);

        using var orb2 = new SolidBrush(Color.FromArgb(18, 59, 130, 246));
        e.Graphics.FillEllipse(orb2, -70, hero.Height - 170, 260, 260);
    }

    private void Login(object? sender, EventArgs e)
    {
        _message.Text = string.Empty;

        if (string.IsNullOrWhiteSpace(_username.Text) ||
            string.IsNullOrEmpty(_password.Text))
        {
            _message.Text = "⚠️ Vui lòng nhập đầy đủ tên tài khoản và mật khẩu.";
            return;
        }

        try
        {
            var user = new AuthRepository().Login(
                _username.Text,
                _password.Text
            );

            if (user is null)
            {
                _message.Text = "❌ Tên đăng nhập hoặc mật khẩu không đúng (hoặc tài khoản đã bị khóa).";
                return;
            }

            _isOpeningMainForm = true;
            Hide();

            using (var main = new MainForm(user))
            {
                main.ShowDialog();

                _isLoggingOut = main.IsLoggingOut;
                _isExiting = main.IsExiting;
            }

            _isOpeningMainForm = false;

            if (_isLoggingOut)
            {
                _isLoggingOut = false;

                _username.Clear();
                _password.Clear();
                _message.Text = string.Empty;

                Show();
                Activate();
            }
            else
            {
                Close();
            }
        }
        catch (Exception ex)
        {
            _isOpeningMainForm = false;

            MessageBox.Show(
                "Không thể đăng nhập. Hãy kiểm tra kết nối SQL Server và connection string.\n\n"
                + ex.Message,
                "Lỗi kết nối CSDL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void OpenRegister(object? sender, EventArgs e)
    {
        using var f = new RegisterForm();
        f.ShowDialog(this);
    }

    private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_isOpeningMainForm || _isExiting)
            return;

        DialogResult result = MessageBox.Show(
            "Bạn có muốn thoát chương trình không?",
            "Xác nhận thoát",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (result == DialogResult.No)
        {
            e.Cancel = true;
        }
    }
}
