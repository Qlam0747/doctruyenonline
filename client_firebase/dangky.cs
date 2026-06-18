using Firebase.Auth;
using Google.Apis.Auth.OAuth2;
using System;
using System.Drawing;
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
            
            btnSignUp.Click += btnSignUp_Click;
            lblSignIn.Click += lblSignIn_Click;
        }

        private async void btnSignUp_Click(object sender, EventArgs e)
        {
            btnSignUp.Enabled = false;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string confirm = txtConfirmPassword.Text;
            string username = txtFullName.Text.Trim();
            string birthdate = dtpBirth.Value.ToString("yyyy-MM-dd");

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Vui lòng nhập tên người dùng.", "Lỗi");
                btnSignUp.Enabled = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập email và mật khẩu.", "Lỗi");
                btnSignUp.Enabled = true;
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Mật khẩu không khớp.", "Lỗi");
                btnSignUp.Enabled = true;
                return;
            }

            try
            {
                string result = await FirebaseAuthService.Auth(email, password, isSignUp: true);

                if (result.StartsWith("Success|"))
                {
                    var parts = result.Split('|');
                    string idToken = parts[1];
                    string localId = parts[2];

                    await FirebaseAuthService.SaveUserProfileAsync(idToken, localId, email, username, birthdate);
                    
                    MessageBox.Show("Đăng ký thành công!", "Thông báo");
                    lblSignIn_Click(null, null);
                }
                else
                {
                    MessageBox.Show("Lỗi: " + result.Split('|')[1]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                btnSignUp.Enabled = true;
            }
        }

        private void lblSignIn_Click(object sender, EventArgs e)
        {
            dangnhap frm = new dangnhap();
            frm.Show();
            this.Close();
        }
    }
}
