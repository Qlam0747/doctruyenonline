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
            SetupPlaceholders(); // Gọi hàm cài đặt UI ngay khi mở form
        }

        // ================= CÀI ĐẶT PLACEHOLDER =================
        private void SetupPlaceholders()
        {
            // Đảm bảo nút con mắt luôn nổi lên trên cùng
            if (buttonShowPassword != null) buttonShowPassword.BringToFront();

            // Cài đặt cho ô Email
            textBox1.Text = emailPlaceholder;
            textBox1.ForeColor = Color.Gray;
            textBox1.Enter += TextBoxEmail_Enter;
            textBox1.Leave += TextBoxEmail_Leave;

            // Cài đặt cho ô Password
            textBox2.Text = passwordPlaceholder;
            textBox2.ForeColor = Color.Gray;
            textBox2.UseSystemPasswordChar = false; // Ban đầu hiện chữ để người dùng đọc được placeholder
            textBox2.Enter += TextBoxPassword_Enter;
            textBox2.Leave += TextBoxPassword_Leave;
        }

        // ================= XỬ LÝ SỰ KIỆN CLICK VÀO/RA Ô EMAIL =================
        private void TextBoxEmail_Enter(object sender, EventArgs e)
        {
            // Xóa chữ nếu đang là placeholder HOẶC đang báo lỗi đỏ
            if (textBox1.Text == emailPlaceholder || textBox1.ForeColor == Color.Red)
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void TextBoxEmail_Leave(object sender, EventArgs e)
        {
            // Nếu người dùng không nhập gì, trả lại placeholder
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = emailPlaceholder;
                textBox1.ForeColor = Color.Gray;
            }
        }

        // ================= XỬ LÝ SỰ KIỆN CLICK VÀO/RA Ô PASSWORD =================
        private void TextBoxPassword_Enter(object sender, EventArgs e)
        {
            // Xóa chữ nếu đang là placeholder HOẶC đang báo lỗi đỏ
            if (textBox2.Text == passwordPlaceholder || textBox2.ForeColor == Color.Red)
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
                // Bật che mật khẩu dựa theo trạng thái của nút con mắt
                textBox2.UseSystemPasswordChar = !_passwordVisible;
            }
        }

        private void TextBoxPassword_Leave(object sender, EventArgs e)
        {
            // Nếu người dùng không nhập gì, trả lại placeholder
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                textBox2.UseSystemPasswordChar = false; // Tắt dấu chấm để hiện chữ placeholder
                textBox2.Text = passwordPlaceholder;
                textBox2.ForeColor = Color.Gray;
            }
        }

        // ================= CÁC HÀM BÁO LỖI VÀO PLACEHOLDER =================
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

        // ================= XỬ LÝ NÚT ĐĂNG NHẬP (EMAIL/PASS) =================
        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            string email = textBox1.Text.Trim();
            string password = textBox2.Text;
            bool hasError = false;

            // Kiểm tra xem người dùng có bỏ trống (hoặc để nguyên placeholder) không
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

            // Nếu có lỗi thì dừng lại, chờ người dùng nhập đúng
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

                    // Lưu phiên hiện tại vào AuthSession
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

        // ================= XỬ LÝ NÚT ẨN/HIỆN MẬT KHẨU =================
        private void buttonShowPassword_Click(object sender, EventArgs e)
        {
            // Nếu TextBox đang hiện chữ Placeholder hoặc Báo lỗi đỏ thì không làm gì cả
            if (textBox2.ForeColor == Color.Gray || textBox2.ForeColor == Color.Red) return;

            _passwordVisible = !_passwordVisible;

            if (_passwordVisible)
            {
                textBox2.UseSystemPasswordChar = false; // Hiện chữ
                if (buttonShowPassword != null) buttonShowPassword.Text = "🙈"; // Đổi icon thành khỉ che mắt
            }
            else
            {
                textBox2.UseSystemPasswordChar = true; // Che dấu chấm
                if (buttonShowPassword != null) buttonShowPassword.Text = "👁"; // Đổi icon thành con mắt
            }
        }

        // ================= XỬ LÝ NÚT ĐĂNG NHẬP GOOGLE =================
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

                    // Lưu session (bao gồm refresh/access token nếu có)
                    AuthSession.FirebaseIdToken = firebaseToken;
                    AuthSession.GoogleIdToken = googleIdToken;
                    AuthSession.GoogleAccessToken = googleAccessToken;
                    // refresh token có thể null nếu Google không trả lại (lần đầu cần offline access)
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

        // ================= CÁC CHUYỂN HƯỚNG FORM KHÁC =================
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

        // Xóa logic cũ trong TextChanged vì nó làm nhiễu UX của Placeholder
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
                // 1. Lấy Token từ Facebook
                string fbTokenResult = await FirebaseAuthService.GetFacebookAccessTokenAsync();

                if (!fbTokenResult.StartsWith("Success|"))
                {
                    MessageBox.Show(fbTokenResult.Split('|')[1], "Lỗi Facebook", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string fbAccessToken = fbTokenResult.Split('|')[1];
                button3.Text = "Đang kết nối Firebase...";

                // 2. Gửi Token lên Firebase
                string loginResult = await FirebaseAuthService.LoginWithFacebookAsync(fbAccessToken);

                if (loginResult.StartsWith("Success|"))
                {
                    var parts = loginResult.Split('|');
                    string idToken = parts[1];
                    string localId = parts[2];
                    string email = parts[3];
                    string userName = parts[4];
                    bool isNewUser = bool.Parse(parts[5]); // Lấy trạng thái Người dùng mới

                    // 3. KIỂM TRA LẦN ĐẦU ĐĂNG NHẬP ĐỂ LƯU DATABASE
                    if (isNewUser)
                    {
                        // Ghi dữ liệu lên Realtime Database (Để trống ngày sinh vì FB không trả về mặc định)
                        await FirebaseAuthService.SaveUserProfileAsync(idToken, localId, email, userName, "");
                        MessageBox.Show($"Chào mừng {userName} lần đầu đến với ứng dụng!\nDữ liệu của bạn đã được khởi tạo thành công.", "Đăng ký thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Mừng {userName} quay trở lại!", "Đăng nhập thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Lưu session (Firebase + Facebook token)
                    AuthSession.FirebaseIdToken = idToken;
                    AuthSession.FirebaseLocalId = localId;
                    AuthSession.FacebookAccessToken = fbAccessToken;

                    // Chuyển sang Form chính
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