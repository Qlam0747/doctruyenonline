using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class UC_Search : UserControl
    {
        private List<BookModel> _allBooks = new List<BookModel>();
        private List<string> _selectedGenres = new List<string>();
        private string _activeSort = "Lượt xem"; 
        private Button btnAISearch;

        private readonly string[] GenresList = new string[]
        {
            "Phiêu lưu", "Lãng mạn", "Viễn tưởng", "Kinh dị", "Hài hước",
            "Hành động", "Dã sử", "Trinh thám", "Cổ đại", "Huyền huyễn",
            "Đô thị", "Học đường", "Võ hiệp", "Khoa học"
        };

        private readonly string[] SortList = new string[]
        {
            "Lượt xem", "Yêu thích", "Mới cập nhật"
        };

        public UC_Search()
        {
            InitializeComponent();

            
            panelSearchInput.Width = 510;
            txtSearch.Width = 460;

            btnAISearch = new Button
            {
                Text = "🤖 Tìm bằng AI",
                Font = new Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold),
                Size = new Size(115, 40),
                Location = new Point(540, 20),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.FromArgb(162, 155, 254), 
                ForeColor = Color.White
            };
            btnAISearch.FlatAppearance.BorderSize = 0;
            btnAISearch.Click += btnAISearch_Click;
            this.Controls.Add(btnAISearch);
            
            
            this.Load += UC_Search_Load;
            this.VisibleChanged += UC_Search_VisibleChanged;
            btnFilter.Click += btnFilter_Click;
            txtSearch.TextChanged += txtSearch_TextChanged;
        }

        private async void UC_Search_Load(object sender, EventArgs e)
        {
            InitializeFilterOptions();
            UpdateLayoutPositions();
            await LoadBooksAsync();
        }

        private async void UC_Search_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                await LoadBooksAsync();
            }
        }

        private void InitializeFilterOptions()
        {
            flpGenres.Controls.Clear();
            foreach (var genre in GenresList)
            {
                Button btn = CreateTagButton(genre, isGenre: true);
                flpGenres.Controls.Add(btn);
            }

            flpSort.Controls.Clear();
            foreach (var sortOption in SortList)
            {
                Button btn = CreateTagButton(sortOption, isGenre: false);
                
                if (sortOption == _activeSort)
                {
                    HighlightButton(btn, active: true);
                }
                flpSort.Controls.Add(btn);
            }
        }

        private Button CreateTagButton(string text, bool isGenre)
        {
            Button btn = new Button
            {
                Text = text,
                AutoSize = true,
                Height = 28,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                Margin = new Padding(0, 0, 8, 8),
                Cursor = Cursors.Hand,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(120, 120, 120)
            };
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 238, 255);

            btn.Click += (s, e) =>
            {
                if (isGenre)
                {
                    if (_selectedGenres.Contains(text))
                    {
                        _selectedGenres.Remove(text);
                        HighlightButton(btn, active: false);
                    }
                    else
                    {
                        _selectedGenres.Add(text);
                        HighlightButton(btn, active: true);
                    }
                }
                else
                {
                    
                    _activeSort = text;
                    foreach (Control ctrl in flpSort.Controls)
                    {
                        if (ctrl is Button sortBtn)
                        {
                            HighlightButton(sortBtn, sortBtn.Text == _activeSort);
                        }
                    }
                }

                PerformSearch();
            };

            return btn;
        }

        private void HighlightButton(Button btn, bool active)
        {
            if (active)
            {
                btn.BackColor = Color.FromArgb(108, 92, 231); 
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderColor = Color.FromArgb(108, 92, 231);
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(90, 75, 200);
            }
            else
            {
                btn.BackColor = Color.White;
                btn.ForeColor = Color.FromArgb(120, 120, 120);
                btn.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 238, 255);
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            panelFilters.Visible = !panelFilters.Visible;
            UpdateLayoutPositions();
        }

        private void UpdateLayoutPositions()
        {
            if (panelFilters.Visible)
            {
                lblResultsTitle.Top = panelFilters.Bottom + 15;
            }
            else
            {
                lblResultsTitle.Top = panelSearchInput.Bottom + 15;
            }
            flpResults.Top = lblResultsTitle.Bottom + 10;
            flpResults.Height = this.Height - flpResults.Top - 20;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayoutPositions();
        }

        public async Task LoadBooksAsync()
        {
            try
            {
                var books = await FirebaseDatabaseService.GetAllBooksAsync();
                var users = await FirebaseDatabaseService.GetAllUsersAsync();
                if (books != null)
                {
                    if (users != null)
                    {
                        foreach (var b in books)
                        {
                            var author = users.FirstOrDefault(u => u.LocalId == b.AuthorId);
                            if (author != null)
                            {
                                b.AuthorName = author.Username;
                            }
                        }
                    }
                    _allBooks = books;
                    PerformSearch();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading search books: " + ex.Message);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            if (_allBooks == null) return;

            flpResults.Controls.Clear();

            string query = txtSearch.Text.Trim().ToLower();

            
            var filtered = _allBooks.Where(b =>
            {
                bool matchesQuery = string.IsNullOrEmpty(query) || 
                                    b.Title.ToLower().Contains(query) || 
                                    (b.AuthorName != null && b.AuthorName.ToLower().Contains(query));

                bool matchesGenres = _selectedGenres.Count == 0 || 
                                     _selectedGenres.All(g => b.Genres != null && b.Genres.Any(bg => string.Equals(bg, g, StringComparison.OrdinalIgnoreCase)));

                return matchesQuery && matchesGenres;
            }).ToList();

            
            if (_activeSort == "Lượt xem")
            {
                filtered.Sort((x, y) => y.Views.CompareTo(x.Views));
            }
            else if (_activeSort == "Yêu thích")
            {
                filtered.Sort((x, y) => {
                    int comp = y.Likes.CompareTo(x.Likes);
                    if (comp == 0) return y.Rating.CompareTo(x.Rating);
                    return comp;
                });
            }
            else if (_activeSort == "Mới cập nhật")
            {
                filtered.Sort((x, y) => y.UpdatedAt.CompareTo(x.UpdatedAt));
            }

            
            if (filtered.Count == 0)
            {
                Label lblNoResults = new Label
                {
                    Text = "Không tìm thấy truyện nào khớp với bộ lọc.",
                    Font = new Font("Segoe UI", 11.25F, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(20, 20, 0, 0)
                };
                flpResults.Controls.Add(lblNoResults);
            }
            else
            {
                foreach (var book in filtered)
                {
                    SearchBookCard card = new SearchBookCard(book);
                    flpResults.Controls.Add(card);
                }
            }
        }

        private async void btnAISearch_Click(object sender, EventArgs e)
        {
            string query = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Vui lòng nhập mô tả hoặc ý tưởng truyện bạn muốn tìm (ví dụ: 'phiêu lưu phép thuật kỳ ảo có mèo máy') vào ô tìm kiếm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSearch.Focus();
                return;
            }

            btnAISearch.Enabled = false;
            btnAISearch.Text = "🤖 Đang tìm...";
            this.Cursor = Cursors.WaitCursor;

            try
            {
                var matchedIds = await GeminiService.SearchBooksAIAsync(query, _allBooks);
                PerformAISearch(matchedIds);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm AI: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnAISearch.Enabled = true;
                btnAISearch.Text = "🤖 Tìm bằng AI";
                this.Cursor = Cursors.Default;
            }
        }

        private void PerformAISearch(List<string> matchedIds)
        {
            flpResults.Controls.Clear();
            if (matchedIds == null || matchedIds.Count == 0)
            {
                Label lblNoResults = new Label
                {
                    Text = "Không tìm thấy truyện nào phù hợp với mô tả AI của bạn.",
                    Font = new Font("Segoe UI", 11.25F, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(20, 20, 0, 0)
                };
                flpResults.Controls.Add(lblNoResults);
                return;
            }

            var matchedBooks = new List<BookModel>();
            foreach (var id in matchedIds)
            {
                var book = _allBooks.FirstOrDefault(b => b.Id == id);
                if (book != null)
                {
                    matchedBooks.Add(book);
                }
            }

            if (matchedBooks.Count == 0)
            {
                Label lblNoResults = new Label
                {
                    Text = "Không tìm thấy truyện nào phù hợp với mô tả AI của bạn.",
                    Font = new Font("Segoe UI", 11.25F, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(20, 20, 0, 0)
                };
                flpResults.Controls.Add(lblNoResults);
            }
            else
            {
                foreach (var book in matchedBooks)
                {
                    SearchBookCard card = new SearchBookCard(book);
                    flpResults.Controls.Add(card);
                }
            }
        }
    }
}
