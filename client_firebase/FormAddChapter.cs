using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_firebase
{
    public class FormAddChapter : Form
    {
        private string _bookId;
        private string _bookTitle;
        private string _bookDesc;
        private string _currentStatus;
        
        private Panel panelHeader;
        private Label lblTitle;
        private Label lblChapterNum;
        private TextBox txtChapterNum;
        private Label lblChapterTitle;
        private TextBox txtChapterTitle;
        private Label lblChapterContent;
        private TextBox txtChapterContent;
        private Label lblStatus;
        private ComboBox cbStatus;
        
        private Button btnAIAssist;
        private Button btnSubmit;
        private Button btnCancel;
        private ContextMenuStrip aiMenu;

        public FormAddChapter(string bookId, string bookTitle, string bookDesc, int nextChapterNum, string currentStatus = "Đang tiến hành")
        {
            _bookId = bookId;
            _bookTitle = bookTitle;
            _bookDesc = bookDesc;
            _currentStatus = currentStatus;
            
            InitializeComponent(nextChapterNum);
            InitializeContextMenu();
        }

        private void InitializeComponent(int nextChapterNum)
        {
            this.Size = new Size(650, 600);
            this.Text = "Thêm chương mới - " + _bookTitle;
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
                Text = "ĐĂNG CHƯƠNG MỚI",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 18),
                AutoSize = true
            };
            panelHeader.Controls.Add(lblTitle);

            
            lblChapterNum = new Label
            {
                Text = "Số chương",
                Location = new Point(20, 75),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 52, 54)
            };

            
            txtChapterNum = new TextBox
            {
                Location = new Point(20, 100),
                Size = new Size(100, 27),
                Text = nextChapterNum.ToString(),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center
            };

            
            lblChapterTitle = new Label
            {
                Text = "Tiêu đề chương",
                Location = new Point(140, 75),
                Size = new Size(330, 20),
                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 52, 54)
            };

            
            txtChapterTitle = new TextBox
            {
                Location = new Point(140, 100),
                Size = new Size(330, 27),
                BorderStyle = BorderStyle.FixedSingle
            };

            
            lblStatus = new Label
            {
                Text = "Trạng thái truyện",
                Location = new Point(485, 75),
                Size = new Size(130, 20),
                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 52, 54)
            };

            
            cbStatus = new ComboBox
            {
                Location = new Point(485, 100),
                Size = new Size(130, 27),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.75F, FontStyle.Regular)
            };
            cbStatus.Items.AddRange(new object[] { "Đang tiến hành", "Đã hoàn thành" });
            cbStatus.SelectedIndex = _currentStatus == "Đã hoàn thành" ? 1 : 0;

            
            lblChapterContent = new Label
            {
                Text = "Nội dung chương truyện",
                Location = new Point(20, 145),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 52, 54)
            };

            
            btnAIAssist = new Button
            {
                Text = "🤖 Trợ lý viết AI",
                Location = new Point(480, 140),
                Size = new Size(135, 30),
                BackColor = Color.FromArgb(162, 155, 254), 
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnAIAssist.FlatAppearance.BorderSize = 0;
            btnAIAssist.Click += btnAIAssist_Click;

            
            txtChapterContent = new TextBox
            {
                Location = new Point(20, 175),
                Size = new Size(595, 310),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                MaxLength = 500000 
            };

            
            Button btnImportFile = new Button
            {
                Text = "📁 Nhập từ file .txt",
                Location = new Point(335, 140),
                Size = new Size(135, 30),
                BackColor = Color.FromArgb(230, 230, 230),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
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
                            txtChapterContent.Text = System.IO.File.ReadAllText(ofd.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi đọc file: " + ex.Message, "Lỗi");
                        }
                    }
                }
            };

            
            btnSubmit = new Button
            {
                Text = "Đăng chương mới",
                Location = new Point(160, 505),
                Size = new Size(150, 38),
                BackColor = Color.FromArgb(46, 204, 113), 
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold)
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Click += btnSubmit_Click;

            
            btnCancel = new Button
            {
                Text = "Hủy bỏ",
                Location = new Point(340, 505),
                Size = new Size(150, 38),
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.FromArgb(45, 52, 54),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            
            this.Controls.Add(panelHeader);
            this.Controls.Add(lblChapterNum);
            this.Controls.Add(txtChapterNum);
            this.Controls.Add(lblChapterTitle);
            this.Controls.Add(txtChapterTitle);
            this.Controls.Add(lblStatus);
            this.Controls.Add(cbStatus);
            this.Controls.Add(lblChapterContent);
            this.Controls.Add(btnAIAssist);
            this.Controls.Add(btnImportFile);
            this.Controls.Add(txtChapterContent);
            this.Controls.Add(btnSubmit);
            this.Controls.Add(btnCancel);
        }

        private void InitializeContextMenu()
        {
            aiMenu = new ContextMenuStrip();
            
            var itemContinue = new ToolStripMenuItem("✍️ Viết tiếp nội dung chương");
            itemContinue.Click += async (s, e) => await CallAIWriteAsync("continue");
            
            var itemPolish = new ToolStripMenuItem("✨ Trau chuốt & làm văn hay hơn");
            itemPolish.Click += async (s, e) => await CallAIWriteAsync("polish");
            
            var itemIdeas = new ToolStripMenuItem("💡 Gợi ý 3 hướng đi cốt truyện cho chương mới");
            itemIdeas.Click += async (s, e) => await CallAIWriteAsync("ideas");

            aiMenu.Items.Add(itemContinue);
            aiMenu.Items.Add(itemPolish);
            aiMenu.Items.Add(new ToolStripSeparator());
            aiMenu.Items.Add(itemIdeas);
        }

        private void btnAIAssist_Click(object sender, EventArgs e)
        {
            aiMenu.Show(btnAIAssist, new Point(0, btnAIAssist.Height));
        }

        private async Task CallAIWriteAsync(string actionType)
        {
            if (actionType != "ideas" && string.IsNullOrWhiteSpace(txtChapterContent.Text) && actionType == "polish")
            {
                MessageBox.Show("Vui lòng nhập một đoạn văn bản vào ô nội dung để AI có dữ liệu chỉnh sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            this.Cursor = Cursors.WaitCursor;
            btnAIAssist.Enabled = false;
            btnAIAssist.Text = "🤖 AI đang nghĩ...";

            string contextText = txtChapterContent.Text.Trim();
            
            
            if (actionType == "ideas" && string.IsNullOrEmpty(contextText))
            {
                contextText = "Không có hướng dẫn thêm";
            }

            string aiResult = await GeminiService.AssistWritingAsync(_bookTitle, _bookDesc, contextText, actionType);

            this.Cursor = Cursors.Default;
            btnAIAssist.Enabled = true;
            btnAIAssist.Text = "🤖 Trợ lý viết AI";

            if (string.IsNullOrEmpty(aiResult) || aiResult.StartsWith("Lỗi"))
            {
                MessageBox.Show(aiResult ?? "Không thể kết nối với AI.", "Lỗi AI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (actionType == "continue")
            {
                var dialog = MessageBox.Show("AI đã viết tiếp một phân đoạn. Bạn có muốn THÊM nó vào cuối chương không?\n\n--- Ý kiến của AI ---\n" + 
                                             (aiResult.Length > 200 ? aiResult.Substring(0, 200) + "..." : aiResult), 
                                             "Xác nhận thêm văn bản", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    if (txtChapterContent.Text.Length > 0)
                        txtChapterContent.AppendText("\r\n\r\n");
                    txtChapterContent.AppendText(aiResult);
                }
            }
            else if (actionType == "polish")
            {
                var dialog = MessageBox.Show("AI đã viết lại phân đoạn này mượt mà hơn. Bạn có muốn THAY THẾ nội dung hiện tại bằng văn bản mới không?\n\n--- Văn bản AI đề nghị ---\n" + 
                                             (aiResult.Length > 200 ? aiResult.Substring(0, 200) + "..." : aiResult), 
                                             "Xác nhận thay thế văn bản", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    txtChapterContent.Text = aiResult;
                }
            }
            else if (actionType == "ideas")
            {
                MessageBox.Show("Dưới đây là 3 hướng phát triển chương mới gợi ý bởi AI:\n\n" + aiResult, "Gợi ý cốt truyện của AI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            string chapNum = txtChapterNum.Text.Trim();
            string title = txtChapterTitle.Text.Trim();
            string content = txtChapterContent.Text.Trim();

            if (string.IsNullOrEmpty(chapNum))
            {
                MessageBox.Show("Vui lòng nhập số chương.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChapterNum.Focus();
                return;
            }

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Vui lòng nhập tiêu đề chương.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChapterTitle.Focus();
                return;
            }

            if (string.IsNullOrEmpty(content))
            {
                MessageBox.Show("Vui lòng nhập nội dung chương.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChapterContent.Focus();
                return;
            }

            btnSubmit.Enabled = false;
            btnCancel.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            string selectedStatus = cbStatus.SelectedItem.ToString();
            string res = await FirebaseDatabaseService.UploadChapterAsync(_bookId, chapNum, title, content, selectedStatus);

            this.Cursor = Cursors.Default;
            btnSubmit.Enabled = true;
            btnCancel.Enabled = true;

            if (res == "Success")
            {
                MessageBox.Show("Đăng chương mới thành công và gửi thông báo tới các độc giả quan tâm!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                string errMsg = res.Split('|')[1];
                MessageBox.Show("Đăng chương thất bại: " + errMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
