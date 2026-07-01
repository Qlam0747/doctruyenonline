using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class UC_Library : UserControl
    {
        private int activeTab = 0; // 0 = Bookmark, 1 = History, 2 = Favorites, 3 = Posted
        private List<BookModel> allBooks = new List<BookModel>();
        private List<string> cachedBookmarks = new List<string>();
        private Dictionary<string, List<BookmarkModel>> cachedBookmarksMap = new Dictionary<string, List<BookmarkModel>>();
        private List<string> cachedHistory = new List<string>();
        private List<string> cachedFavorites = new List<string>();

        private Button btnTabPosted;

        public UC_Library()
        {
            InitializeComponent();

            // Set up dynamic tab button for Posted stories
            btnTabPosted = new Button
            {
                Cursor = Cursors.Hand,
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                Text = "✍  Đã đăng",
                UseVisualStyleBackColor = true
            };
            btnTabPosted.FlatAppearance.BorderSize = 0;

            // Set up a responsive TableLayoutPanel for the tab buttons
            TableLayoutPanel tlpTabs = new TableLayoutPanel
            {
                ColumnCount = 4,
                RowCount = 1,
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };

            tlpTabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0F));
            tlpTabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0F));
            tlpTabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0F));
            tlpTabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0F));
            tlpTabs.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            btnTabBookmark.Dock = DockStyle.Fill;
            btnTabBookmark.Margin = new Padding(2);
            btnTabHistory.Dock = DockStyle.Fill;
            btnTabHistory.Margin = new Padding(2);
            btnTabFavorites.Dock = DockStyle.Fill;
            btnTabFavorites.Margin = new Padding(2);

            panelTabs.Controls.Remove(btnTabBookmark);
            panelTabs.Controls.Remove(btnTabHistory);
            panelTabs.Controls.Remove(btnTabFavorites);

            tlpTabs.Controls.Add(btnTabBookmark, 0, 0);
            tlpTabs.Controls.Add(btnTabHistory, 1, 0);
            tlpTabs.Controls.Add(btnTabFavorites, 2, 0);
            tlpTabs.Controls.Add(btnTabPosted, 3, 0);

            panelTabs.Controls.Add(tlpTabs);

            btnTabBookmark.Click += (s, e) => SwitchTab(0);
            btnTabHistory.Click += (s, e) => SwitchTab(1);
            btnTabFavorites.Click += (s, e) => SwitchTab(2);
            btnTabPosted.Click += (s, e) => SwitchTab(3);

            btnEdit.Click += btnEdit_Click;
            btnSettings.Click += btnSettings_Click;

            pbAvatar.Cursor = Cursors.Hand;
            pbAvatar.Click += pbAvatar_Click;

            // Make followers/following labels clickable
            lblFollowers.Cursor = Cursors.Hand;
            lblFollowing.Cursor = Cursors.Hand;
            lblFollowers.Click += (s, e) => ShowFollowsWindow(0);
            lblFollowing.Click += (s, e) => ShowFollowsWindow(1);

            this.Load += UC_Library_Load;
        }

        private void ShowFollowsWindow(int tabIndex)
        {
            using (var form = new FormFollows(tabIndex))
            {
                form.ShowDialog();
                // Always refresh profile data to update follower/following counts if changed
                _ = RefreshLibraryData();
            }
        }

        private async void UC_Library_Load(object sender, EventArgs e)
        {
            await RefreshLibraryData();
        }

        public async Task RefreshLibraryData()
        {
            var taskProfile = LoadUserProfileAsync();
            var taskBooks = LoadBooksDataAsync();
            await Task.WhenAll(taskProfile, taskBooks);
        }

        private async Task LoadUserProfileAsync()
        {
            try
            {
                var profileTask = FirebaseDatabaseService.GetCurrentUserProfileAsync();
                var bioTask = FirebaseDatabaseService.GetUserBioAsync();
                var bookCountTask = FirebaseDatabaseService.GetUserBookCountAsync();
                var followersTask = FirebaseDatabaseService.GetFollowersCountAsync(AuthSession.FirebaseLocalId);
                var followingTask = FirebaseDatabaseService.GetFollowingCountAsync(AuthSession.FirebaseLocalId);

                await Task.WhenAll(profileTask, bioTask, bookCountTask, followersTask, followingTask);

                var profile = profileTask.Result;
                string bio = bioTask.Result;
                int bookCount = bookCountTask.Result;
                int followersCount = followersTask.Result;
                int followingCount = followingTask.Result;

                if (profile != null)
                {
                    lblUsername.Text = profile.Username;
                    DrawAvatar(pbAvatar, profile.Username, profile.Avatar);
                }
                else
                {
                    lblUsername.Text = "Người đọc";
                    DrawAvatar(pbAvatar, "Người đọc");
                }

                lblBio.Text = bio;
                lblPostedCount.Text = $"{bookCount} Truyện đã đăng";
                lblFollowers.Text = $"{followersCount} Người theo dõi";
                lblFollowing.Text = $"{followingCount} Đang theo dõi";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading profile: " + ex.Message);
            }
        }

        private async Task LoadBooksDataAsync()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                var allBooksTask = FirebaseDatabaseService.GetAllBooksAsync();
                var bookmarksTask = FirebaseDatabaseService.GetAllUserBookmarksAsync();
                var historyTask = FirebaseDatabaseService.GetHistoryBookIdsAsync();
                var favoritesTask = FirebaseDatabaseService.GetFavoriteBookIdsAsync();

                await Task.WhenAll(allBooksTask, bookmarksTask, historyTask, favoritesTask);

                allBooks = allBooksTask.Result;
                cachedBookmarksMap = bookmarksTask.Result;
                cachedBookmarks = new List<string>(cachedBookmarksMap.Keys);
                cachedHistory = historyTask.Result;
                cachedFavorites = favoritesTask.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading books data: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

            await RenderBooksListAsync();
        }

        private void SwitchTab(int tabIndex)
        {
            activeTab = tabIndex;

            // Reset tab colors
            btnTabBookmark.BackColor = (tabIndex == 0) ? Color.White : Color.Transparent;
            btnTabHistory.BackColor = (tabIndex == 1) ? Color.White : Color.Transparent;
            btnTabFavorites.BackColor = (tabIndex == 2) ? Color.White : Color.Transparent;
            btnTabPosted.BackColor = (tabIndex == 3) ? Color.White : Color.Transparent;

            // Update headers
            if (tabIndex == 0)
                lblBodyHeader.Text = "Truyện đã bookmark";
            else if (tabIndex == 1)
                lblBodyHeader.Text = "Lịch sử đọc";
            else if (tabIndex == 2)
                lblBodyHeader.Text = "Truyện yêu thích";
            else
                lblBodyHeader.Text = "Truyện đã đăng";

            _ = RenderBooksListAsync();
        }

        private async Task RenderBooksListAsync()
        {
            flpBooks.Controls.Clear();

            List<string> targetBookIds;

            if (activeTab == 0)
            {
                targetBookIds = cachedBookmarks;
            }
            else if (activeTab == 1)
            {
                targetBookIds = cachedHistory;
            }
            else if (activeTab == 2)
            {
                targetBookIds = cachedFavorites;
            }
            else
            {
                targetBookIds = allBooks
                    .Where(b => b.AuthorId == AuthSession.FirebaseLocalId)
                    .Select(b => b.Id)
                    .ToList();
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
                List<BookmarkModel> bookmarksForBook = null;
                if (activeTab == 0 && cachedBookmarksMap.ContainsKey(b.Id))
                {
                    bookmarksForBook = cachedBookmarksMap[b.Id];
                }
                BookCard card = CreateBookCard(b, bookmarksForBook);
                flpBooks.Controls.Add(card);
            }
        }

        private BookCard CreateBookCard(BookModel b, List<BookmarkModel> bookmarks = null)
        {
            BookCard card = new BookCard();
            card.lblTitle.Text = b.Title;
            card.lblAuthor.Text = b.AuthorName ?? "Ẩn danh";
            card.lblRating.Text = $"★ {b.Rating:F1}";
            card.lblViews.Text = $"{b.Views} xem";

            if (bookmarks != null && bookmarks.Count > 0)
            {
                // Find all unique bookmarked chapters
                var chapters = bookmarks.Select(bm => bm.ChapterNumber).Distinct().OrderBy(c => c).ToList();
                card.lblAuthor.Text = "Chương bm: " + string.Join(", ", chapters);
                card.lblAuthor.ForeColor = Color.FromArgb(108, 92, 231);
                card.lblAuthor.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            }

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

            // Clicking opens book detail view, or resumes reading if bookmarks exist
            Action click = () =>
            {
                if (this.ParentForm is MainForm mf)
                {
                    if (bookmarks != null && bookmarks.Count > 0)
                    {
                        // Open reading screen directly to the last bookmarked chapter
                        var latestBm = bookmarks.OrderByDescending(bm => bm.Timestamp).First();
                        mf.ShowReadingScreen(b, latestBm.ChapterNumber);
                    }
                    else
                    {
                        mf.ShowBookDetail(b);
                    }
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

        private async void btnSettings_Click(object sender, EventArgs e)
        {
            using (var form = new FormEditProfile())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await RefreshLibraryData();
                }
            }
        }

        private async void pbAvatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
                ofd.Title = "Chọn ảnh đại diện của bạn";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        byte[] bytes = File.ReadAllBytes(ofd.FileName);
                        string base64 = Convert.ToBase64String(bytes);

                        this.Cursor = Cursors.WaitCursor;
                        bool success = await FirebaseDatabaseService.UpdateUserAvatarAsync(base64);
                        this.Cursor = Cursors.Default;

                        if (success)
                        {
                            MessageBox.Show("Cập nhật ảnh đại diện thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await RefreshLibraryData();
                            if (this.ParentForm is MainForm mf)
                            {
                                await mf.UpdateProfileAvatar();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật ảnh đại diện thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi đọc file ảnh: " + ex.Message, "Lỗi");
                    }
                }
            }
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
                    return; // Bypass dynamic Paint avatar
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
