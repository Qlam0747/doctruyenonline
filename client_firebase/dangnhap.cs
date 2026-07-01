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

        public dangnhap()
        {
            InitializeComponent();
            
            // Wire up events manually if needed or ensure they match designer
            btnSignIn.Click += btnSignIn_Click;
            btnGoogle.Click += btnGoogle_Click;
            btnFacebook.Click += btnFacebook_Click;
            btnShowPassword.Click += btnShowPassword_Click;
            lblSignUp.Click += lblSignUp_Click;
            lblForgetPassword.Click += lblForgetPassword_Click;
        }

        private void btnShowPassword_Click(object sender, EventArgs e)
        {
            _passwordVisible = !_passwordVisible;
            txtPassword.PasswordChar = _passwordVisible ? '\0' : '●';
            btnShowPassword.Text = _passwordVisible ? "🙈" : "👁";
        }

        private async void btnSignIn_Click(object sender, EventArgs e)
        {
            btnSignIn.Enabled = false;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            bool hasError = false;

            if (string.IsNullOrWhiteSpace(email))
            {
                labelEmailError.Text = "Vui lòng nhập email.";
                labelEmailError.Visible = true;
                hasError = true;
            }
            else { labelEmailError.Visible = false; }

            if (string.IsNullOrWhiteSpace(password))
            {
                labelPasswordError.Text = "Vui lòng nhập mật khẩu.";
                labelPasswordError.Visible = true;
                hasError = true;
            }
            else if (password.Length < 6)
            {
                labelPasswordError.Text = "Mật khẩu ít nhất 6 ký tự.";
                labelPasswordError.Visible = true;
                hasError = true;
            }
            else { labelPasswordError.Visible = false; }

            if (hasError)
            {
                btnSignIn.Enabled = true;
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
                string msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += "\nInner: " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        msg += "\nInnerInner: " + ex.InnerException.InnerException.Message;
                    }
                    if (ex.InnerException is System.Reflection.ReflectionTypeLoadException rtle)
                    {
                        msg += "\nTypeLoadErrors: ";
                        foreach (var le in rtle.LoaderExceptions)
                        {
                            if (le != null) msg += le.Message + "; ";
                        }
                    }
                }
                MessageBox.Show("Lỗi kết nối: " + msg, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSignIn.Enabled = true;
            }
        }

        private async void btnGoogle_Click(object sender, EventArgs e)
        {
            btnGoogle.Enabled = false;
            btnGoogle.Text = "Processing...";

            try
            {
                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = AppConfig.GoogleClientId,
                        ClientSecret = AppConfig.GoogleClientSecret
                    },
                    new[] { "openid", "email", "profile" },
                    Guid.NewGuid().ToString(), 
                    CancellationToken.None);

                string googleIdToken = credential.Token.IdToken;

                if (string.IsNullOrEmpty(googleIdToken))
                {
                    MessageBox.Show("Không lấy được mã định danh từ Google.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string result = await FirebaseAuthService.LoginWithGoogleAsync(googleIdToken);

                if (result.StartsWith("Success|"))
                {
                    var parts = result.Split('|');
                    AuthSession.FirebaseIdToken = parts[1];
                    AuthSession.FirebaseLocalId = parts[2];
                    string email = parts.Length > 3 ? parts[3] : "Chưa cung cấp email";
                    string displayName = parts.Length > 4 ? parts[4] : "Người dùng Google";
                    bool isNewUser = parts.Length > 5 && bool.Parse(parts[5]);

                    if (isNewUser)
                    {
                        await FirebaseAuthService.SaveUserProfileAsync(AuthSession.FirebaseIdToken, AuthSession.FirebaseLocalId, email, displayName, "2000-01-01");
                    }
                    
                    MainForm mainForm = new MainForm();
                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Lỗi: " + result.Split('|')[1], "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += "\nInner: " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        msg += "\nInnerInner: " + ex.InnerException.InnerException.Message;
                    }
                }
                MessageBox.Show("Lỗi: " + msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGoogle.Enabled = true;
                btnGoogle.Text = "Google";
            }
        }

        private async void btnFacebook_Click(object sender, EventArgs e)
        {
            btnFacebook.Enabled = false;
            try
            {
                string fbTokenResult = await FirebaseAuthService.GetFacebookAccessTokenAsync();
                if (!fbTokenResult.StartsWith("Success|")) return;

                string fbAccessToken = fbTokenResult.Split('|')[1];
                string loginResult = await FirebaseAuthService.LoginWithFacebookAsync(fbAccessToken);

                if (loginResult.StartsWith("Success|"))
                {
                    var parts = loginResult.Split('|');
                    AuthSession.FirebaseIdToken = parts[1];
                    AuthSession.FirebaseLocalId = parts[2];
                    string email = parts.Length > 3 ? parts[3] : "Chưa cung cấp email";
                    string displayName = parts.Length > 4 ? parts[4] : "Người dùng Facebook";
                    bool isNewUser = parts.Length > 5 && bool.Parse(parts[5]);

                    if (isNewUser)
                    {
                        await FirebaseAuthService.SaveUserProfileAsync(AuthSession.FirebaseIdToken, AuthSession.FirebaseLocalId, email, displayName, "2000-01-01");
                    }

                    MainForm mainForm = new MainForm();
                    mainForm.Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += "\nInner: " + ex.InnerException.Message;
                }
                MessageBox.Show("Lỗi: " + msg);
            }
            finally
            {
                btnFacebook.Enabled = true;
            }
        }

        private void lblSignUp_Click(object sender, EventArgs e)
        {
            dangky dk = new dangky();
            dk.Show();
            this.Hide();
        }

        private void lblForgetPassword_Click(object sender, EventArgs e)
        {
            Forget_verify forget = new Forget_verify();
            forget.Show();
            this.Hide();
        }
    }
}