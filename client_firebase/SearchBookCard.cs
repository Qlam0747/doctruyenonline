using System;
using System.Drawing;
using System.Windows.Forms;

namespace client_firebase
{
    public class SearchBookCard : UserControl
    {
        public PictureBox picCover;
        public Label lblTitle;
        public Label lblAuthor;
        public Label lblChapters;
        public Label lblStatus;
        public PictureBox picBookIcon;

        private BookModel _book;

        public SearchBookCard(BookModel book)
        {
            _book = book;
            InitializeComponent();
            BindData();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(160, 240);
            this.BackColor = Color.White;
            this.Cursor = Cursors.Hand;
            this.Padding = new Padding(1); // Border padding

            // Cover Image
            picCover = new PictureBox
            {
                Location = new Point(8, 8),
                Size = new Size(144, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(245, 245, 245),
            };

            // Title
            lblTitle = new Label
            {
                Location = new Point(8, 162),
                Size = new Size(144, 20),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Author
            lblAuthor = new Label
            {
                Location = new Point(8, 182),
                Size = new Size(144, 15),
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Book Icon
            picBookIcon = new PictureBox
            {
                Location = new Point(8, 204),
                Size = new Size(16, 16),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = global::client_firebase.Properties.Resources.book__1_,
            };

            // Chapters Count Label
            lblChapters = new Label
            {
                Location = new Point(28, 203),
                Size = new Size(40, 18),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(108, 92, 231),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Status Label
            lblStatus = new Label
            {
                Location = new Point(72, 203),
                Size = new Size(80, 18),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113), // Green status
                TextAlign = ContentAlignment.MiddleRight,
                Text = "Hoàn thành"
            };

            this.Controls.Add(picCover);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblAuthor);
            this.Controls.Add(picBookIcon);
            this.Controls.Add(lblChapters);
            this.Controls.Add(lblStatus);

            // Wire up clicks to navigate to book details
            Action clickAction = () =>
            {
                if (this.ParentForm is MainForm mf)
                {
                    mf.ShowBookDetail(_book);
                }
            };

            this.Click += (s, e) => clickAction();
            foreach (Control ctrl in this.Controls)
            {
                ctrl.Click += (s, e) => clickAction();
            }
        }

        private void BindData()
        {
            if (_book == null) return;

            lblTitle.Text = _book.Title;
            lblAuthor.Text = _book.AuthorName ?? "Ẩn danh";

            int chapterCount = _book.Chapters != null ? _book.Chapters.Count : 0;
            lblChapters.Text = chapterCount.ToString();

            // Dynamic status based on chapter count (similar to complete vs ongoing)
            if (chapterCount == 0)
            {
                lblStatus.Text = "Đang soạn";
                lblStatus.ForeColor = Color.Gray;
            }
            else if (chapterCount < 5)
            {
                lblStatus.Text = "Đang ra";
                lblStatus.ForeColor = Color.FromArgb(241, 196, 15); // Yellow/Orange
            }
            else
            {
                lblStatus.Text = "Hoàn thành";
                lblStatus.ForeColor = Color.FromArgb(46, 204, 113); // Green
            }

            if (!string.IsNullOrEmpty(_book.CoverBase64))
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(_book.CoverBase64);
                    using (var ms = new System.IO.MemoryStream(bytes))
                    {
                        var img = Image.FromStream(ms);
                        picCover.Image = (Image)img.Clone();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error decoding search card cover: " + ex.Message);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Draw border
            using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }
    }
}
