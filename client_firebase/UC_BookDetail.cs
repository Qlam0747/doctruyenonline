using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class UC_BookDetail : UserControl
    {
        private BookModel currentBook;
        private List<ChapterModel> chaptersList = new List<ChapterModel>();
        private Button btnAddChapter = null;
        private Button btnFavorite = null;
        private Button btnFollowAuthor = null;
        private bool isFavorite = false;
        private bool isFollowingAuthor = false;
        private TableLayoutPanel tlpButtons = null;
        private Button btnBookmarkDetail = null;
        private bool isBookmarked = false;

        public UC_BookDetail()
        {
            InitializeComponent();

            txtComment.GotFocus += (s, e) => { if (txtComment.Text == "Viết bình luận của bạn...") { txtComment.Text = ""; txtComment.ForeColor = Color.Black; } };
            txtComment.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtComment.Text)) { txtComment.Text = "Viết bình luận của bạn..."; txtComment.ForeColor = Color.Gray; } };

            InitializeButtonsLayout();

            btnRead.Click += btnRead_Click;
            btnRate.Click += btnRate_Click;
            btnChat.Click += btnChat_Click;
            btnPostComment.Click += btnPostComment_Click;
        }

        private void InitializeButtonsLayout()
        {
            if (tlpButtons == null)
            {
                tlpButtons = new TableLayoutPanel
                {
                    Location = new Point(160, 172),
                    Size = new Size(panelInfoCard.Width - 175, 36),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    ColumnCount = 4,
                    RowCount = 1,
                    BackColor = Color.Transparent
                };

                tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

                tlpButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                panelInfoCard.Controls.Add(tlpButtons);

                
                panelInfoCard.Controls.Remove(btnRead);
                panelInfoCard.Controls.Remove(btnRate);
                panelInfoCard.Controls.Remove(btnChat);

                btnRead.Dock = DockStyle.Fill;
                btnRead.Margin = new Padding(3);
                btnRead.FlatAppearance.BorderSize = 0;

                btnRate.Dock = DockStyle.Fill;
                btnRate.Margin = new Padding(3);
                btnRate.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
                btnRate.FlatAppearance.BorderSize = 1;

                btnChat.Dock = DockStyle.Fill;
                btnChat.Margin = new Padding(3);
                btnChat.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
                btnChat.FlatAppearance.BorderSize = 1;

                tlpButtons.Controls.Add(btnRead, 0, 0);
                
                tlpButtons.Controls.Add(btnRate, 2, 0);
                
                tlpButtons.Controls.Add(btnChat, 3, 0);
            }
        }

        public async void SetBook(BookModel book)
        {
            currentBook = book;
            LoadAuthorDetails(book.AuthorId, book.AuthorName);

            
            var favoriteTask = FirebaseDatabaseService.IsFavoriteAsync(book.Id);
            var followAuthorTask = FirebaseDatabaseService.IsFollowingAuthorAsync(book.AuthorId);
            var bookmarkTask = FirebaseDatabaseService.IsBookmarkedAsync(book.Id);
            var chaptersTask = FirebaseDatabaseService.GetChaptersAsync(book.Id);
            var commentsTask = FirebaseDatabaseService.GetCommentsAsync(book.Id);

            await Task.WhenAll(favoriteTask, followAuthorTask, bookmarkTask, chaptersTask, commentsTask);

            isFavorite = favoriteTask.Result;
            isFollowingAuthor = followAuthorTask.Result;
            isBookmarked = bookmarkTask.Result;
            chaptersList = chaptersTask.Result;
            var commentsList = commentsTask.Result;

            
            if (btnFavorite == null)
            {
                btnFavorite = new Button
                {
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(3)
                };
                btnFavorite.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
                btnFavorite.FlatAppearance.BorderSize = 1;
                btnFavorite.Click += btnFavorite_Click;
                tlpButtons.Controls.Add(btnFavorite, 1, 0);
            }
            UpdateFavoriteButtonUI();

            
            if (btnFollowAuthor == null)
            {
                btnFollowAuthor = new Button
                {
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Size = new Size(85, 30),
                    Location = new Point(395, 45),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                };
                btnFollowAuthor.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
                btnFollowAuthor.FlatAppearance.BorderSize = 1;
                btnFollowAuthor.Click += btnFollowAuthor_Click;
                panelInfoCard.Controls.Add(btnFollowAuthor);
            }

            if (book.AuthorId == AuthSession.FirebaseLocalId)
            {
                btnFollowAuthor.Visible = false;
            }
            else
            {
                btnFollowAuthor.Visible = true;
                UpdateFollowAuthorButtonUI();
            }

            
            if (btnBookmarkDetail == null)
            {
                btnBookmarkDetail = new Button
                {
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Size = new Size(115, 30),
                    Location = new Point(490, 45),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                };
                btnBookmarkDetail.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
                btnBookmarkDetail.FlatAppearance.BorderSize = 1;
                btnBookmarkDetail.Click += btnBookmarkDetail_Click;
                panelInfoCard.Controls.Add(btnBookmarkDetail);
            }
            UpdateBookmarkButtonUI();

            
            if (book.AuthorId == AuthSession.FirebaseLocalId)
            {
                if (btnAddChapter == null)
                {
                    btnAddChapter = new Button
                    {
                        Text = "➕ Thêm chương",
                        Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand,
                        BackColor = Color.FromArgb(46, 204, 113), 
                        ForeColor = Color.White,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(3)
                    };
                    btnAddChapter.FlatAppearance.BorderSize = 0;
                    btnAddChapter.Click += btnAddChapter_Click;
                    tlpButtons.Controls.Add(btnAddChapter, 3, 0);
                }
                btnAddChapter.Visible = true;
                btnChat.Visible = false;
            }
            else
            {
                if (btnAddChapter != null) btnAddChapter.Visible = false;
                btnChat.Visible = true;
            }

            
            lblTitle.Text = book.Title;
            lblDescription.Text = book.Description;

            
            int totalChapterViews = 0;
            if (chaptersList != null)
            {
                foreach (var ch in chaptersList)
                {
                    totalChapterViews += ch.Views;
                }
            }
            lblViewsCount.Text = $"👁 {totalChapterViews} lượt xem";
            lblLikesCount.Text = $"♡ {book.Likes} lượt thích";
            
            
            int roundedStars = (int)Math.Round(book.Rating);
            lblRating.Text = $"⭐ {book.Rating:F1} (" + new string('★', roundedStars) + new string('☆', 5 - roundedStars) + ")";

            
            Label lblStatus = panelInfoCard.Controls["lblStatus"] as Label;
            if (lblStatus == null)
            {
                lblStatus = new Label
                {
                    Name = "lblStatus",
                    Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                    ForeColor = Color.FromArgb(108, 92, 231),
                    Location = new Point(610, 145),
                    Size = new Size(130, 15),
                    AutoSize = true
                };
                panelInfoCard.Controls.Add(lblStatus);
            }
            
            
            
            

            
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

            
            lblChaptersCount.Text = $"📖 {chaptersList.Count} chương";
            RenderChaptersList();
            RenderCommentsList(commentsList);
        }

        private void RenderChaptersList()
        {
            flpChapters.Controls.Clear();
            if (currentBook == null) return;

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
                    Text = $"👁 {ch.Views}",
                    Font = new Font("Segoe UI", 8F),
                    ForeColor = Color.Gray,
                    Location = new Point(row.Width - 100, 15),
                    Size = new Size(80, 15),
                    TextAlign = ContentAlignment.TopRight
                };

                row.Controls.Add(lblChName);
                row.Controls.Add(lblDate);
                row.Controls.Add(lblChViews);

                
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

        private async void RenderCommentsList(List<CommentModel> comments)
        {
            flpComments.Controls.Clear();
            if (currentBook == null) return;

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

            
            var likedStatuses = new Dictionary<string, bool>();
            try
            {
                var tasks = comments.Select(async c =>
                {
                    bool isLiked = await FirebaseDatabaseService.IsCommentLikedAsync(currentBook.Id, c.Id);
                    lock (likedStatuses)
                    {
                        likedStatuses[c.Id] = isLiked;
                    }
                });
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading comment liked statuses: " + ex.Message);
            }

            // Fetch all users to dynamically resolve name and avatar
            var usersList = new List<UserModel>();
            try
            {
                usersList = await FirebaseDatabaseService.GetAllUsersAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error fetching all users for comments: " + ex.Message);
            }

            foreach (var c in comments)
            {
                bool isLiked = likedStatuses.ContainsKey(c.Id) && likedStatuses[c.Id];

                // Find the latest user details
                var commenter = usersList?.FirstOrDefault(u => u.LocalId == c.UserId);
                string displayUsername = commenter?.Username ?? c.Username ?? "Ẩn danh";
                string avatarBase64 = commenter?.Avatar ?? c.Avatar;

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
                DrawAvatar(pb, displayUsername, avatarBase64);

                Panel bubble = new Panel
                {
                    BackColor = Color.FromArgb(235, 236, 240),
                    Location = new Point(50, 5),
                    Width = (int)((row.Width - 60) * 0.9),
                };

                Label lblUser = new Label
                {
                    Text = displayUsername,
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

                
                Size commentSize;
                using (Graphics g = flpComments.CreateGraphics())
                {
                    SizeF sf = g.MeasureString(c.Text, lblText.Font, bubble.Width - 20);
                    commentSize = new Size((int)Math.Ceiling(sf.Width) + 5, (int)Math.Ceiling(sf.Height) + 5);
                }
                lblText.Size = commentSize;
                bubble.Size = new Size(bubble.Width, commentSize.Height + 35); 

                bubble.Controls.Add(lblUser);
                bubble.Controls.Add(lblText);

                Label lblHeart = new Label
                {
                    Text = isLiked ? "❤️ Đã thích" : "♡ Thích",
                    Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                    ForeColor = isLiked ? Color.FromArgb(231, 76, 60) : Color.FromArgb(108, 92, 231),
                    Cursor = Cursors.Hand,
                    AutoSize = true,
                    Location = new Point(55, bubble.Bottom + 2),
                    Tag = isLiked
                };

                Label lblLikes = new Label
                {
                    Text = $"|   {c.Likes} lượt thích   |   {FormatTime(c.Timestamp)}",
                    Font = new Font("Segoe UI", 7.5F, FontStyle.Regular),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(lblHeart.Right + 5, bubble.Bottom + 2)
                };

                lblHeart.Click += async (lblSender, lblEvent) =>
                {
                    lblHeart.Enabled = false;
                    bool curLiked = (bool)lblHeart.Tag;

                    int newLikes = await FirebaseDatabaseService.ToggleCommentLikeAsync(currentBook.Id, c.Id);
                    c.Likes = newLikes;

                    bool nextLiked = !curLiked;
                    lblHeart.Tag = nextLiked;
                    lblHeart.Text = nextLiked ? "❤️ Đã thích" : "♡ Thích";
                    lblHeart.ForeColor = nextLiked ? Color.FromArgb(231, 76, 60) : Color.FromArgb(108, 92, 231);
                    lblLikes.Text = $"|   {newLikes} lượt thích   |   {FormatTime(c.Timestamp)}";
                    lblLikes.Location = new Point(lblHeart.Right + 5, bubble.Bottom + 2);

                    lblHeart.Enabled = true;
                };

                row.Controls.Add(pb);
                row.Controls.Add(bubble);
                row.Controls.Add(lblHeart);
                row.Controls.Add(lblLikes);

                
                row.Height = bubble.Bottom + 25;

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
                
                var updatedComments = await FirebaseDatabaseService.GetCommentsAsync(currentBook.Id);
                RenderCommentsList(updatedComments);
            }
            else
            {
                MessageBox.Show("Đăng bình luận thất bại.", "Lỗi");
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



        private void UpdateFavoriteButtonUI()
        {
            if (isFavorite)
            {
                btnFavorite.Text = "❤️ Yêu thích";
                btnFavorite.BackColor = Color.FromArgb(231, 76, 60); 
                btnFavorite.ForeColor = Color.White;
                btnFavorite.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btnFavorite.Text = "♡ Yêu thích";
                btnFavorite.BackColor = Color.White;
                btnFavorite.ForeColor = Color.Black;
                btnFavorite.FlatAppearance.BorderSize = 1;
                btnFavorite.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
            }
        }

        private void UpdateFollowAuthorButtonUI()
        {
            if (isFollowingAuthor)
            {
                btnFollowAuthor.Text = "👥 Followed";
                btnFollowAuthor.BackColor = Color.FromArgb(52, 152, 219); 
                btnFollowAuthor.ForeColor = Color.White;
                btnFollowAuthor.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btnFollowAuthor.Text = "👤 Follow";
                btnFollowAuthor.BackColor = Color.White;
                btnFollowAuthor.ForeColor = Color.Black;
                btnFollowAuthor.FlatAppearance.BorderSize = 1;
                btnFollowAuthor.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
            }
        }

        private async void btnFavorite_Click(object sender, EventArgs e)
        {
            if (currentBook == null) return;
            btnFavorite.Enabled = false;
            isFavorite = !isFavorite;
            if (isFavorite)
            {
                await FirebaseDatabaseService.AddToFavoritesAsync(currentBook.Id);
                int newLikes = await FirebaseDatabaseService.ToggleBookLikeAsync(currentBook.Id, isAdd: true);
                currentBook.Likes = newLikes;
                lblLikesCount.Text = $"♡ {newLikes} lượt thích";
                MessageBox.Show("Đã thêm truyện vào danh sách yêu thích!", "Yêu thích");
            }
            else
            {
                await FirebaseDatabaseService.RemoveFromFavoritesAsync(currentBook.Id);
                int newLikes = await FirebaseDatabaseService.ToggleBookLikeAsync(currentBook.Id, isAdd: false);
                currentBook.Likes = newLikes;
                lblLikesCount.Text = $"♡ {newLikes} lượt thích";
                MessageBox.Show("Đã xóa truyện khỏi danh sách yêu thích!", "Yêu thích");
            }
            UpdateFavoriteButtonUI();
            btnFavorite.Enabled = true;
        }

        private async void btnFollowAuthor_Click(object sender, EventArgs e)
        {
            if (currentBook == null || string.IsNullOrEmpty(currentBook.AuthorId)) return;
            btnFollowAuthor.Enabled = false;
            isFollowingAuthor = !isFollowingAuthor;
            if (isFollowingAuthor)
            {
                await FirebaseDatabaseService.FollowAuthorAsync(currentBook.AuthorId);
            }
            else
            {
                await FirebaseDatabaseService.UnfollowAuthorAsync(currentBook.AuthorId);
            }
            UpdateFollowAuthorButtonUI();
            btnFollowAuthor.Enabled = true;
        }

        private void UpdateBookmarkButtonUI()
        {
            if (btnBookmarkDetail != null)
            {
                if (isBookmarked)
                {
                    btnBookmarkDetail.Text = "🔖 Đã Bookmark";
                    btnBookmarkDetail.BackColor = Color.FromArgb(108, 92, 231);
                    btnBookmarkDetail.ForeColor = Color.White;
                    btnBookmarkDetail.FlatAppearance.BorderSize = 0;
                }
                else
                {
                    btnBookmarkDetail.Text = "🔖 Bookmark";
                    btnBookmarkDetail.BackColor = Color.White;
                    btnBookmarkDetail.ForeColor = Color.Black;
                    btnBookmarkDetail.FlatAppearance.BorderSize = 1;
                    btnBookmarkDetail.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
                }
            }
        }

        private async void btnBookmarkDetail_Click(object sender, EventArgs e)
        {
            if (currentBook == null) return;
            btnBookmarkDetail.Enabled = false;
            isBookmarked = !isBookmarked;
            if (isBookmarked)
            {
                await FirebaseDatabaseService.AddToBookmarksAsync(currentBook.Id);
                MessageBox.Show("Đã đánh dấu bookmark truyện và đăng ký nhận thông báo chương mới thành công!", "Bookmark");
            }
            else
            {
                await FirebaseDatabaseService.RemoveFromBookmarksAsync(currentBook.Id);
                MessageBox.Show("Đã bỏ bookmark truyện và hủy đăng ký nhận thông báo chương mới.", "Bookmark");
            }
            UpdateBookmarkButtonUI();
            btnBookmarkDetail.Enabled = true;
        }

        private async void btnRate_Click(object sender, EventArgs e)
        {
            if (currentBook == null) return;

            
            using (Form rateForm = new Form())
            {
                rateForm.Text = "Đánh giá truyện";
                rateForm.Size = new Size(300, 160);
                rateForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                rateForm.StartPosition = FormStartPosition.CenterParent;
                rateForm.MaximizeBox = false;
                rateForm.MinimizeBox = false;

                Label lblPrompt = new Label() { Text = "Chọn số sao đánh giá (1-5):", Left = 20, Top = 20, Width = 260 };
                
                ComboBox cbStars = new ComboBox() { Left = 20, Top = 45, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
                cbStars.Items.AddRange(new object[] { "★ (1 sao)", "★★ (2 sao)", "★★★ (3 sao)", "★★★★ (4 sao)", "★★★★★ (5 sao)" });
                cbStars.SelectedIndex = 4; 

                Button btnSubmit = new Button() { Text = "Gửi", Left = 60, Top = 80, Width = 80, DialogResult = DialogResult.OK };
                Button btnCancel = new Button() { Text = "Hủy", Left = 160, Top = 80, Width = 80, DialogResult = DialogResult.Cancel };

                rateForm.Controls.Add(lblPrompt);
                rateForm.Controls.Add(cbStars);
                rateForm.Controls.Add(btnSubmit);
                rateForm.Controls.Add(btnCancel);
                
                rateForm.AcceptButton = btnSubmit;
                rateForm.CancelButton = btnCancel;

                if (rateForm.ShowDialog() == DialogResult.OK)
                {
                    double ratingValue = cbStars.SelectedIndex + 1;
                    btnRate.Enabled = false;
                    double newAvg = await FirebaseDatabaseService.RateBookAsync(currentBook.Id, ratingValue);
                    btnRate.Enabled = true;

                    currentBook.Rating = newAvg;
                    int roundedStars = (int)Math.Round(newAvg);
                    lblRating.Text = $"⭐ {newAvg:F1} (" + new string('★', roundedStars) + new string('☆', 5 - roundedStars) + ")";
                    MessageBox.Show($"Đã gửi đánh giá {ratingValue} sao thành công! Đánh giá trung bình mới: {newAvg:F1}", "Đánh giá");
                }
            }
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

        private async void LoadAuthorDetails(string authorId, string defaultName)
        {
            lblAuthorName.Text = defaultName ?? "Ẩn danh";
            DrawAvatar(pbAuthor, defaultName ?? "Ẩn danh");

            if (string.IsNullOrEmpty(authorId)) return;

            var authorProfile = await FirebaseDatabaseService.GetUserProfileAsync(authorId);
            if (authorProfile != null)
            {
                lblAuthorName.Text = authorProfile.Username ?? "Ẩn danh";
                DrawAvatar(pbAuthor, authorProfile.Username, authorProfile.Avatar);
            }
        }

        public void RefreshBookDetail()
        {
            if (currentBook != null)
            {
                SetBook(currentBook);
            }
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

        private async void btnAddChapter_Click(object sender, EventArgs e)
        {
            if (currentBook == null) return;

            int nextChapterNum = chaptersList.Count + 1;
            using (var form = new FormAddChapter(currentBook.Id, currentBook.Title, currentBook.Description, nextChapterNum, currentBook.Status))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    
                    var updatedBook = await FirebaseDatabaseService.GetBookByIdAsync(currentBook.Id);
                    if (updatedBook != null)
                    {
                        currentBook = updatedBook;
                        var lblStatus = panelInfoCard.Controls["lblStatus"] as Label;
                        if (lblStatus != null)
                        {
                            lblStatus.Text = $"({currentBook.Status ?? "Đang tiến hành"})";
                        }
                    }

                    chaptersList = await FirebaseDatabaseService.GetChaptersAsync(currentBook.Id);
                    lblChaptersCount.Text = $"📖 {chaptersList.Count} chương";
                    RenderChaptersList();
                }
            }
        }
    }
}
