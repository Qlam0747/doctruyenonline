using Firebase.Auth;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class dangky : Form
    {
        public dangky()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            // Basic validation
            string email = textBox1.Text.Trim();
            string password = textBox2.Text;
            string confirm = textBoxConfirm.Text;
            string username = textBoxUser.Text.Trim();
            string birthdate = dateTimePicker1.Value.ToString("yyyy-MM-dd");

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Vui lòng nhập tên người dùng.", "Lỗi");
                button1.Enabled = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập email và mật khẩu.", "Lỗi");
                button1.Enabled = true;
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Mật khẩu và nhập lại mật khẩu không khớp.", "Lỗi");
                button1.Enabled = true;
                return;
            }

            string result = await FirebaseAuthService.Auth(email, password, isSignUp: true);

            if (result.StartsWith("Success|"))
            {
                // Expecting Success|idToken|localId
                var parts = result.Split('|');
                string idToken = parts.Length > 1 ? parts[1] : null;
                string localId = parts.Length > 2 ? parts[2] : null;

                if (!string.IsNullOrEmpty(idToken) && !string.IsNullOrEmpty(localId))
                {
                    // Save extra profile data to Realtime Database (if configured)
                    var saveResult = await FirebaseAuthService.SaveUserProfileAsync(idToken, localId, email, username, birthdate);
                    if (saveResult.StartsWith("Success"))
                    {
                        MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thông báo");
                        dangnhap frmDangNhap = new dangnhap();
                        frmDangNhap.Show();
                        this.Close();
                    }
                    else
                    {
                        // Profile save failed but account created. Inform user.
                        MessageBox.Show("Tạo tài khoản thành công nhưng lưu thông tin người dùng thất bại: " + saveResult, "Cảnh báo");
                        dangnhap frmDangNhap = new dangnhap();
                        frmDangNhap.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Đăng ký thành công nhưng thiếu dữ liệu phản hồi từ server.", "Thông báo");
                }
            }
            else
            {
                string errorMessage = result.Split('|')[1];
                MessageBox.Show("Đăng ký thất bại: " + errorMessage, "Lỗi");
            }

            button1.Enabled = true;
        }

        private void label4_Click(object sender, EventArgs e)
        {
            dangnhap form1 = new dangnhap();
            form1.Show();
            this.Hide();
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
                    new[] { "email", "profile" },
                    "user",
                    CancellationToken.None);

                string googleAccessToken = credential.Token.AccessToken;

                button2.Text = "Đang đăng nhập Firebase...";
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(AppConfig.FirebaseApiKey));

                var authLink = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Google, googleAccessToken);

                string firebaseToken = authLink.FirebaseToken;
                string userEmail = authLink.User.Email;
                string userName = authLink.User.DisplayName;

                MessageBox.Show($"Đăng nhập Google thành công!\nXin chào: {userName} ({userEmail})",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                MainForm mainForm = new MainForm();
                mainForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đăng nhập Google thất bại hoặc bị hủy: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button2.Enabled = true;
                button2.Text = "Đăng nhập với Google";
            }
        }

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
