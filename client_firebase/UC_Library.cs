using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class UC_Library : UserControl
    {
        private int activeTab = 0; // 0 = Bookmark, 1 = History, 2 = Favorites
        private List<BookModel> allBooks = new List<BookModel>();

        public UC_Library()
        {
            InitializeComponent();

            btnTabBookmark.Click += (s, e) => SwitchTab(0);
            btnTabHistory.Click += (s, e) => SwitchTab(1);
            btnTabFavorites.Click += (s, e) => SwitchTab(2);

            btnEdit.Click += btnEdit_Click;
            btnSettings.Click += btnSettings_Click;

            this.Load += UC_Library_Load;
        }

        private async void UC_Library_Load(object sender, EventArgs e)
        {
            await LoadUserProfileAsync();
            await LoadBooksDataAsync();
        }

        public async Task RefreshLibraryData()
        {
            await LoadUserProfileAsync();
            await LoadBooksDataAsync();
        }

        private async Task LoadUserProfileAsync()
        {
            try
            {
                var profile = await FirebaseDatabaseService.GetCurrentUserProfileAsync();
                if (profile != null)
                {
                    lblUsername.Text = profile.Username;
                    DrawAvatar(pbAvatar, profile.Username);
                }
                else
                {
                    lblUsername.Text = "Người đọc";
                    DrawAvatar(pbAvatar, "Người đọc");
                }

                // Load Bio
                string bio = await FirebaseDatabaseService.GetUserBioAsync();
                lblBio.Text = bio;

                // Load dynamic book count
                int bookCount = await FirebaseDatabaseService.GetUserBookCountAsync();
                lblPostedCount.Text = $"{bookCount} Truyện đã đăng";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading profile: " + ex.Message);
            }
        }

        private async Task LoadBooksDataAsync()
        {
            this.Cursor = Cursors.WaitCursor;
            allBooks = await FirebaseDatabaseService.GetAllBooksAsync();
            this.Cursor = Cursors.Default;

            await RenderBooksListAsync();
        }

        private void SwitchTab(int tabIndex)
        {
            activeTab = tabIndex;

            // Reset tab colors
            btnTabBookmark.BackColor = (tabIndex == 0) ? Color.White : Color.Transparent;
            btnTabHistory.BackColor = (tabIndex == 1) ? Color.White : Color.Transparent;
            btnTabFavorites.BackColor = (tabIndex == 2) ? Color.White : Color.Transparent;

            // Update headers
            if (tabIndex == 0)
                lblBodyHeader.Text = "Truyện đã bookmark";
            else if (tabIndex == 1)
                lblBodyHeader.Text = "Lịch sử đọc";
            else
                lblBodyHeader.Text = "Truyện yêu thích";

            _ = RenderBooksListAsync();
        }

        private async Task RenderBooksListAsync()
        {
            flpBooks.Controls.Clear();

            List<string> targetBookIds = new List<string>();

            if (activeTab == 0)
            {
                targetBookIds = await FirebaseDatabaseService.GetBookmarkedBookIdsAsync();
            }
            else if (activeTab == 1)
            {
                targetBookIds = await FirebaseDatabaseService.GetHistoryBookIdsAsync();
            }
            else
            {
                targetBookIds = await FirebaseDatabaseService.GetFavoriteBookIdsAsync();
            }

            if (targetBookIds.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "Không có truyện nào trong danh sách",
                    Font = new Font("Segoe UI", 9.75F, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(15)
                };
                flpBooks.Controls.Add(lblEmpty);
                return;
            }

            // Map target IDs to BookModels
            var filteredBooks = new List<BookModel>();
            foreach (var id in targetBookIds)
            {
                foreach (var b in allBooks)
                {
                    if (b.Id == id)
                    {
                        filteredBooks.Add(b);
                        break;
                    }
                }
            }

            foreach (var b in filteredBooks)
            {
                BookCard card = CreateBookCard(b);
                flpBooks.Controls.Add(card);
            }
        }

        private BookCard CreateBookCard(BookModel b)
        {
            BookCard card = new BookCard();
            card.lblTitle.Text = b.Title;
            card.lblAuthor.Text = b.AuthorName ?? "Ẩn danh";
            card.lblRating.Text = $"★ {b.Rating:F1}";
            card.lblViews.Text = $"{b.Views} xem";

            if (!string.IsNullOrEmpty(b.CoverBase64))
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(b.CoverBase64);
                    using (var ms = new MemoryStream(bytes))
                    {
                        card.pictureBox1.Image = Image.FromStream(ms);
                        card.pictureBox1.Image = (Image)card.pictureBox1.Image.Clone();
                    }
                }
                catch
                {
                    card.pictureBox1.Image = null;
                }
            }

            // Clicking opens book detail view
            Action click = () =>
            {
                if (this.ParentForm is MainForm mf)
                {
                    mf.ShowBookDetail(b);
                }
            };

            card.Click += (s, e) => click();
            card.pictureBox1.Click += (s, e) => click();
            card.lblTitle.Click += (s, e) => click();
            card.lblAuthor.Click += (s, e) => click();
            card.lblRating.Click += (s, e) => click();
            card.lblViews.Click += (s, e) => click();

            return card;
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            // Trigger VisualBasic input box to edit bio tagline
            string currentBio = lblBio.Text;
            string newBio = Microsoft.VisualBasic.Interaction.InputBox(
                "Nhập tiểu sử (bio) mới của bạn:", 
                "Chỉnh sửa tiểu sử", 
                currentBio);

            if (newBio != null && newBio != currentBio)
            {
                lblBio.Text = newBio;
                await FirebaseDatabaseService.UpdateUserBioAsync(newBio);
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Màn hình Cài đặt cá nhân sẽ được phát triển sau!", "Cài đặt");
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
                using (Font f = new Font("Segoe UI", pb.Width > 50 ? 24 : 12, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    SizeF size = pe.Graphics.MeasureString(letter, f);
                    pe.Graphics.DrawString(letter, f, textBrush, (pb.Width - size.Width) / 2, (pb.Height - size.Height) / 2);
                }
            }
        }
    }
}
