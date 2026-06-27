using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class UC_Chat : UserControl
    {
        private UserModel selectedUser = null;
        private List<UserModel> allUsers = new List<UserModel>();
        private Timer pollTimer;
        private long lastMessageTimestamp = 0;

        public UC_Chat()
        {
            InitializeComponent();

            // Set up search textbox events
            txtSearchContacts.GotFocus += (s, e) => { if (txtSearchContacts.Text == "Tìm kiếm tin nhắn...") { txtSearchContacts.Text = ""; txtSearchContacts.ForeColor = Color.Black; } };
            txtSearchContacts.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearchContacts.Text)) { txtSearchContacts.Text = "Tìm kiếm tin nhắn..."; txtSearchContacts.ForeColor = Color.Gray; } };
            txtSearchContacts.TextChanged += txtSearchContacts_TextChanged;

            btnSend.Click += btnSend_Click;
            txtInput.KeyDown += txtInput_KeyDown;

            // Register Options dropdown click
            btnHeaderOptions.Click += btnHeaderOptions_Click;

            // Shift input bar to fit attachment and emoji buttons
            txtInput.Location = new Point(125, 12);
            txtInput.Width = 355;

            Button btnAttachFile = new Button
            {
                Text = "📁",
                Font = new Font("Segoe UI", 12F),
                Size = new Size(32, 36),
                Location = new Point(10, 12),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black
            };
            btnAttachFile.FlatAppearance.BorderSize = 0;
            btnAttachFile.Click += btnAttachFile_Click;

            Button btnAttachImage = new Button
            {
                Text = "📷",
                Font = new Font("Segoe UI", 12F),
                Size = new Size(32, 36),
                Location = new Point(48, 12),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black
            };
            btnAttachImage.FlatAppearance.BorderSize = 0;
            btnAttachImage.Click += btnAttachImage_Click;

            Button btnEmoji = new Button
            {
                Text = "😀",
                Font = new Font("Segoe UI", 12F),
                Size = new Size(32, 36),
                Location = new Point(86, 12),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black
            };
            btnEmoji.FlatAppearance.BorderSize = 0;
            btnEmoji.Click += btnEmoji_Click;

            panelInput.Controls.Add(btnAttachFile);
            panelInput.Controls.Add(btnAttachImage);
            panelInput.Controls.Add(btnEmoji);

            // Set up polling timer for new messages (every 3 seconds)
            pollTimer = new Timer();
            pollTimer.Interval = 3000;
            pollTimer.Tick += PollTimer_Tick;

            this.Load += UC_Chat_Load;
            
            // Set right panel initial state (disabled/invisible chat details if no partner selected)
            ShowChatArea(false);
        }

        private async void UC_Chat_Load(object sender, EventArgs e)
        {
            await LoadContactsAsync();
            pollTimer.Start();
        }

        private void ShowChatArea(bool show)
        {
            panelHeader.Visible = show;
            flpMessages.Visible = show;
            panelInput.Visible = show;
        }

        private async Task LoadContactsAsync()
        {
            flpContacts.Controls.Clear();
            this.Cursor = Cursors.WaitCursor;

            allUsers = await FirebaseDatabaseService.GetAllUsersAsync();

            this.Cursor = Cursors.Default;

            if (allUsers.Count == 0)
            {
                Label lblNoContacts = new Label
                {
                    Text = "Không có người dùng nào khác",
                    Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(10)
                };
                flpContacts.Controls.Add(lblNoContacts);
                return;
            }

            foreach (var user in allUsers)
            {
                // Create contact item card
                Panel card = new Panel
                {
                    Width = flpContacts.Width - 25,
                    Height = 70,
                    BackColor = Color.White,
                    Margin = new Padding(0, 5, 0, 5),
                    Cursor = Cursors.Hand,
                    Tag = user
                };

                // Rounded/border style
                card.Paint += (s, pe) =>
                {
                    using (Pen pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                    {
                        pe.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                    }
                };

                PictureBox pbAvatar = new PictureBox
                {
                    Location = new Point(10, 15),
                    Size = new Size(40, 40),
                    SizeMode = PictureBoxSizeMode.Zoom
                };
                DrawAvatar(pbAvatar, user.Username, user.Avatar);

                Label lblName = new Label
                {
                    Text = user.Username,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Location = new Point(60, 15),
                    Size = new Size(card.Width - 80, 20),
                    AutoEllipsis = true
                };

                Label lblLastMsg = new Label
                {
                    Text = "Nhấp để trò chuyện...",
                    Font = new Font("Segoe UI", 8.25F, FontStyle.Regular),
                    ForeColor = Color.Gray,
                    Location = new Point(60, 38),
                    Size = new Size(card.Width - 80, 18),
                    AutoEllipsis = true
                };

                // Add to card
                card.Controls.Add(pbAvatar);
                card.Controls.Add(lblName);
                card.Controls.Add(lblLastMsg);

                // Add click handlers for card and all its children
                Action clickAction = async () =>
                {
                    await SelectContact(user, card);
                };
                card.Click += (s, e) => clickAction();
                pbAvatar.Click += (s, e) => clickAction();
                lblName.Click += (s, e) => clickAction();
                lblLastMsg.Click += (s, e) => clickAction();

                flpContacts.Controls.Add(card);

                // Load last message asynchronously for each user
                LoadLastMessageForContact(user, lblLastMsg);
            }
        }

        private async void LoadLastMessageForContact(UserModel user, Label label)
        {
            var msg = await FirebaseDatabaseService.GetLastMessageAsync(user.LocalId);
            if (msg != null && this.IsHandleCreated)
            {
                this.BeginInvoke(new Action(() =>
                {
                    label.Text = msg.Text;
                }));
            }
        }

        public async void SelectUserById(string userId)
        {
            if (flpContacts.Controls.Count == 0 || allUsers.Count == 0)
            {
                await LoadContactsAsync();
            }

            foreach (Control ctrl in flpContacts.Controls)
            {
                if (ctrl is Panel card && card.Tag is UserModel user && user.LocalId == userId)
                {
                    await SelectContact(user, card);
                    return;
                }
            }
        }

        private async Task SelectContact(UserModel user, Panel card)
        {
            selectedUser = user;

            // Clear unread flag for this chat
            await FirebaseDatabaseService.SetChatUnreadAsync(user.LocalId, false);

            // Highlight chosen card
            foreach (Control ctrl in flpContacts.Controls)
            {
                if (ctrl is Panel p)
                {
                    p.BackColor = Color.White;
                }
            }
            card.BackColor = Color.FromArgb(235, 233, 255); // Pale violet matching selected

            // Determine role: anyone who uploaded a book is an Author, else Reader
            bool isAuthor = false;
            try
            {
                var books = await FirebaseDatabaseService.GetAllBooksAsync();
                isAuthor = books.Any(b => b.AuthorId == user.LocalId);
            }
            catch {}

            // Update header info
            lblHeaderName.Text = user.Username;
            lblHeaderRole.Text = isAuthor ? "Tác giả" : "Độc giả";
            DrawAvatar(pbHeaderAvatar, user.Username);

            ShowChatArea(true);
            lastMessageTimestamp = 0;
            flpMessages.Controls.Clear();

            await LoadMessagesAsync();
        }

        private async Task LoadMessagesAsync()
        {
            if (selectedUser == null) return;

            var messages = await FirebaseDatabaseService.GetMessagesAsync(selectedUser.LocalId);
            flpMessages.SuspendLayout();
            flpMessages.Controls.Clear();

            foreach (var msg in messages)
            {
                AddMessageBubble(msg);
                lastMessageTimestamp = Math.Max(lastMessageTimestamp, msg.Timestamp);
            }

            flpMessages.ResumeLayout(true);
            ScrollToBottom();
        }

        private void AddMessageBubble(MessageModel msg)
        {
            bool isMe = msg.SenderId == AuthSession.FirebaseLocalId;
            int maxBubbleWidth = (int)((flpMessages.Width - 50) * 0.7);

            // Message container panel
            Panel rowPanel = new Panel
            {
                Width = flpMessages.Width - 35,
                Margin = new Padding(0, 5, 0, 5),
                BackColor = Color.Transparent
            };

            // Timestamp Label
            Label lblTime = new Label
            {
                Text = FormatTime(msg.Timestamp),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = true
            };

            // Bubble Panel
            Panel bubble = new Panel
            {
                BackColor = isMe ? Color.FromArgb(108, 92, 231) : Color.FromArgb(230, 230, 230),
                Padding = new Padding(10, 8, 10, 8),
            };

            if (msg.FileType == "image" && !string.IsNullOrEmpty(msg.FileBase64))
            {
                PictureBox pbMsg = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Size = new Size(180, 120),
                    Cursor = Cursors.Hand,
                    Location = new Point(10, 8)
                };
                try
                {
                    byte[] bytes = Convert.FromBase64String(msg.FileBase64);
                    using (var ms = new MemoryStream(bytes))
                    {
                        pbMsg.Image = Image.FromStream(ms);
                        pbMsg.Image = (Image)pbMsg.Image.Clone();
                    }
                }
                catch
                {
                    pbMsg.Image = null;
                }
                pbMsg.Click += (s, e) =>
                {
                    // Show full-size image in a temporary popup Form
                    Form imgForm = new Form
                    {
                        Text = "Xem hình ảnh",
                        Size = new Size(600, 500),
                        StartPosition = FormStartPosition.CenterScreen
                    };
                    PictureBox pbFull = new PictureBox
                    {
                        Image = pbMsg.Image,
                        Dock = DockStyle.Fill,
                        SizeMode = PictureBoxSizeMode.Zoom
                    };
                    imgForm.Controls.Add(pbFull);
                    imgForm.ShowDialog();
                };
                bubble.Width = 200;
                bubble.Height = 136;
                bubble.Controls.Add(pbMsg);
            }
            else if (msg.FileType == "file" && !string.IsNullOrEmpty(msg.FileBase64))
            {
                Label lblFileIcon = new Label
                {
                    Text = "📎 Tệp tin:",
                    Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                    ForeColor = isMe ? Color.White : Color.Black,
                    Location = new Point(10, 8),
                    AutoSize = true
                };
                Label lblFileName = new Label
                {
                    Text = msg.FileName ?? "document.dat",
                    Font = new Font("Segoe UI", 9.75F, FontStyle.Underline),
                    ForeColor = isMe ? Color.White : Color.FromArgb(108, 92, 231),
                    Location = new Point(10, 26),
                    Size = new Size(180, 20),
                    AutoEllipsis = true,
                    Cursor = Cursors.Hand
                };
                lblFileName.Click += (s, e) =>
                {
                    // Save file dialog to download
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.FileName = msg.FileName ?? "document.dat";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                byte[] bytes = Convert.FromBase64String(msg.FileBase64);
                                File.WriteAllBytes(sfd.FileName, bytes);
                                MessageBox.Show("Đã tải tệp tin thành công!", "Tải xuống");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi tải tệp tin: " + ex.Message, "Lỗi");
                            }
                        }
                    }
                };
                bubble.Width = 200;
                bubble.Height = 55;
                bubble.Controls.Add(lblFileIcon);
                bubble.Controls.Add(lblFileName);
            }
            else
            {
                // Message Text
                Label lblText = new Label
                {
                    Text = msg.Text,
                    Font = new Font("Segoe UI", 9.75F, FontStyle.Regular),
                    ForeColor = isMe ? Color.White : Color.Black,
                    Location = new Point(10, 8),
                    MaximumSize = new Size(maxBubbleWidth, 0),
                    AutoSize = true
                };

                // Measure size to fit text perfectly
                Size bubbleSize;
                using (Graphics g = flpMessages.CreateGraphics())
                {
                    SizeF sf = g.MeasureString(msg.Text, lblText.Font, maxBubbleWidth);
                    bubbleSize = new Size((int)Math.Ceiling(sf.Width) + 20, (int)Math.Ceiling(sf.Height) + 16);
                }
                lblText.Size = bubbleSize;
                bubble.Size = bubbleSize;
                bubble.Controls.Add(lblText);
            }

            // Layout row Panel
            rowPanel.Controls.Add(lblTime);
            rowPanel.Controls.Add(bubble);

            // Positioning based on sender
            if (isMe)
            {
                lblTime.Location = new Point((rowPanel.Width - lblTime.Width) / 2, 0);
                bubble.Location = new Point(rowPanel.Width - bubble.Width - 10, 16);
            }
            else
            {
                lblTime.Location = new Point((rowPanel.Width - lblTime.Width) / 2, 0);
                bubble.Location = new Point(10, 16);
            }

            rowPanel.Height = bubble.Bottom + 5;
            flpMessages.Controls.Add(rowPanel);
        }

        private string FormatTime(long timestamp)
        {
            try
            {
                var dt = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
                return dt.ToString("h:mm, d/M/yyyy");
            }
            catch
            {
                return "";
            }
        }

        private void ScrollToBottom()
        {
            flpMessages.VerticalScroll.Value = flpMessages.VerticalScroll.Maximum;
            flpMessages.PerformLayout();
        }

        private void DrawAvatar(PictureBox pb, string name, string avatarBase64 = null)
        {
            pb.Paint -= pb_PaintAvatar;
            if (!string.IsNullOrEmpty(avatarBase64))
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(avatarBase64);
                    using (var ms = new MemoryStream(bytes))
                    {
                        pb.Image = Image.FromStream(ms);
                        pb.Image = (Image)pb.Image.Clone();
                    }
                    return;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error parsing user avatar: " + ex.Message);
                }
            }

            pb.Image = null;
            pb.Tag = name;
            pb.Paint += pb_PaintAvatar;
            pb.Invalidate();
        }

        private void pb_PaintAvatar(object sender, PaintEventArgs pe)
        {
            if (sender is PictureBox pb && pb.Tag != null)
            {
                string name = pb.Tag.ToString();
                pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                int hash = name.GetHashCode();
                Color bg = Color.FromArgb(100 + Math.Abs(hash % 100), 100 + Math.Abs((hash / 100) % 100), 220);
                using (Brush brush = new SolidBrush(bg))
                {
                    pe.Graphics.FillEllipse(brush, 0, 0, pb.Width - 1, pb.Height - 1);
                }

                string letter = string.IsNullOrEmpty(name) ? "?" : name.Substring(0, 1).ToUpper();
                using (Font f = new Font("Segoe UI", pb.Width > 40 ? 12 : 10, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    SizeF size = pe.Graphics.MeasureString(letter, f);
                    pe.Graphics.DrawString(letter, f, textBrush, (pb.Width - size.Width) / 2, (pb.Height - size.Height) / 2);
                }
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            await SendCurrentMessage();
        }

        private async void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true; // Prevent beep sound and newline
                await SendCurrentMessage();
            }
        }

        private async Task SendCurrentMessage()
        {
            if (selectedUser == null) return;
            string text = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            txtInput.Clear();
            txtInput.Focus();

            bool sent = await FirebaseDatabaseService.SendMessageAsync(selectedUser.LocalId, text);
            if (sent)
            {
                // Reload messages immediately
                await LoadMessagesAsync();
                
                // Update contact last message in list
                UpdateLastMessageInList(selectedUser.LocalId, text);
            }
        }

        private void UpdateLastMessageInList(string userId, string text)
        {
            foreach (Control ctrl in flpContacts.Controls)
            {
                if (ctrl is Panel card && card.Tag is UserModel user && user.LocalId == userId)
                {
                    foreach (Control child in card.Controls)
                    {
                        if (child is Label lbl && lbl.ForeColor == Color.Gray) // Last message label
                        {
                            lbl.Text = text;
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private async void PollTimer_Tick(object sender, EventArgs e)
        {
            if (selectedUser == null) return;

            var messages = await FirebaseDatabaseService.GetMessagesAsync(selectedUser.LocalId);
            if (messages.Count > 0)
            {
                long maxTs = 0;
                bool hasNew = false;

                foreach (var msg in messages)
                {
                    maxTs = Math.Max(maxTs, msg.Timestamp);
                    if (msg.Timestamp > lastMessageTimestamp)
                    {
                        AddMessageBubble(msg);
                        hasNew = true;
                    }
                }

                if (hasNew)
                {
                    lastMessageTimestamp = maxTs;
                    ScrollToBottom();
                    if (messages.Count > 0)
                    {
                        UpdateLastMessageInList(selectedUser.LocalId, messages[messages.Count - 1].Text);
                    }
                }
            }
        }

        private void txtSearchContacts_TextChanged(object sender, EventArgs e)
        {
            string query = txtSearchContacts.Text.Trim().ToLower();
            if (query == "tìm kiếm tin nhắn...") query = "";

            foreach (Control ctrl in flpContacts.Controls)
            {
                if (ctrl is Panel card && card.Tag is UserModel user)
                {
                    bool matches = user.Username.ToLower().Contains(query) || (user.Email != null && user.Email.ToLower().Contains(query));
                    card.Visible = matches;
                }
            }
        }

        private async void btnAttachFile_Click(object sender, EventArgs e)
        {
            if (selectedUser == null) return;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Document Files|*.pdf;*.docx;*.xlsx;*.txt;*.zip|All Files|*.*";
                ofd.Title = "Chọn file đính kèm";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        byte[] bytes = File.ReadAllBytes(ofd.FileName);
                        string base64 = Convert.ToBase64String(bytes);
                        string fileName = Path.GetFileName(ofd.FileName);

                        btnSend.Enabled = false;
                        this.Cursor = Cursors.WaitCursor;
                        bool sent = await FirebaseDatabaseService.SendMessageWithFileAsync(
                            selectedUser.LocalId,
                            $"Đã gửi file: {fileName}",
                            "file",
                            base64,
                            fileName
                        );
                        this.Cursor = Cursors.Default;
                        btnSend.Enabled = true;

                        if (sent)
                        {
                            await LoadMessagesAsync();
                            UpdateLastMessageInList(selectedUser.LocalId, $"📎 Tệp tin: {fileName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi đọc file đính kèm: " + ex.Message, "Lỗi");
                    }
                }
            }
        }

        private async void btnAttachImage_Click(object sender, EventArgs e)
        {
            if (selectedUser == null) return;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
                ofd.Title = "Chọn hình ảnh gửi";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        byte[] bytes = File.ReadAllBytes(ofd.FileName);
                        string base64 = Convert.ToBase64String(bytes);
                        string fileName = Path.GetFileName(ofd.FileName);

                        btnSend.Enabled = false;
                        this.Cursor = Cursors.WaitCursor;
                        bool sent = await FirebaseDatabaseService.SendMessageWithFileAsync(
                            selectedUser.LocalId,
                            "[Hình ảnh]",
                            "image",
                            base64,
                            fileName
                        );
                        this.Cursor = Cursors.Default;
                        btnSend.Enabled = true;

                        if (sent)
                        {
                            await LoadMessagesAsync();
                            UpdateLastMessageInList(selectedUser.LocalId, "📷 [Hình ảnh]");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi đọc file hình ảnh: " + ex.Message, "Lỗi");
                    }
                }
            }
        }

        private void btnEmoji_Click(object sender, EventArgs e)
        {
            ContextMenuStrip emojiMenu = new ContextMenuStrip();
            string[] emojis = { "😀", "😂", "🥰", "👍", "🔥", "🎉", "❤️", "😮", "😭", "🙏" };
            foreach (var emoji in emojis)
            {
                var item = emojiMenu.Items.Add(emoji);
                item.Click += (s, ev) =>
                {
                    txtInput.SelectedText = emoji;
                };
            }
            emojiMenu.Show(Cursor.Position);
        }

        private async void btnHeaderOptions_Click(object sender, EventArgs e)
        {
            if (selectedUser == null) return;

            ContextMenuStrip optionsMenu = new ContextMenuStrip();

            var blockItem = optionsMenu.Items.Add("🚫 Chặn người dùng");
            var viewFilesItem = optionsMenu.Items.Add("📂 Xem file/hình ảnh đã gửi");
            var deleteItem = optionsMenu.Items.Add("🗑️ Xóa hội thoại");

            blockItem.Click += async (s, ev) =>
            {
                var dr = MessageBox.Show($"Bạn có chắc chắn muốn chặn {selectedUser.Username}? Hai bên sẽ không thể nhắn tin cho nhau.", "Xác nhận chặn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    bool res = await FirebaseDatabaseService.BlockUserAsync(selectedUser.LocalId);
                    if (res)
                    {
                        MessageBox.Show("Đã chặn người dùng thành công.", "Thông báo");
                    }
                }
            };

            viewFilesItem.Click += async (s, ev) =>
            {
                var messages = await FirebaseDatabaseService.GetMessagesAsync(selectedUser.LocalId);
                var files = messages.Where(m => m.FileType == "image" || m.FileType == "file").ToList();
                if (files.Count == 0)
                {
                    MessageBox.Show("Không có tệp đính kèm nào được gửi trong cuộc trò chuyện này.", "Thông báo");
                    return;
                }

                using (Form listForm = new Form { Text = "Tệp đính kèm đã gửi", Size = new Size(400, 300), StartPosition = FormStartPosition.CenterParent })
                {
                    ListBox lb = new ListBox { Dock = DockStyle.Fill };
                    foreach (var f in files)
                    {
                        lb.Items.Add($"{FormatTime(f.Timestamp)} - {(f.FileType == "image" ? "📷" : "📎")} {f.FileName ?? "File đính kèm"}");
                    }
                    listForm.Controls.Add(lb);
                    listForm.ShowDialog();
                }
            };

            deleteItem.Click += async (s, ev) =>
            {
                var dr = MessageBox.Show("Bạn có chắc chắn muốn xóa toàn bộ tin nhắn trong hội thoại này? Thao tác không thể khôi phục.", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    bool res = await FirebaseDatabaseService.DeleteConversationAsync(selectedUser.LocalId);
                    if (res)
                    {
                        flpMessages.Controls.Clear();
                        MessageBox.Show("Đã xóa hội thoại thành công.", "Thông báo");
                    }
                }
            };

            optionsMenu.Show(Cursor.Position);
        }
    }
}
