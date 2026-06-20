using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class UC_BookDetail : UserControl
    {
        private BookModel currentBook;
        private List<ChapterModel> chaptersList = new List<ChapterModel>();
        private bool isBookmarked = false;

        public UC_BookDetail()
        {
            InitializeComponent();

            txtComment.GotFocus += (s, e) => { if (txtComment.Text == "Viết bình luận của bạn...") { txtComment.Text = ""; txtComment.ForeColor = Color.Black; } };
            txtComment.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtComment.Text)) { txtComment.Text = "Viết bình luận của bạn..."; txtComment.ForeColor = Color.Gray; } };

            btnRead.Click += btnRead_Click;
            btnBookmark.Click += btnBookmark_Click;
            btnRate.Click += btnRate_Click;
            btnChat.Click += btnChat_Click;
            btnPostComment.Click += btnPostComment_Click;
        }

        public async void SetBook(BookModel book)
        {
            currentBook = book;
            isBookmarked = await FirebaseDatabaseService.IsBookmarkedAsync(book.Id);
            if (isBookmarked)
            {
                btnBookmark.BackColor = Color.FromArgb(108, 92, 231);
                btnBookmark.ForeColor = Color.White;
            }
            else
            {
                btnBookmark.BackColor = Color.White;
                btnBookmark.ForeColor = Color.Black;
            }

            // Bind values
            lblTitle.Text = book.Title;
            lblDescription.Text = book.Description;
            lblAuthorName.Text = book.AuthorName ?? "Ẩn danh";
            lblViewsCount.Text = $"👁 {book.Views} lượt xem";
            lblLikesCount.Text = $"♡ {new Random().Next(10, 100)} lượt thích";
            lblRating.Text = $"⭐ {book.Rating:F1} ★★★★★";

            DrawAvatar(pbAuthor, book.AuthorName ?? "Ẩn danh");

            // Set cover image
            if (!string.IsNullOrEmpty(book.CoverBase64))
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(book.CoverBase64);
                    using (var ms = new MemoryStream(bytes))
                    {
                        pbCover.Image = Image.FromStream(ms);
                        pbCover.Image = (Image)pbCover.Image.Clone();
                    }
                }
                catch
                {
                    pbCover.Image = null;
                }
            }
            else
            {
                pbCover.Image = null;
            }

            // Asynchronously load chapters & comments
            await LoadChaptersAsync();
            await LoadCommentsAsync();
        }

        private async Task LoadChaptersAsync()
        {
            flpChapters.Controls.Clear();
            if (currentBook == null) return;

            chaptersList = await FirebaseDatabaseService.GetChaptersAsync(currentBook.Id);
            lblChaptersCount.Text = $"📖 {chaptersList.Count} chương";

            if (chaptersList.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "Chưa có chương nào",
                    ForeColor = Color.Gray,
                    Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                    AutoSize = true,
                    Margin = new Padding(10)
                };
                flpChapters.Controls.Add(lblEmpty);
                return;
            }

            foreach (var ch in chaptersList)
            {
                Panel row = new Panel
                {
                    Width = flpChapters.Width - 25,
                    Height = 45,
                    BackColor = Color.FromArgb(250, 250, 250),
                    Margin = new Padding(0, 3, 0, 3),
                    Cursor = Cursors.Hand
                };

                row.Paint += (s, pe) =>
                {
                    using (Pen pen = new Pen(Color.FromArgb(235, 235, 235), 1))
                    {
                        pe.Graphics.DrawRectangle(pen, 0, 0, row.Width - 1, row.Height - 1);
                    }
                };

                Label lblChName = new Label
                {
                    Text = $"Chương {ch.ChapterNumber}: {ch.Title}",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Location = new Point(15, 6),
                    Size = new Size(row.Width - 150, 18),
                    AutoEllipsis = true
                };

                Label lblDate = new Label
                {
                    Text = FormatTime(ch.CreatedAt),
                    Font = new Font("Segoe UI", 8F),
                    ForeColor = Color.Gray,
                    Location = new Point(15, 25),
                    Size = new Size(150, 15)
                };

                Label lblChViews = new Label
                {
                    Text = $"👁 {new Random().Next(10, currentBook.Views)}",
                    Font = new Font("Segoe UI", 8F),
                    ForeColor = Color.Gray,
                    Location = new Point(row.Width - 100, 15),
                    Size = new Size(80, 15),
                    TextAlign = ContentAlignment.TopRight
                };

                row.Controls.Add(lblChName);
                row.Controls.Add(lblDate);
                row.Controls.Add(lblChViews);

                // Click event
                Action click = () =>
                {
                    if (this.ParentForm is MainForm mf)
                    {
                        mf.ShowReadingScreen(currentBook, ch.ChapterNumber);
                    }
                };

                row.Click += (s, e) => click();
                lblChName.Click += (s, e) => click();
                lblDate.Click += (s, e) => click();
                lblChViews.Click += (s, e) => click();

                flpChapters.Controls.Add(row);
            }
        }

        private async Task LoadCommentsAsync()
        {
            flpComments.Controls.Clear();
            if (currentBook == null) return;

            var comments = await FirebaseDatabaseService.GetCommentsAsync(currentBook.Id);

            if (comments.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "Chưa có bình luận nào. Hãy trở thành người đầu tiên bình luận!",
                    ForeColor = Color.Gray,
                    Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                    AutoSize = true,
                    Margin = new Padding(10)
                };
                flpComments.Controls.Add(lblEmpty);
                return;
            }

            foreach (var c in comments)
            {
                Panel row = new Panel
                {
                    Width = flpComments.Width - 30,
                    BackColor = Color.Transparent,
                    Margin = new Padding(0, 5, 0, 5)
                };

                PictureBox pb = new PictureBox
                {
                    Size = new Size(32, 32),
                    Location = new Point(10, 5),
                    SizeMode = PictureBoxSizeMode.Zoom
                };
                DrawAvatar(pb, c.Username);

                Panel bubble = new Panel
                {
                    BackColor = Color.FromArgb(235, 236, 240),
                    Padding = new Padding(10, 8, 10, 8),
                    Location = new Point(50, 5),
                    Width = (int)((row.Width - 60) * 0.9),
                    AutoSize = true
                };

                Label lblUser = new Label
                {
                    Text = c.Username,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Location = new Point(10, 8),
                    AutoSize = true
                };

                Label lblText = new Label
                {
                    Text = c.Text,
                    Font = new Font("Segoe UI", 9F),
                    Location = new Point(10, 25),
                    MaximumSize = new Size(bubble.Width - 20, 0),
                    AutoSize = true
                };

                bubble.Controls.Add(lblUser);
                bubble.Controls.Add(lblText);

                Label lblLikes = new Label
                {
                    Text = $"♡ {c.Likes}   {FormatTime(c.Timestamp)}",
                    Font = new Font("Segoe UI", 7.5F, FontStyle.Regular),
                    ForeColor = Color.Gray,
                    AutoSize = true
                };

                row.Controls.Add(pb);
                row.Controls.Add(bubble);
                row.Controls.Add(lblLikes);

                // Compute row height & position of footer
                row.Height = bubble.Bottom + 20;
                lblLikes.Location = new Point(55, bubble.Bottom + 2);

                flpComments.Controls.Add(row);
            }
        }

        private async void btnPostComment_Click(object sender, EventArgs e)
        {
            if (currentBook == null) return;
            string text = txtComment.Text.Trim();
            if (string.IsNullOrEmpty(text) || text == "Viết bình luận của bạn...") return;

            txtComment.Text = "Viết bình luận của bạn...";
            txtComment.ForeColor = Color.Gray;

            btnPostComment.Enabled = false;
            bool res = await FirebaseDatabaseService.PostCommentAsync(currentBook.Id, text);
            btnPostComment.Enabled = true;

            if (res)
            {
                await LoadCommentsAsync();
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            if (chaptersList.Count > 0 && this.ParentForm is MainForm mf)
            {
                mf.ShowReadingScreen(currentBook, chaptersList[0].ChapterNumber);
            }
            else
            {
                MessageBox.Show("Truyện chưa có chương để đọc!", "Thông báo");
            }
        }

        private async void btnBookmark_Click(object sender, EventArgs e)
        {
            if (currentBook == null) return;
            isBookmarked = !isBookmarked;
            if (isBookmarked)
            {
                await FirebaseDatabaseService.AddToBookmarksAsync(currentBook.Id);
                await FirebaseDatabaseService.AddToFavoritesAsync(currentBook.Id);
                btnBookmark.BackColor = Color.FromArgb(108, 92, 231);
                btnBookmark.ForeColor = Color.White;
                MessageBox.Show("Đã thêm truyện vào danh sách yêu thích!", "Bookmark");
            }
            else
            {
                await FirebaseDatabaseService.RemoveFromBookmarksAsync(currentBook.Id);
                await FirebaseDatabaseService.RemoveFromFavoritesAsync(currentBook.Id);
                btnBookmark.BackColor = Color.White;
                btnBookmark.ForeColor = Color.Black;
                MessageBox.Show("Đã xóa khỏi danh sách yêu thích!", "Bookmark");
            }
        }

        private void btnRate_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Cảm ơn bạn đã đánh giá truyện 5 sao!", "Đánh giá");
        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            if (currentBook == null || string.IsNullOrEmpty(currentBook.AuthorId)) return;
            if (currentBook.AuthorId == AuthSession.FirebaseLocalId)
            {
                MessageBox.Show("Bạn không thể tự trò chuyện với chính mình!", "Thông báo");
                return;
            }

            if (this.ParentForm is MainForm mf)
            {
                mf.ShowChatWithUser(currentBook.AuthorId, currentBook.AuthorName);
            }
        }

        private void DrawAvatar(PictureBox pb, string name)
        {
            pb.Paint -= pb_PaintAvatar;
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
                using (Font f = new Font("Segoe UI", pb.Width > 40 ? 12 : 9, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    SizeF size = pe.Graphics.MeasureString(letter, f);
                    pe.Graphics.DrawString(letter, f, textBrush, (pb.Width - size.Width) / 2, (pb.Height - size.Height) / 2);
                }
            }
        }

        private string FormatTime(long timestamp)
        {
            try
            {
                var dt = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
                return dt.ToString("h:mm d/M/yyyy");
            }
            catch
            {
                return "";
            }
        }
    }
}
