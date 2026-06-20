using System;
using System.Collections.Generic;
using System.Drawing;
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
                DrawAvatar(pbAvatar, user.Username);

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

            // Highlight chosen card
            foreach (Control ctrl in flpContacts.Controls)
            {
                if (ctrl is Panel p)
                {
                    p.BackColor = Color.White;
                }
            }
            card.BackColor = Color.FromArgb(235, 233, 255); // Pale violet matching selected

            // Update header info
            lblHeaderName.Text = user.Username;
            lblHeaderRole.Text = "Độc giả";
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

            // Message Text
            Label lblText = new Label
            {
                Text = msg.Text,
                Font = new Font("Segoe UI", 9.75F, FontStyle.Regular),
                ForeColor = isMe ? Color.White : Color.Black,
                AutoSize = true,
                MaximumSize = new Size(maxBubbleWidth, 0)
            };

            // Bubble Panel
            Panel bubble = new Panel
            {
                BackColor = isMe ? Color.FromArgb(108, 92, 231) : Color.FromArgb(230, 230, 230),
                Padding = new Padding(10, 8, 10, 8),
                AutoSize = true
            };
            bubble.Controls.Add(lblText);
            lblText.Location = new Point(10, 8);

            // Layout row Panel
            rowPanel.Controls.Add(lblTime);
            rowPanel.Controls.Add(bubble);

            // Positioning based on sender
            if (isMe)
            {
                // Align right
                lblTime.Location = new Point((rowPanel.Width - lblTime.Width) / 2, 0);
                bubble.Location = new Point(rowPanel.Width - bubble.Width - 10, 16);
            }
            else
            {
                // Align left
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

        private void DrawAvatar(PictureBox pb, string name)
        {
            pb.Paint -= pb_PaintAvatar; // Remove existing to prevent multiple
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
    }
}
