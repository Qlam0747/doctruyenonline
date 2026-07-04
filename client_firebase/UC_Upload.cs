using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class UC_Upload : UserControl
    {
        private string coverBase64 = "";
        private List<string> selectedGenres = new List<string>();

        public UC_Upload()
        {
            InitializeComponent();
            
            // Register events
            pbCover.Click += pbCover_Click;
            pbCover.Paint += pbCover_Paint;
            btnNext.Click += btnNext_Click;
            btnBack.Click += btnBack_Click;
            btnSubmit.Click += btnSubmit_Click;



            // Adjust txtDescription and flpGenres height to fit more genres
            txtDescription.Height = 160; // Reduce height from 225 to 160
            lblGenre.Location = new Point(10, 295); // Move up
            flpGenres.Location = new Point(10, 315);
            flpGenres.Height = 110;
            flpGenres.AutoScroll = true;

            // Set txtChapterContent MaxLength to 500,000 characters
            txtChapterContent.MaxLength = 500000;

            // Create btnImportFile dynamically in Step 2
            Button btnImportFile = new Button
            {
                Text = "📁 Nhập từ file .txt",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Size = new Size(130, 25),
                Location = new Point(590, 155), // Right aligned, above txtChapterContent
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.FromArgb(230, 230, 230),
                ForeColor = Color.Black
            };
            btnImportFile.Click += (s, e) =>
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Text Files|*.txt|All Files|*.*";
                    ofd.Title = "Chọn file văn bản nội dung chương";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            txtChapterContent.Text = File.ReadAllText(ofd.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi đọc file: " + ex.Message, "Lỗi");
                        }
                    }
                }
            };
            panelStep2.Controls.Add(btnImportFile);

            // Load genres
            InitializeGenreButtons();
        }

        private void InitializeGenreButtons()
        {
            string[] genres = { "Phiêu lưu", "Lãng mạn", "Viễn tưởng", "Kinh dị", "Hài hước", "Hành động", "Dã sử", "Trinh thám", "Cổ đại", "Huyền huyễn", "Đô thị", "Học đường", "Võ hiệp", "Khoa học" };
            flpGenres.Controls.Clear();
            foreach (var genre in genres)
            {
                Button btn = new Button();
                btn.Text = genre;
                btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btn.Size = new Size(110, 32);
                btn.Margin = new Padding(5);
                btn.Cursor = Cursors.Hand;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Color.Black;
                btn.BackColor = Color.White;
                btn.ForeColor = Color.Black;

                btn.Click += (s, ev) =>
                {
                    if (selectedGenres.Contains(genre))
                    {
                        selectedGenres.Remove(genre);
                        btn.BackColor = Color.White;
                        btn.ForeColor = Color.Black;
                        btn.FlatAppearance.BorderSize = 1;
                        btn.FlatAppearance.BorderColor = Color.Black;
                    }
                    else
                    {
                        selectedGenres.Add(genre);
                        btn.BackColor = Color.FromArgb(108, 92, 231);
                        btn.ForeColor = Color.White;
                        btn.FlatAppearance.BorderSize = 0;
                    }
                };

                flpGenres.Controls.Add(btn);
            }
        }

        private void pbCover_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
                ofd.Title = "Chọn ảnh bìa truyện";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        byte[] imageBytes = File.ReadAllBytes(ofd.FileName);
                        coverBase64 = Convert.ToBase64String(imageBytes);
                        
                        using (var ms = new MemoryStream(imageBytes))
                        {
                            pbCover.Image = Image.FromStream(ms);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi load ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void pbCover_Paint(object sender, PaintEventArgs e)
        {
            if (pbCover.Image == null)
            {
                // Draw dotted border
                using (Pen pen = new Pen(Color.Gray, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    e.Graphics.DrawRectangle(pen, 1, 1, pbCover.Width - 3, pbCover.Height - 3);
                }

                // Draw icon & text
                using (Font font = new Font("Segoe UI", 9F, FontStyle.Regular))
                using (Brush brush = new SolidBrush(Color.Gray))
                {
                    string icon = "📷";
                    using (Font iconFont = new Font("Segoe UI", 28F))
                    {
                        SizeF iconSize = e.Graphics.MeasureString(icon, iconFont);
                        e.Graphics.DrawString(icon, iconFont, brush, (pbCover.Width - iconSize.Width) / 2, pbCover.Height / 2 - 40);
                    }

                    string text = "Nhấp để thêm ảnh bìa";
                    SizeF textSize = e.Graphics.MeasureString(text, font);
                    e.Graphics.DrawString(text, font, brush, (pbCover.Width - textSize.Width) / 2, pbCover.Height / 2 + 10);
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // Validate Step 1
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tên truyện.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Vui lòng nhập mô tả truyện.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescription.Focus();
                return;
            }

            if (selectedGenres.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một thể loại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(coverBase64))
            {
                MessageBox.Show("Vui lòng tải lên ảnh bìa truyện.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Switch to Step 2
            panelStep1.Visible = false;
            panelStep2.Visible = true;
            btnBack.Visible = true;
            btnNext.Visible = false;
            btnSubmit.Visible = true;

            // Update indicators
            lblStep2Circle.BackColor = Color.FromArgb(108, 92, 231);
            lblStep2Text.ForeColor = Color.FromArgb(108, 92, 231);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            // Switch to Step 1
            panelStep2.Visible = false;
            panelStep1.Visible = true;

            btnBack.Visible = false;
            btnNext.Visible = true;
            btnSubmit.Visible = false;

            // Reset indicators
            lblStep2Circle.BackColor = Color.FromArgb(200, 200, 200);
            lblStep2Text.ForeColor = Color.Gray;
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            // Validate Step 2
            if (string.IsNullOrWhiteSpace(txtChapterNum.Text))
            {
                MessageBox.Show("Vui lòng nhập số chương.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChapterNum.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtChapterTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tiêu đề chương.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChapterTitle.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtChapterContent.Text))
            {
                MessageBox.Show("Vui lòng nhập nội dung chương.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChapterContent.Focus();
                return;
            }

            btnSubmit.Enabled = false;
            btnBack.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            string title = txtTitle.Text.Trim();
            string desc = txtDescription.Text.Trim();
            string chapNum = txtChapterNum.Text.Trim();
            string chapTitle = txtChapterTitle.Text.Trim();
            string chapContent = txtChapterContent.Text.Trim();

            string status = "Đang tiến hành";

            string res = await FirebaseDatabaseService.UploadBookAsync(title, desc, coverBase64, selectedGenres, chapNum, chapTitle, chapContent, status);

            this.Cursor = Cursors.Default;
            btnSubmit.Enabled = true;
            btnBack.Enabled = true;

            if (res.StartsWith("Success|"))
            {
                MessageBox.Show("Đăng truyện thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                
                // Return to home page
                if (this.ParentForm is MainForm mf)
                {
                    mf.GoToHome();
                }
            }
            else
            {
                string errMsg = res.Split('|')[1];
                MessageBox.Show("Đăng truyện thất bại: " + errMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtTitle.Clear();
            txtDescription.Clear();
            txtChapterNum.Text = "1";
            txtChapterTitle.Clear();
            txtChapterContent.Clear();
            pbCover.Image = null;
            coverBase64 = "";
            selectedGenres.Clear();
            InitializeGenreButtons();

            // Reset navigation panel
            panelStep2.Visible = false;
            panelStep1.Visible = true;
            btnBack.Visible = false;
            btnNext.Visible = true;
            btnSubmit.Visible = false;

            lblStep2Circle.BackColor = Color.FromArgb(200, 200, 200);
            lblStep2Text.ForeColor = Color.Gray;
        }
    }
}
