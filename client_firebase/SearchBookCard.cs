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

        
        
        
        public SearchBookCard()
        {
            InitializeComponent();
            SetupClickEvents(); 
        }

        
        
        
        public SearchBookCard(BookModel book) : this() 
        {
            _book = book;
            BindData();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(160, 240);
            this.BackColor = Color.White;
            this.Cursor = Cursors.Hand;
            this.Padding = new Padding(1); 

            
            picCover = new PictureBox
            {
                Location = new Point(8, 8),
                Size = new Size(144, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(245, 245, 245),
            };

            
            lblTitle = new Label
            {
                Location = new Point(8, 162),
                Size = new Size(144, 20),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Tên sách mẫu" 
            };

            
            lblAuthor = new Label
            {
                Location = new Point(8, 182),
                Size = new Size(144, 15),
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Tác giả"
            };

            
            picBookIcon = new PictureBox
            {
                Location = new Point(8, 204),
                Size = new Size(16, 16),
                SizeMode = PictureBoxSizeMode.Zoom,
                
                Image = global::client_firebase.Properties.Resources.book__1_,
            };

            
            lblChapters = new Label
            {
                Location = new Point(28, 203),
                Size = new Size(40, 18),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(108, 92, 231),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "0"
            };

            
            lblStatus = new Label
            {
                Location = new Point(72, 203),
                Size = new Size(80, 18),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113), 
                TextAlign = ContentAlignment.MiddleRight,
                Text = "Hoàn thành"
            };

            this.Controls.Add(picCover);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblAuthor);
            this.Controls.Add(picBookIcon);
            this.Controls.Add(lblChapters);
            this.Controls.Add(lblStatus);
        }

        
        
        
        private void SetupClickEvents()
        {
            Action clickAction = () =>
            {
                
                if (this.DesignMode) return;

                
                if (_book != null && this.ParentForm is MainForm mf)
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

            
            string displayStatus = _book.Status;
            if (string.IsNullOrEmpty(displayStatus))
            {
                if (chapterCount == 0) displayStatus = "Đang soạn";
                else if (chapterCount < 5) displayStatus = "Đang ra";
                else displayStatus = "Hoàn thành";
            }

            if (displayStatus == "Đã hoàn thành" || displayStatus == "Hoàn thành")
            {
                lblStatus.Text = "Hoàn thành";
                lblStatus.ForeColor = Color.FromArgb(46, 204, 113); 
            }
            else if (displayStatus == "Đang tiến hành" || displayStatus == "Đang ra")
            {
                lblStatus.Text = "Đang ra";
                lblStatus.ForeColor = Color.FromArgb(241, 196, 15); 
            }
            else if (displayStatus == "Đang soạn")
            {
                lblStatus.Text = "Đang soạn";
                lblStatus.ForeColor = Color.Gray;
            }
            else
            {
                lblStatus.Text = displayStatus;
                lblStatus.ForeColor = Color.FromArgb(108, 92, 231); 
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

            
            using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }
    }
}