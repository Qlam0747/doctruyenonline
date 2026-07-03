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

            
            txtSearchContacts.GotFocus += (s, e) => { if (txtSearchContacts.Text == "Tìm kiếm tin nhắn...") { txtSearchContacts.Text = ""; txtSearchContacts.ForeColor = Color.Black; } };
            txtSearchContacts.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearchContacts.Text)) { txtSearchContacts.Text = "Tìm kiếm tin nhắn..."; txtSearchContacts.ForeColor = Color.Gray; } };
            txtSearchContacts.TextChanged += txtSearchContacts_TextChanged;

            btnSend.Click += btnSend_Click;
            txtInput.KeyDown += txtInput_KeyDown;

            
            btnHeaderOptions.Click += btnHeaderOptions_Click;

            
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

            
            pollTimer = new Timer();
            pollTimer.Interval = 3000;
            pollTimer.Tick += PollTimer_Tick;

            this.Load += UC_Chat_Load;
            
            
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

        private bool IsOnlyEmojis(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            
            
            
            
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"^[\s\p{So}\p{Cs}\p{Cf}]+$");
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            
            path.AddArc(arc, 180, 90);

            
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private Size MeasureTextSize(string text, Font font, int maxWidth)
        {
            using (Graphics g = flpMessages.CreateGraphics())
            {
                string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                int maxW = 0;
                int totalH = 0;

                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        totalH += (int)g.MeasureString(" ", font).Height;
                        continue;
                    }

                    SizeF sizeNoWrap = g.MeasureString(line, font);
                    if (sizeNoWrap.Width <= maxWidth)
                    {
                        maxW = Math.Max(maxW, (int)Math.Ceiling(sizeNoWrap.Width));
                        totalH += (int)Math.Ceiling(sizeNoWrap.Height);
                    }
                    else
                    {
                        SizeF sizeWrap = g.MeasureString(line, font, maxWidth);
                        maxW = Math.Max(maxW, (int)Math.Ceiling(sizeWrap.Width));
                        totalH += (int)Math.Ceiling(sizeWrap.Height);
                    }
                }

                
                return new Size(Math.Min(maxW, maxWidth) + 5, totalH + 4);
            }
        }

        private Panel CreateContactCard(UserModel user)
        {
            
            Panel card = new Panel
            {
                Width = flpContacts.Width - 25,
                Height = 70,
                BackColor = Color.White,
                Margin = new Padding(0, 5, 0, 5),
                Cursor = Cursors.Hand,
                Tag = user
            };

            
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

            
            card.Controls.Add(pbAvatar);
            card.Controls.Add(lblName);
            card.Controls.Add(lblLastMsg);

            
            Action clickAction = async () =>
            {
                await SelectContact(user, card);
            };
            card.Click += (s, e) => clickAction();
            pbAvatar.Click += (s, e) => clickAction();
            lblName.Click += (s, e) => clickAction();
            lblLastMsg.Click += (s, e) => clickAction();

            
            LoadLastMessageForContact(user, lblLastMsg);

            return card;
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

            
            var userLastMsgs = new Dictionary<string, MessageModel>();
            var tasks = allUsers.Select(async u =>
            {
                var lastMsg = await FirebaseDatabaseService.GetLastMessageAsync(u.LocalId);
                if (lastMsg != null)
                {
                    lock (userLastMsgs)
                    {
                        userLastMsgs[u.LocalId] = lastMsg;
                    }
                }
            });
            await Task.WhenAll(tasks);

            
            var activeUsers = allUsers.Where(u => userLastMsgs.ContainsKey(u.LocalId)).ToList();

            
            activeUsers = activeUsers.OrderByDescending(u => userLastMsgs[u.LocalId].Timestamp).ToList();

            if (activeUsers.Count == 0)
            {
                Label lblNoContacts = new Label
                {
                    Text = "Không có cuộc trò chuyện nào",
                    Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(10)
                };
                flpContacts.Controls.Add(lblNoContacts);
                return;
            }

            foreach (var user in activeUsers)
            {
                Panel card = CreateContactCard(user);
                flpContacts.Controls.Add(card);
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
            if (allUsers.Count == 0)
            {
                allUsers = await FirebaseDatabaseService.GetAllUsersAsync();
            }

            UserModel targetUser = allUsers.FirstOrDefault(u => u.LocalId == userId);
            if (targetUser == null)
            {
                return;
            }

            
            Panel existingCard = null;
            foreach (Control ctrl in flpContacts.Controls)
            {
                if (ctrl is Panel card && card.Tag is UserModel user && user.LocalId == userId)
                {
                    existingCard = card;
                    break;
                }
            }

            if (existingCard != null)
            {
                await SelectContact(targetUser, existingCard);
            }
            else
            {
                
                var labelsToRemove = flpContacts.Controls.OfType<Label>().ToList();
                foreach (var lbl in labelsToRemove)
                {
                    flpContacts.Controls.Remove(lbl);
                    lbl.Dispose();
                }

                
                Panel newCard = CreateContactCard(targetUser);
                flpContacts.Controls.Add(newCard);
                
                flpContacts.Controls.SetChildIndex(newCard, 0);
                await SelectContact(targetUser, newCard);
            }
        }

        private async Task SelectContact(UserModel user, Panel card)
        {
            
            if (selectedUser != null && selectedUser.LocalId != user.LocalId)
            {
                var oldMsgs = await FirebaseDatabaseService.GetMessagesAsync(selectedUser.LocalId);
                if (oldMsgs.Count == 0)
                {
                    
                    Panel oldCard = null;
                    foreach (Control ctrl in flpContacts.Controls)
                    {
                        if (ctrl is Panel p && p.Tag is UserModel u && u.LocalId == selectedUser.LocalId)
                        {
                            oldCard = p;
                            break;
                        }
                    }
                    if (oldCard != null)
                    {
                        flpContacts.Controls.Remove(oldCard);
                        oldCard.Dispose();
                    }
                }
            }

            selectedUser = user;

            
            await FirebaseDatabaseService.SetChatUnreadAsync(user.LocalId, false);

            
            foreach (Control ctrl in flpContacts.Controls)
            {
                if (ctrl is Panel p)
                {
                    p.BackColor = Color.White;
                }
            }
            card.BackColor = Color.FromArgb(235, 233, 255); 

            
            bool isAuthor = false;
            try
            {
                var books = await FirebaseDatabaseService.GetAllBooksAsync();
                isAuthor = books.Any(b => b.AuthorId == user.LocalId);
            }
            catch {}

            
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

            
            long prevTimestamp = 0;
            if (flpMessages.Controls.Count > 0)
            {
                Control lastCtrl = flpMessages.Controls[flpMessages.Controls.Count - 1];
                if (lastCtrl.Tag is long ts)
                {
                    prevTimestamp = ts;
                }
            }

            bool showTime = true;
            if (prevTimestamp > 0)
            {
                long diffMs = Math.Abs(msg.Timestamp - prevTimestamp);
                if (diffMs < 5 * 60 * 1000) 
                {
                    showTime = false;
                }
            }

            
            Panel rowPanel = new Panel
            {
                Width = flpMessages.Width - 35,
                Margin = new Padding(0, 5, 0, 5),
                BackColor = Color.Transparent,
                Tag = msg.Timestamp
            };

            
            Label lblTime = new Label
            {
                Text = FormatTime(msg.Timestamp),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = true
            };

            
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
                    Location = new Point(0, 0)
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
                bubble.BackColor = Color.FromArgb(250, 250, 250); 
                bubble.Padding = new Padding(0);
                bubble.Width = 180;
                bubble.Height = 120;
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
                bool isOnlyEmoji = !string.IsNullOrEmpty(msg.Text) && IsOnlyEmojis(msg.Text);
                if (isOnlyEmoji)
                {
                    
                    RichTextBox rtb = new RichTextBox
                    {
                        Text = msg.Text,
                        Font = new Font("Segoe UI Emoji", 26F),
                        ReadOnly = true,
                        BorderStyle = BorderStyle.None,
                        ScrollBars = RichTextBoxScrollBars.None,
                        BackColor = Color.FromArgb(250, 250, 250), 
                        Multiline = true,
                        WordWrap = true,
                        Location = new Point(0, 0)
                    };
                    rtb.Enter += (s, e) => { flpMessages.Focus(); };

                    
                    Size emojiSize;
                    using (Graphics g = flpMessages.CreateGraphics())
                    {
                        SizeF sf = g.MeasureString(msg.Text, rtb.Font, maxBubbleWidth);
                        emojiSize = new Size((int)Math.Ceiling(sf.Width) + 15, (int)Math.Ceiling(sf.Height) + 10);
                    }
                    rtb.Size = emojiSize;

                    bubble.BackColor = Color.FromArgb(250, 250, 250); 
                    bubble.Size = emojiSize;
                    bubble.Padding = new Padding(0);
                    bubble.Controls.Add(rtb);
                }
                else
                {
                    
                    RichTextBox rtb = new RichTextBox
                    {
                        Text = msg.Text,
                        Font = new Font("Segoe UI Emoji", 9.75F, FontStyle.Regular),
                        ReadOnly = true,
                        BorderStyle = BorderStyle.None,
                        ScrollBars = RichTextBoxScrollBars.None,
                        BackColor = isMe ? Color.FromArgb(108, 92, 231) : Color.FromArgb(230, 230, 230),
                        Multiline = true,
                        WordWrap = true,
                        Location = new Point(10, 8)
                    };
                    rtb.Enter += (s, e) => { flpMessages.Focus(); };
                    rtb.SelectAll();
                    rtb.SelectionColor = isMe ? Color.White : Color.Black;
                    rtb.DeselectAll();

                    Size textSize = MeasureTextSize(msg.Text, rtb.Font, maxBubbleWidth);
                    rtb.Size = new Size(textSize.Width + 10, textSize.Height + 5);
                    bubble.Size = new Size(rtb.Width + 20, rtb.Height + 16);
                    bubble.Controls.Add(rtb);
                }
            }

            
            if (bubble.BackColor != Color.FromArgb(250, 250, 250))
            {
                using (var path = GetRoundedRectanglePath(new Rectangle(0, 0, bubble.Width, bubble.Height), 12))
                {
                    bubble.Region = new Region(path);
                }
            }

            
            if (showTime)
            {
                rowPanel.Controls.Add(lblTime);
                rowPanel.Controls.Add(bubble);

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
            }
            else
            {
                rowPanel.Controls.Add(bubble);

                if (isMe)
                {
                    bubble.Location = new Point(rowPanel.Width - bubble.Width - 10, 0);
                }
                else
                {
                    bubble.Location = new Point(10, 0);
                }
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
                e.SuppressKeyPress = true; 
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
                
                await LoadMessagesAsync();
                
                
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
                        if (child is Label lbl && lbl.ForeColor == Color.Gray) 
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
            Button btnEmoji = sender as Button;
            if (btnEmoji == null) return;

            Form emojiPopup = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.Manual,
                Size = new Size(185, 185),
                BackColor = Color.White
            };

            emojiPopup.Paint += (s, pe) =>
            {
                using (Pen p = new Pen(Color.FromArgb(200, 200, 200), 1))
                {
                    pe.Graphics.DrawRectangle(p, 0, 0, emojiPopup.Width - 1, emojiPopup.Height - 1);
                }
            };

            emojiPopup.Deactivate += (s, ev) => emojiPopup.Close();

            TableLayoutPanel tlp = new TableLayoutPanel
            {
                ColumnCount = 5,
                RowCount = 5,
                Dock = DockStyle.Fill,
                Padding = new Padding(3),
                BackColor = Color.White
            };

            for (int i = 0; i < 5; i++)
            {
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
                tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            }

            string[] emojis = {
                "😀", "😂", "🥰", "👍", "🔥",
                "🎉", "❤️", "😮", "😭", "🙏",
                "🤔", "👏", "🥳", "✨", "😎",
                "💡", "⭐", "🚀", "👀", "💔",
                "💩", "🤩", "😜", "😡", "🤢"
            };

            foreach (var emoji in emojis)
            {
                Panel cell = new Panel
                {
                    Dock = DockStyle.Fill,
                    Cursor = Cursors.Hand,
                    BackColor = Color.White,
                    Margin = new Padding(1)
                };

                RichTextBox rtbEmoji = new RichTextBox
                {
                    Text = emoji,
                    Font = new Font("Segoe UI Emoji", 14F),
                    ReadOnly = true,
                    BorderStyle = BorderStyle.None,
                    ScrollBars = RichTextBoxScrollBars.None,
                    BackColor = Color.White,
                    Multiline = false,
                    Cursor = Cursors.Hand,
                    Dock = DockStyle.Fill
                };

                
                rtbEmoji.SelectAll();
                rtbEmoji.SelectionAlignment = HorizontalAlignment.Center;
                rtbEmoji.DeselectAll();
                rtbEmoji.Enter += (s, ev) => { emojiPopup.Focus(); };

                Action selectEmoji = async () =>
                {
                    emojiPopup.Close();
                    if (selectedUser == null) return;

                    bool sent = await FirebaseDatabaseService.SendMessageAsync(selectedUser.LocalId, emoji);
                    if (sent)
                    {
                        await LoadMessagesAsync();
                        UpdateLastMessageInList(selectedUser.LocalId, emoji);
                    }
                    txtInput.Focus();
                };

                cell.Click += (s, ev) => selectEmoji();
                rtbEmoji.Click += (s, ev) => selectEmoji();

                EventHandler hoverEnter = (s, ev) =>
                {
                    cell.BackColor = Color.FromArgb(240, 240, 240);
                    rtbEmoji.BackColor = Color.FromArgb(240, 240, 240);
                };
                EventHandler hoverLeave = (s, ev) =>
                {
                    cell.BackColor = Color.White;
                    rtbEmoji.BackColor = Color.White;
                };

                cell.MouseEnter += hoverEnter;
                cell.MouseLeave += hoverLeave;
                rtbEmoji.MouseEnter += hoverEnter;
                rtbEmoji.MouseLeave += hoverLeave;

                cell.Controls.Add(rtbEmoji);
                tlp.Controls.Add(cell);
            }

            emojiPopup.Controls.Add(tlp);

            
            Point pt = btnEmoji.PointToScreen(Point.Empty);
            emojiPopup.Location = new Point(pt.X - (emojiPopup.Width - btnEmoji.Width) / 2, pt.Y - emojiPopup.Height - 5);
            emojiPopup.Show();
            emojiPopup.Focus();
        }

        private async void btnHeaderOptions_Click(object sender, EventArgs e)
        {
            if (selectedUser == null) return;

            
            bool isBlocked = await FirebaseDatabaseService.IsUserBlockedAsync(selectedUser.LocalId);

            ContextMenuStrip optionsMenu = new ContextMenuStrip();

            var blockItem = optionsMenu.Items.Add(isBlocked ? "🔓 Gỡ chặn tin nhắn" : "🚫 Chặn người dùng");
            var viewFilesItem = optionsMenu.Items.Add("📂 Xem file/hình ảnh đã gửi");
            var deleteItem = optionsMenu.Items.Add("🗑️ Xóa hội thoại");

            blockItem.Click += async (s, ev) =>
            {
                if (isBlocked)
                {
                    var dr = MessageBox.Show($"Bạn có muốn gỡ chặn {selectedUser.Username}? Hai bên sẽ có thể nhắn tin lại cho nhau.", "Xác nhận gỡ chặn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        bool res = await FirebaseDatabaseService.UnblockUserAsync(selectedUser.LocalId);
                        if (res)
                        {
                            MessageBox.Show("Đã gỡ chặn người dùng thành công.", "Thông báo");
                        }
                    }
                }
                else
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
                }
            };

            viewFilesItem.Click += async (s, ev) =>
            {
                var messages = await FirebaseDatabaseService.GetMessagesAsync(selectedUser.LocalId);
                var attachments = messages.Where(m => !string.IsNullOrEmpty(m.FileBase64)).ToList();
                var imageFiles = attachments.Where(m => m.FileType == "image").ToList();
                var regularFiles = attachments.Where(m => m.FileType == "file").ToList();
                
                if (attachments.Count == 0)
                {
                    MessageBox.Show("Không có tệp đính kèm nào được gửi trong cuộc trò chuyện này.", "Thông báo");
                    return;
                }

                using (Form listForm = new Form 
                { 
                    Text = $"Tệp đính kèm với {selectedUser.Username}", 
                    Size = new Size(620, 500), 
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.White
                })
                {
                    TabControl tabControl = new TabControl { Dock = DockStyle.Fill, Padding = new Point(10, 5) };
                    TabPage tabImages = new TabPage { Text = "Hình ảnh 📷", BackColor = Color.White };
                    TabPage tabFiles = new TabPage { Text = "Tệp tin 📁", BackColor = Color.White };
                    
                    tabControl.TabPages.Add(tabImages);
                    tabControl.TabPages.Add(tabFiles);
                    
                    
                    FlowLayoutPanel flpImages = new FlowLayoutPanel 
                    { 
                        Dock = DockStyle.Fill, 
                        AutoScroll = true,
                        Padding = new Padding(10)
                    };
                    
                    if (imageFiles.Count == 0)
                    {
                        flpImages.Controls.Add(new Label 
                        { 
                            Text = "Không có hình ảnh nào.", 
                            Font = new Font("Segoe UI", 10F, FontStyle.Italic), 
                            ForeColor = Color.Gray,
                            AutoSize = true,
                            Margin = new Padding(10)
                        });
                    }
                    else
                    {
                        foreach (var imgMsg in imageFiles)
                        {
                            Panel imgCard = new Panel
                            {
                                Size = new Size(130, 160),
                                Margin = new Padding(10),
                                BorderStyle = BorderStyle.None
                            };
                            
                            imgCard.Paint += (senderPaint, pe) =>
                            {
                                using (Pen pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                                {
                                    pe.Graphics.DrawRectangle(pen, 0, 0, imgCard.Width - 1, imgCard.Height - 1);
                                }
                            };
                            
                            PictureBox pb = new PictureBox
                            {
                                Size = new Size(110, 110),
                                Location = new Point(10, 10),
                                SizeMode = PictureBoxSizeMode.Zoom,
                                Cursor = Cursors.Hand
                            };
                            
                            try
                            {
                                byte[] bytes = Convert.FromBase64String(imgMsg.FileBase64);
                                using (var ms = new MemoryStream(bytes))
                                {
                                    pb.Image = Image.FromStream(ms);
                                    pb.Image = (Image)pb.Image.Clone();
                                }
                            }
                            catch
                            {
                                pb.Image = null;
                            }
                            
                            Label lblImgTime = new Label
                            {
                                Text = FormatTime(imgMsg.Timestamp),
                                Font = new Font("Segoe UI", 7.5F, FontStyle.Regular),
                                ForeColor = Color.Gray,
                                Location = new Point(5, 125),
                                Size = new Size(120, 30),
                                TextAlign = ContentAlignment.TopCenter,
                                AutoEllipsis = true
                            };
                            
                            Action openImageAction = () =>
                            {
                                Form imgForm = new Form
                                {
                                    Text = "Xem hình ảnh",
                                    Size = new Size(600, 500),
                                    StartPosition = FormStartPosition.CenterScreen
                                };
                                PictureBox pbFull = new PictureBox
                                {
                                    Image = pb.Image,
                                    Dock = DockStyle.Fill,
                                    SizeMode = PictureBoxSizeMode.Zoom
                                };
                                imgForm.Controls.Add(pbFull);
                                imgForm.ShowDialog();
                            };
                            
                            pb.Click += (imgSender, imgEvent) => openImageAction();
                            
                            imgCard.Controls.Add(pb);
                            imgCard.Controls.Add(lblImgTime);
                            flpImages.Controls.Add(imgCard);
                        }
                    }
                    tabImages.Controls.Add(flpImages);
                    
                    
                    FlowLayoutPanel flpFiles = new FlowLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        AutoScroll = true,
                        FlowDirection = FlowDirection.TopDown,
                        WrapContents = false,
                        Padding = new Padding(10)
                    };
                    
                    if (regularFiles.Count == 0)
                    {
                        flpFiles.Controls.Add(new Label 
                        { 
                            Text = "Không có tệp tin nào.", 
                            Font = new Font("Segoe UI", 10F, FontStyle.Italic), 
                            ForeColor = Color.Gray,
                            AutoSize = true,
                            Margin = new Padding(10)
                        });
                    }
                    else
                    {
                        foreach (var fileMsg in regularFiles)
                        {
                            Panel fileRow = new Panel
                            {
                                Size = new Size(560, 50),
                                Margin = new Padding(0, 5, 0, 5),
                                BackColor = Color.FromArgb(248, 249, 250)
                            };
                            
                            fileRow.Paint += (senderPaint, pe) =>
                            {
                                using (Pen pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                                {
                                    pe.Graphics.DrawRectangle(pen, 0, 0, fileRow.Width - 1, fileRow.Height - 1);
                                }
                            };
                            
                            Label lblFileIcon = new Label
                            {
                                Text = "📎",
                                Font = new Font("Segoe UI", 12F),
                                Location = new Point(10, 15),
                                Size = new Size(25, 25),
                                AutoSize = true
                            };
                            
                            Label lblFileName = new Label
                            {
                                Text = fileMsg.FileName ?? "document.dat",
                                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                                Location = new Point(40, 15),
                                Size = new Size(240, 20),
                                AutoEllipsis = true,
                                Cursor = Cursors.Hand
                            };
                            
                            Label lblFileTime = new Label
                            {
                                Text = FormatTime(fileMsg.Timestamp),
                                Font = new Font("Segoe UI", 8F),
                                ForeColor = Color.Gray,
                                Location = new Point(290, 17),
                                Size = new Size(150, 20)
                            };
                            
                            Button btnDownload = new Button
                            {
                                Text = "Tải xuống",
                                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                                Size = new Size(85, 30),
                                Location = new Point(460, 10),
                                FlatStyle = FlatStyle.Flat,
                                BackColor = Color.FromArgb(108, 92, 231),
                                ForeColor = Color.White,
                                Cursor = Cursors.Hand
                            };
                            btnDownload.FlatAppearance.BorderSize = 0;
                            
                            Action downloadAction = () =>
                            {
                                using (SaveFileDialog sfd = new SaveFileDialog())
                                {
                                    sfd.FileName = fileMsg.FileName ?? "document.dat";
                                    if (sfd.ShowDialog() == DialogResult.OK)
                                    {
                                        try
                                        {
                                            byte[] bytes = Convert.FromBase64String(fileMsg.FileBase64);
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
                            
                            lblFileName.Click += (lblSender, lblEvent) => downloadAction();
                            btnDownload.Click += (btnSender, btnEvent) => downloadAction();
                            
                            fileRow.Controls.Add(lblFileIcon);
                            fileRow.Controls.Add(lblFileName);
                            fileRow.Controls.Add(lblFileTime);
                            fileRow.Controls.Add(btnDownload);
                            
                            flpFiles.Controls.Add(fileRow);
                        }
                    }
                    tabFiles.Controls.Add(flpFiles);
                    
                    listForm.Controls.Add(tabControl);
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
                        selectedUser = null;
                        ShowChatArea(false);
                        await LoadContactsAsync();
                    }
                }
            };

            optionsMenu.Show(Cursor.Position);
        }
    }
}
