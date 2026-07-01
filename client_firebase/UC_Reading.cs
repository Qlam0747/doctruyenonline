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

        private FlowLayoutPanel flpContent;
        private Button btnBookmarksList;
        private Dictionary<int, Label> paragraphControls = new Dictionary<int, Label>();
        private int targetScrollParagraphIndex = -1;

        public UC_Reading()
        {
            InitializeComponent();

            // Set up scrollable flow layout for paragraphs below the header and divider
            flpContent = new FlowLayoutPanel
            {
                Location = new Point(20, 90),
                Size = new Size(730, 335),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.White,
                Padding = new Padding(20)
            };
            panelCard.Controls.Add(flpContent);
            txtContent.Visible = false; // Hide the standard TextBox

            // Update paragraph panel widths dynamically when flpContent resizes
            flpContent.SizeChanged += (s, e) =>
            {
                int targetWidth = flpContent.Width - 40;
                if (targetWidth <= 0) return;

                flpContent.SuspendLayout();
                foreach (Control ctrl in flpContent.Controls)
                {
                    if (ctrl is Panel pPanel)
                    {
                        pPanel.Width = targetWidth;
                        foreach (Control child in pPanel.Controls)
                        {
                            if (child is Label lblPara)
                            {
                                lblPara.MaximumSize = new Size(targetWidth - 60, 0);
                            }
                        }
                    }
                }
                flpContent.ResumeLayout();
            };

            // Set up dynamic bookmark button next to font settings
            btnBookmarksList = new Button
            {
                Text = "🔖",
                Font = new Font("Segoe UI", 12F),
                Size = new Size(35, 35),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnBookmarksList.FlatAppearance.BorderSize = 0;
            btnBookmarksList.Click += btnBookmarksList_Click;
            panelHeader.Controls.Add(btnBookmarksList);

            // Position controls relative to the right side of the control dynamically to prevent WinForms anchoring layout bugs
            int currentWidth = this.Width;
            btnSettings.Location = new Point(currentWidth - 55, 12);
            btnBookmarksList.Location = new Point(currentWidth - 100, 12);
            cbChapters.Location = new Point(currentWidth - 260, 17);

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
                flpContent.Controls.Clear();
                Label lblEmpty = new Label
                {
                    Text = "Truyện chưa có nội dung để đọc.",
                    Font = new Font("Segoe UI", 11.25F, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true
                };
                flpContent.Controls.Add(lblEmpty);
                btnPrev.Enabled = false;
                btnNext.Enabled = false;
            }
        }

        private async void LoadChapterByIndex(int index)
        {
            if (index < 0 || index >= chaptersList.Count) return;
            currentChapterIndex = index;

            var ch = chaptersList[index];

            // Increment views for this chapter and the book in background
            ch.Views++;
            await FirebaseDatabaseService.IncrementChapterViewsAsync(currentBook.Id, ch.Id);
            await FirebaseDatabaseService.IncrementBookViewsAsync(currentBook.Id);

            // Update details
            string titleText = $"Chương {ch.ChapterNumber}: {ch.Title}";
            lblHeaderChapName.Text = titleText;
            lblChapTitle.Text = titleText;

            // Build paragraph layout
            flpContent.Controls.Clear();
            paragraphControls.Clear();

            string[] paragraphs = ch.Content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < paragraphs.Length; i++)
            {
                string pText = paragraphs[i];
                int pIndex = i;

                Panel pPanel = new Panel
                {
                    Width = flpContent.Width - 40,
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    Margin = new Padding(0, 5, 0, 5)
                };

                Label lblPara = new Label
                {
                    Text = pText,
                    Font = new Font("Segoe UI", fontSizes[currentFontSizeIndex], FontStyle.Regular),
                    ForeColor = Color.FromArgb(40, 40, 40),
                    AutoSize = true,
                    MaximumSize = new Size(pPanel.Width - 60, 0),
                    Location = new Point(0, 0),
                    Cursor = Cursors.Hand,
                    Tag = pIndex
                };

                paragraphControls[pIndex] = lblPara;

                lblPara.DoubleClick += (s, e) =>
                {
                    RemoveTemporaryBookmarkButtons();

                    Button btnSaveBm = new Button
                    {
                        Text = "🔖",
                        Size = new Size(35, 30),
                        Location = new Point(lblPara.Right + 5, Math.Max(0, (lblPara.Height - 30) / 2)),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand,
                        BackColor = Color.FromArgb(108, 92, 231),
                        ForeColor = Color.White
                    };
                    btnSaveBm.FlatAppearance.BorderSize = 0;

                    btnSaveBm.Click += async (senderBtn, eBtn) =>
                    {
                        btnSaveBm.Enabled = false;
                        bool success = await FirebaseDatabaseService.AddLineBookmarkAsync(currentBook.Id, ch.ChapterNumber, pIndex, pText);
                        if (success)
                        {
                            btnSaveBm.BackColor = Color.FromArgb(46, 204, 113);
                            MessageBox.Show("Đã lưu bookmark thành công!", "Bookmark");
                            btnSaveBm.Dispose();
                        }
                        else
                        {
                            btnSaveBm.Enabled = true;
                            MessageBox.Show("Không thể lưu bookmark.", "Lỗi");
                        }
                    };

                    pPanel.Controls.Add(btnSaveBm);
                };

                pPanel.Controls.Add(lblPara);
                flpContent.Controls.Add(pPanel);
            }

            // Enable/disable buttons
            btnPrev.Enabled = (index > 0);
            btnNext.Enabled = (index < chaptersList.Count - 1);

            // Scroll to target paragraph if any
            if (targetScrollParagraphIndex != -1)
            {
                ScrollToParagraph(targetScrollParagraphIndex);
            }
        }

        private void ScrollToParagraph(int index)
        {
            if (index != -1 && paragraphControls.ContainsKey(index))
            {
                var targetCtrl = paragraphControls[index];
                flpContent.ScrollControlIntoView(targetCtrl);
                // Highlight
                targetCtrl.BackColor = Color.FromArgb(254, 239, 179); // Pastel yellow highlight
                Timer t = new Timer { Interval = 1500 };
                t.Tick += (s, e) =>
                {
                    targetCtrl.BackColor = Color.Transparent;
                    t.Stop();
                    t.Dispose();
                };
                t.Start();
                targetScrollParagraphIndex = -1;
            }
        }

        private void RemoveTemporaryBookmarkButtons()
        {
            foreach (Control parent in flpContent.Controls)
            {
                if (parent is Panel pPanel)
                {
                    List<Control> toRemove = new List<Control>();
                    foreach (Control child in pPanel.Controls)
                    {
                        if (child is Button)
                        {
                            toRemove.Add(child);
                        }
                    }
                    foreach (var c in toRemove)
                    {
                        pPanel.Controls.Remove(c);
                        c.Dispose();
                    }
                }
            }
        }

        private async void btnBookmarksList_Click(object sender, EventArgs e)
        {
            if (currentBook == null) return;
            var list = await FirebaseDatabaseService.GetLineBookmarksAsync(currentBook.Id);
            if (list.Count == 0)
            {
                MessageBox.Show("Chưa có dòng nào được đánh dấu bookmark trong truyện này.", "Bookmarks");
                return;
            }

            ContextMenuStrip menu = new ContextMenuStrip();
            foreach (var bm in list)
            {
                ToolStripMenuItem item = new ToolStripMenuItem($"Ch. {bm.ChapterNumber}: {bm.LineText}");
                item.Click += (sItem, eItem) =>
                {
                    targetScrollParagraphIndex = bm.ParagraphIndex;
                    int chIndex = chaptersList.FindIndex(ch => ch.ChapterNumber == bm.ChapterNumber);
                    if (chIndex != -1)
                    {
                        if (cbChapters.SelectedIndex == chIndex)
                        {
                            ScrollToParagraph(targetScrollParagraphIndex);
                        }
                        else
                        {
                            cbChapters.SelectedIndex = chIndex;
                        }
                    }
                };
                menu.Items.Add(item);
            }
            menu.Show(btnBookmarksList, new Point(0, btnBookmarksList.Height));
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
            
            foreach (var lbl in paragraphControls.Values)
            {
                lbl.Font = new Font("Segoe UI", newSize, FontStyle.Regular);
            }
            
            MessageBox.Show($"Đã thay đổi cỡ chữ đọc sang: {newSize}pt", "Cài đặt đọc", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
