using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace client_firebase
{
    public partial class Forget_verify : Form
    {
        public Forget_verify()
        {
            InitializeComponent();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string email = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ email của bạn!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Định dạng email không hợp lệ. Vui lòng kiểm tra lại (ví dụ: name@gmail.com)!",
                                "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            button1.Enabled = false;
            button1.Text = "Đang gửi...";

            try
            {
                string result = await FirebaseAuthService.ResetPassword(email);

                if (result == "Success")
                {
                    MessageBox.Show("Một email khôi phục mật khẩu đã được gửi đến hộp thư của bạn. Vui lòng kiểm tra (cả trong thư mục Spam).",
                                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    dangnhap formDangNhap = new dangnhap();
                    formDangNhap.Show();
                    this.Close();
                }
                else
                {
                    string errorMessage = result.Split('|')[1];
                    string msgVN = "Đã có lỗi xảy ra.";

                    if (errorMessage == "EMAIL_NOT_FOUND")
                        msgVN = "Email này chưa được đăng ký trong hệ thống.";

                    MessageBox.Show(msgVN, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button1.Enabled = true;
                button1.Text = "Gửi link khôi phục";
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            dangnhap dangnhap = new dangnhap();
            dangnhap.Show();
            this.Close();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            dangky dangky = new dangky();
            dangky.Show();
            this.Close();
        }
    }
}
