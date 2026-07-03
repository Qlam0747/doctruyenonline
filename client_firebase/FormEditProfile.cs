using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_firebase
{
    public class FormEditProfile : Form
    {
        private Label lblTitle;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblBirthdate;
        private DateTimePicker dtpBirth;
        private Button btnSave;
        private Button btnCancel;
        private Panel panelHeader;

        public FormEditProfile()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 420);
            this.Text = "Chỉnh sửa thông tin";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular);

            
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(108, 92, 231) 
            };

            lblTitle = new Label
            {
                Text = "THÔNG TIN CÁ NHÂN",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 18),
                AutoSize = true
            };
            panelHeader.Controls.Add(lblTitle);

            
            lblEmail = new Label
            {
                Text = "Địa chỉ Email (Không thể thay đổi)",
                Location = new Point(20, 80),
                Size = new Size(360, 20),
                ForeColor = Color.FromArgb(120, 120, 120)
            };

            
            txtEmail = new TextBox
            {
                Location = new Point(20, 105),
                Size = new Size(345, 27),
                ReadOnly = true,
                BackColor = Color.FromArgb(230, 230, 230),
                BorderStyle = BorderStyle.FixedSingle
            };

            
            lblUsername = new Label
            {
                Text = "Tên hiển thị / Biệt danh",
                Location = new Point(20, 150),
                Size = new Size(360, 20),
                ForeColor = Color.FromArgb(45, 52, 54)
            };

            
            txtUsername = new TextBox
            {
                Location = new Point(20, 175),
                Size = new Size(345, 27),
                BorderStyle = BorderStyle.FixedSingle
            };

            
            lblBirthdate = new Label
            {
                Text = "Ngày sinh",
                Location = new Point(20, 220),
                Size = new Size(360, 20),
                ForeColor = Color.FromArgb(45, 52, 54)
            };

            
            dtpBirth = new DateTimePicker
            {
                Location = new Point(20, 245),
                Size = new Size(345, 27),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };

            
            btnSave = new Button
            {
                Text = "Lưu thay đổi",
                Location = new Point(60, 310),
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(108, 92, 231),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += btnSave_Click;

            
            btnCancel = new Button
            {
                Text = "Hủy bỏ",
                Location = new Point(210, 310),
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.FromArgb(45, 52, 54),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(panelHeader);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblBirthdate);
            this.Controls.Add(dtpBirth);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            this.Load += FormEditProfile_Load;
        }

        private async void FormEditProfile_Load(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            try
            {
                var profile = await FirebaseDatabaseService.GetCurrentUserProfileAsync();
                if (profile != null)
                {
                    txtEmail.Text = profile.Email ?? "";
                    txtUsername.Text = profile.Username ?? "";
                    
                    if (!string.IsNullOrEmpty(profile.Birthdate) && 
                        DateTime.TryParseExact(profile.Birthdate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime bdate))
                    {
                        dtpBirth.Value = bdate;
                    }
                    else if (!string.IsNullOrEmpty(profile.Birthdate) && DateTime.TryParse(profile.Birthdate, out DateTime bdate2))
                    {
                        dtpBirth.Value = bdate2;
                    }
                    btnSave.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Không thể tải thông tin cá nhân. Vui lòng kiểm tra kết nối mạng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thông tin: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Vui lòng nhập tên hiển thị.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSave.Enabled = false;
            string birthdateStr = dtpBirth.Value.ToString("yyyy-MM-dd");

            bool success = await FirebaseDatabaseService.UpdateUserProfileAsync(username, birthdateStr);
            if (success)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Không thể lưu thông tin. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = true;
            }
        }
    }
}
