using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class UC_Reading : UserControl
    {
        private BookModel currentBook;
        private List<ChapterModel> chaptersList = new List<ChapterModel>();
        private int currentChapterIndex = -1;
        private float[] fontSizes = { 10f, 11.25f, 13f, 15f, 17f };
        private int currentFontSizeIndex = 1; // Default to index 1 (11.25f)

        public UC_Reading()
        {
            InitializeComponent();

            btnHome.Click += btnHome_Click;
            btnList.Click += btnList_Click;
            btnListBottom.Click += btnList_Click;
            btnPrev.Click += btnPrev_Click;
            btnNext.Click += btnNext_Click;
            btnSettings.Click += btnSettings_Click;

            cbChapters.SelectedIndexChanged += cbChapters_SelectedIndexChanged;
        }

        public async void SetBook(BookModel book, string chapterNum)
        {
            currentBook = book;
            lblHeaderBookTitle.Text = book.Title;
            lblAuthor.Text = book.AuthorName ?? "Ẩn danh";

            // Log to reading history
            await FirebaseDatabaseService.AddToHistoryAsync(book.Id);
            await FirebaseDatabaseService.IncrementBookViewsAsync(book.Id);

            this.Cursor = Cursors.WaitCursor;
            chaptersList = await FirebaseDatabaseService.GetChaptersAsync(book.Id);
            this.Cursor = Cursors.Default;

            // Bind to combobox
            cbChapters.SelectedIndexChanged -= cbChapters_SelectedIndexChanged; // Temporarily unbind
            cbChapters.Items.Clear();

            int targetIndex = 0;
            for (int i = 0; i < chaptersList.Count; i++)
            {
                var ch = chaptersList[i];
                cbChapters.Items.Add($"Chương {ch.ChapterNumber}: {ch.Title}");
                if (ch.ChapterNumber == chapterNum)
                {
                    targetIndex = i;
                }
            }
            cbChapters.SelectedIndexChanged += cbChapters_SelectedIndexChanged;

            if (cbChapters.Items.Count > 0)
            {
                cbChapters.SelectedIndex = targetIndex;
            }
            else
            {
                lblChapTitle.Text = "Chưa có chương";
                txtContent.Text = "Truyện chưa có nội dung để đọc.";
                btnPrev.Enabled = false;
                btnNext.Enabled = false;
            }
        }

        private void LoadChapterByIndex(int index)
        {
            if (index < 0 || index >= chaptersList.Count) return;
            currentChapterIndex = index;

            var ch = chaptersList[index];

            // Update details
            string titleText = $"Chương {ch.ChapterNumber}: {ch.Title}";
            lblHeaderChapName.Text = titleText;
            lblChapTitle.Text = titleText;
            txtContent.Text = ch.Content;

            // Enable/disable buttons
            btnPrev.Enabled = (index > 0);
            btnNext.Enabled = (index < chaptersList.Count - 1);
        }

        private void cbChapters_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadChapterByIndex(cbChapters.SelectedIndex);
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentChapterIndex > 0)
            {
                cbChapters.SelectedIndex = currentChapterIndex - 1;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentChapterIndex < chaptersList.Count - 1)
            {
                cbChapters.SelectedIndex = currentChapterIndex + 1;
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (this.ParentForm is MainForm mf)
            {
                mf.GoToHome();
            }
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            if (currentBook != null && this.ParentForm is MainForm mf)
            {
                mf.ShowBookDetail(currentBook);
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            // Cycle through font sizes
            currentFontSizeIndex = (currentFontSizeIndex + 1) % fontSizes.Length;
            float newSize = fontSizes[currentFontSizeIndex];
            txtContent.Font = new Font("Segoe UI", newSize, FontStyle.Regular);
            
            MessageBox.Show($"Đã thay đổi cỡ chữ đọc sang: {newSize}pt", "Cài đặt đọc", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
