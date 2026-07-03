using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class UC_Home : UserControl
    {
        public UC_Home()
        {
            InitializeComponent();

            
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            flowLayoutPanel2.AutoSize = true;
            flowLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            EnableDoubleBuffer(flowLayoutPanel1);
            EnableDoubleBuffer(flowLayoutPanel2);
            this.Load += (s, e) => LoadBooksAsync();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            if (label2 == null || flowLayoutPanel1 == null || label3 == null || flowLayoutPanel2 == null)
                return;

            int scrollX = this.AutoScrollPosition.X;
            int scrollY = this.AutoScrollPosition.Y;

            
            int contentWidth = Math.Max(100, this.ClientSize.Width - 40);

            
            int currentY = 20;

            
            label2.Location = new Point(20 + scrollX, currentY + scrollY);
            currentY += label2.Height + 10;

            
            flowLayoutPanel1.Width = contentWidth;
            flowLayoutPanel1.Location = new Point(20 + scrollX, currentY + scrollY);
            currentY += flowLayoutPanel1.Height + 20;

            
            label3.Location = new Point(20 + scrollX, currentY + scrollY);
            currentY += label3.Height + 10;

            
            flowLayoutPanel2.Width = contentWidth;
            flowLayoutPanel2.Location = new Point(20 + scrollX, currentY + scrollY);
        }

        private void EnableDoubleBuffer(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        public async void LoadBooksAsync()
        {
            try
            {
                var books = await FirebaseDatabaseService.GetAllBooksAsync();
                var users = await FirebaseDatabaseService.GetAllUsersAsync();
                
                flowLayoutPanel1.Controls.Clear();
                flowLayoutPanel2.Controls.Clear();

                if (books == null || books.Count == 0) return;

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

                
                var newUpdates = new List<BookModel>(books);
                newUpdates.Sort((x, y) => y.UpdatedAt.CompareTo(x.UpdatedAt));

                
                var trends = new List<BookModel>(books);
                trends.Sort((x, y) => {
                    double scoreX = x.Views * x.Rating;
                    double scoreY = y.Views * y.Rating;
                    return scoreY.CompareTo(scoreX);
                });

                foreach (var b in newUpdates)
                {
                    BookCard card = CreateBookCard(b);
                    flowLayoutPanel1.Controls.Add(card);
                }

                foreach (var b in trends)
                {
                    BookCard card = CreateBookCard(b);
                    flowLayoutPanel2.Controls.Add(card);
                }
                this.PerformLayout();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading books: " + ex.Message);
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
                    
                    using (var ms = new System.IO.MemoryStream(bytes))
                    {
                        card.pictureBox1.Image = Image.FromStream(ms);
                        
                        card.pictureBox1.Image = (Image)card.pictureBox1.Image.Clone();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error decoding cover image: " + ex.Message);
                }
            }

            
            Action clickAction = () =>
            {
                if (this.ParentForm is MainForm mf)
                {
                    mf.ShowBookDetail(b);
                }
            };

            card.Click += (s, e) => clickAction();
            card.pictureBox1.Click += (s, e) => clickAction();
            card.lblTitle.Click += (s, e) => clickAction();
            card.lblAuthor.Click += (s, e) => clickAction();
            card.lblRating.Click += (s, e) => clickAction();
            card.lblViews.Click += (s, e) => clickAction();

            return card;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
