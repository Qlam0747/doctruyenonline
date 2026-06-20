using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class UC_Notification : UserControl
    {
        private bool showingUnread = true;
        private List<NotificationModel> allNotifications = new List<NotificationModel>();

        public UC_Notification()
        {
            InitializeComponent();

            btnTabUnread.Click += (s, e) => SwitchTab(true);
            btnTabRead.Click += (s, e) => SwitchTab(false);
            btnMarkAll.Click += btnMarkAll_Click;

            this.Load += UC_Notification_Load;
        }

        private async void UC_Notification_Load(object sender, EventArgs e)
        {
            await LoadNotificationsAsync();
        }

        public async Task RefreshNotifications()
        {
            await LoadNotificationsAsync();
        }

        private void SwitchTab(bool unread)
        {
            showingUnread = unread;
            if (unread)
            {
                btnTabUnread.BackColor = Color.White;
                btnTabRead.BackColor = Color.Transparent;
            }
            else
            {
                btnTabUnread.BackColor = Color.Transparent;
                btnTabRead.BackColor = Color.White;
            }

            RenderNotifications();
        }

        private async Task LoadNotificationsAsync()
        {
            this.Cursor = Cursors.WaitCursor;
            allNotifications = await FirebaseDatabaseService.GetNotificationsAsync();
            this.Cursor = Cursors.Default;

            RenderNotifications();
        }

        private void RenderNotifications()
        {
            flpNotifications.Controls.Clear();

            var filtered = new List<NotificationModel>();
            foreach (var n in allNotifications)
            {
                if (showingUnread && !n.IsRead)
                    filtered.Add(n);
                else if (!showingUnread && n.IsRead)
                    filtered.Add(n);
            }

            if (filtered.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = showingUnread ? "Không có thông báo mới" : "Không có thông báo đã đọc",
                    Font = new Font("Segoe UI", 9.75F, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(15)
                };
                flpNotifications.Controls.Add(lblEmpty);
                return;
            }

            foreach (var n in filtered)
            {
                Panel item = new Panel
                {
                    Width = flpNotifications.Width - 25,
                    Height = 85,
                    BackColor = Color.White,
                    Margin = new Padding(0, 5, 0, 5)
                };

                item.Paint += (s, pe) =>
                {
                    using (Pen pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                    {
                        pe.Graphics.DrawRectangle(pen, 0, 0, item.Width - 1, item.Height - 1);
                    }
                };

                // Book Icon
                Label lblIcon = new Label
                {
                    Text = "📘",
                    Font = new Font("Segoe UI", 20F),
                    Location = new Point(15, 20),
                    Size = new Size(35, 45)
                };

                // Title
                Label lblTitle = new Label
                {
                    Text = n.Title,
                    Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                    Location = new Point(60, 15),
                    Size = new Size(item.Width - 130, 20),
                    AutoEllipsis = true
                };

                // Link / LinkLabel
                LinkLabel lnkChapter = new LinkLabel
                {
                    Text = n.ChapterName,
                    Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                    LinkColor = Color.FromArgb(108, 92, 231),
                    ActiveLinkColor = Color.FromArgb(80, 60, 200),
                    Location = new Point(60, 36),
                    Size = new Size(item.Width - 130, 20),
                    AutoEllipsis = true
                };

                // Time ago
                Label lblTime = new Label
                {
                    Text = n.TimeAgo,
                    Font = new Font("Segoe UI", 8F),
                    ForeColor = Color.Gray,
                    Location = new Point(60, 58),
                    Size = new Size(150, 15)
                };

                // Trash Delete Button
                Button btnDelete = new Button
                {
                    Text = "🗑",
                    Font = new Font("Segoe UI", 12F),
                    ForeColor = Color.FromArgb(108, 92, 231),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Size = new Size(35, 35),
                    Location = new Point(item.Width - 50, 25),
                    UseVisualStyleBackColor = true
                };
                btnDelete.FlatAppearance.BorderSize = 0;

                btnDelete.Click += async (s, e) =>
                {
                    btnDelete.Enabled = false;
                    bool deleted = await FirebaseDatabaseService.DeleteNotificationAsync(n.Id);
                    if (deleted)
                    {
                        await LoadNotificationsAsync();
                    }
                    else
                    {
                        btnDelete.Enabled = true;
                    }
                };

                lnkChapter.LinkClicked += async (s, e) =>
                {
                    // Handle jumping to book
                    if (n.BookId.Contains("dummy"))
                    {
                        // Mock alert for dummy items
                        MessageBox.Show($"Bạn đang xem thông báo về: {n.ChapterName}", "Màn hình đọc thử");
                    }
                    else
                    {
                        // Load book detail
                        this.Cursor = Cursors.WaitCursor;
                        var books = await FirebaseDatabaseService.GetAllBooksAsync();
                        BookModel targetBook = null;
                        foreach (var b in books)
                        {
                            if (b.Id == n.BookId)
                            {
                                targetBook = b;
                                break;
                            }
                        }
                        this.Cursor = Cursors.Default;

                        if (targetBook != null && this.ParentForm is MainForm mf)
                        {
                            // Mark single notification as read when clicking it
                            var update = new Dictionary<string, bool> { { "IsRead", true } };
                            await FirebaseDatabaseService.DeleteNotificationAsync(n.Id); // delete or update to read
                            // For simplicity, we can delete or update. Let's just navigate to book detail:
                            mf.ShowBookDetail(targetBook);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy truyện tương ứng với thông báo này.", "Lỗi");
                        }
                    }
                };

                item.Controls.Add(lblIcon);
                item.Controls.Add(lblTitle);
                item.Controls.Add(lnkChapter);
                item.Controls.Add(lblTime);
                item.Controls.Add(btnDelete);

                flpNotifications.Controls.Add(item);
            }
        }

        private async void btnMarkAll_Click(object sender, EventArgs e)
        {
            btnMarkAll.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            await FirebaseDatabaseService.MarkAllNotificationsAsReadAsync();

            this.Cursor = Cursors.Default;
            btnMarkAll.Enabled = true;

            await LoadNotificationsAsync();
        }
    }
}
