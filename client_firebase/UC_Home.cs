using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class UC_Home : UserControl
    {
        public UC_Home()
        {
            InitializeComponent();
            EnableDoubleBuffer(flowLayoutPanel1);
            EnableDoubleBuffer(flowLayoutPanel2);
            this.Load += (s, e) => LoadBooksAsync();
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
                flowLayoutPanel1.Controls.Clear();
                flowLayoutPanel2.Controls.Clear();

                if (books == null || books.Count == 0) return;

                // Sort books by UpdatedAt timestamp for "New Updates"
                var newUpdates = new List<BookModel>(books);
                newUpdates.Sort((x, y) => y.UpdatedAt.CompareTo(x.UpdatedAt));

                // Sort by composite trend score (views * rating)
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
                    // Use a clone or copy so MemoryStream disposal doesn't break the image drawing
                    using (var ms = new System.IO.MemoryStream(bytes))
                    {
                        card.pictureBox1.Image = Image.FromStream(ms);
                        // Cloning image prevents WinForms drawing errors when MS is closed
                        card.pictureBox1.Image = (Image)card.pictureBox1.Image.Clone();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error decoding cover image: " + ex.Message);
                }
            }

            // Click handlers to open book details
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
    }
}
