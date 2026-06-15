using Firebase.Auth;
using Google.Apis.Auth.OAuth2;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class dangnhap : Form
    {
        private bool _passwordVisible = false;
        private string emailPlaceholder = "Nhập email của bạn";
        private string passwordPlaceholder = "Nhập mật khẩu của bạn";

        public dangnhap()
        {
            InitializeComponent();
            SetupPlaceholders(); 
        }

        private void SetupPlaceholders()
        {
            if (buttonShowPassword != null) buttonShowPassword.BringToFront();

            // Set initial text WITHOUT event handlers attached yet
            textBox1.Text = emailPlaceholder;
            textBox1.ForeColor = Color.Gray;

            textBox2.Text = passwordPlaceholder;
            textBox2.ForeColor = Color.Gray;
            textBox2.UseSystemPasswordChar = false;

            // NOW attach event handlers to avoid circular triggering
            textBox1.Enter += TextBoxEmail_Enter;
            textBox1.Leave += TextBoxEmail_Leave;
            textBox2.Enter += TextBoxPassword_Enter;
            textBox2.Leave += TextBoxPassword_Leave;
        }

        private void TextBoxEmail_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == emailPlaceholder || textBox1.ForeColor == Color.Red)
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void TextBoxEmail_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = emailPlaceholder;
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void TextBoxPassword_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == passwordPlaceholder || textBox2.ForeColor == Color.Red)
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
                textBox2.UseSystemPasswordChar = !_passwordVisible;
            }
        }

        private void TextBoxPassword_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                textBox2.UseSystemPasswordChar = false; 
                textBox2.Text = passwordPlaceholder;
                textBox2.ForeColor = Color.Gray;
            }
        }

        private void ShowEmailError(string message)
        {
            textBox1.Text = message;
            textBox1.ForeColor = Color.Red;
        }

        private void ShowPasswordError(string message)
        {
            textBox2.UseSystemPasswordChar = false;
            textBox2.Text = message;
            textBox2.ForeColor = Color.Red;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            string email = textBox1.Text.Trim();
            string password = textBox2.Text;
            bool hasError = false;

            if (string.IsNullOrWhiteSpace(email) || email == emailPlaceholder || textBox1.ForeColor == Color.Red)
            {
                ShowEmailError("Vui lòng nhập email.");
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(password) || password == passwordPlaceholder || textBox2.ForeColor == Color.Red)
            {
                ShowPasswordError("Vui lòng nhập mật khẩu.");
                hasError = true;
            }
            else if (password.Length < 6)
            {
                ShowPasswordError("Mật khẩu ít nhất 6 ký tự.");
                hasError = true;
            }

            if (hasError)
            {
                button1.Enabled = true;
                return;
            }

            try
            {
                string result = await FirebaseAuthService.Auth(email, password, isSignUp: false);
                if (result.StartsWith("Success|"))
                {
                    var parts = result.Split('|');
                    string idToken = parts.Length > 1 ? parts[1] : null;
                    string localId = parts.Length > 2 ? parts[2] : null;

                    AuthSession.FirebaseIdToken = idToken;
                    AuthSession.FirebaseLocalId = localId;

                    MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MainForm mainForm = new MainForm();
                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    string errorMessage = result.Split('|')[1];
                    MessageBox.Show("Đăng nhập thất bại: " + errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button1.Enabled = true;
            }
        }

        private void buttonShowPassword_Click(object sender, EventArgs e)
        {
            if (textBox2.ForeColor == Color.Gray || textBox2.ForeColor == Color.Red) return;

            _passwordVisible = !_passwordVisible;

            if (_passwordVisible)
            {
                textBox2.UseSystemPasswordChar = false; 
                if (buttonShowPassword != null) buttonShowPassword.Text = "🙈"; 
            }
            else
            {
                textBox2.UseSystemPasswordChar = true; 
                if (buttonShowPassword != null) buttonShowPassword.Text = "👁"; 
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button2.Text = "Đang mở trình duyệt...";

            try
            {
                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = AppConfig.GoogleClientId,
                        ClientSecret = AppConfig.GoogleClientSecret
                    },
                    new[] { "openid", "email", "profile" },
                    "user",
                    CancellationToken.None);

                string googleIdToken = credential.Token.IdToken;
                string googleAccessToken = credential.Token.AccessToken;
                string googleRefreshToken = credential.Token.RefreshToken;

                if (string.IsNullOrEmpty(googleIdToken))
                {
                    MessageBox.Show("Không lấy được mã định danh từ Google. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                button2.Text = "Đang đăng nhập Firebase...";
                string result = await FirebaseAuthService.LoginWithGoogleAsync(googleIdToken);

                if (result.StartsWith("Success|"))
                {
                    var parts = result.Split('|');
                    string firebaseToken = parts.Length > 1 ? parts[1] : null;
                    string userEmail = parts.Length > 2 ? parts[2] : "";
                    string userName = parts.Length > 3 ? parts[3] : "";

                    AuthSession.FirebaseIdToken = firebaseToken;
                    AuthSession.GoogleIdToken = googleIdToken;
                    AuthSession.GoogleAccessToken = googleAccessToken;
                    if (!string.IsNullOrEmpty(googleRefreshToken)) AuthSession.GoogleRefreshToken = googleRefreshToken;

                    MessageBox.Show($"Đăng nhập Google thành công!\nXin chào: {userName} ({userEmail})", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    MainForm mainForm = new MainForm();
                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    string errorMessage = result.Split('|')[1];
                    MessageBox.Show("Đăng nhập Firebase thất bại: " + errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đăng nhập Google thất bại hoặc bị hủy: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button2.Enabled = true;
                button2.Text = "Đăng nhập với Google";
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            dangky dangky = new dangky();
            dangky.Show();
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            Forget_verify forget = new Forget_verify();
            forget.Show();
            this.Hide();
        }

        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void dangnhap_Load(object sender, EventArgs e) { }

        private async void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button3.Text = "Đang mở Facebook...";

            try
            {
                string fbTokenResult = await FirebaseAuthService.GetFacebookAccessTokenAsync();

                if (!fbTokenResult.StartsWith("Success|"))
                {
                    MessageBox.Show(fbTokenResult.Split('|')[1], "Lỗi Facebook", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string fbAccessToken = fbTokenResult.Split('|')[1];
                button3.Text = "Đang kết nối Firebase...";

                string loginResult = await FirebaseAuthService.LoginWithFacebookAsync(fbAccessToken);

                if (loginResult.StartsWith("Success|"))
                {
                    var parts = loginResult.Split('|');
                    string idToken = parts[1];
                    string localId = parts[2];
                    string email = parts[3];
                    string userName = parts[4];
                    bool isNewUser = bool.Parse(parts[5]); 

                    if (isNewUser)
                    {
                        await FirebaseAuthService.SaveUserProfileAsync(idToken, localId, email, userName, "");
                        MessageBox.Show($"Chào mừng {userName} lần đầu đến với ứng dụng!\nDữ liệu của bạn đã được khởi tạo thành công.", "Đăng ký thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Mừng {userName} quay trở lại!", "Đăng nhập thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    AuthSession.FirebaseIdToken = idToken;
                    AuthSession.FirebaseLocalId = localId;
                    AuthSession.FacebookAccessToken = fbAccessToken;

                    MainForm mainForm = new MainForm();
                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Đăng nhập Firebase thất bại: " + loginResult.Split('|')[1], "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button3.Enabled = true;
                button3.Text = "f  Facebook";
            }
        }
    }
}