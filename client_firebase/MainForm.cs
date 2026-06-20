using System;
using System.Reflection;
using System.Windows.Forms;

namespace client_firebase
{
    public partial class MainForm : Form
    {
        private UC_Home ucHome;
        private UC_Search ucSearch;
        private UC_Library ucLibrary;
        private UC_Upload ucUpload;
        private UC_Chat ucChat;
        private UC_BookDetail ucBookDetail;
        private UC_Reading ucReading;
        private UC_Notification ucNotification;

        public MainForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            
            // Khởi tạo các UserControl
            ucHome = new UC_Home();
            ucSearch = new UC_Search();
            ucLibrary = new UC_Library();
            ucUpload = new UC_Upload();
            ucChat = new UC_Chat();
            ucBookDetail = new UC_BookDetail();
            ucReading = new UC_Reading();
            ucNotification = new UC_Notification();
            
            // Thêm vào panelContent và thiết lập Dock
            ucHome.Dock = DockStyle.Fill;
            ucSearch.Dock = DockStyle.Fill;
            ucLibrary.Dock = DockStyle.Fill;
            ucUpload.Dock = DockStyle.Fill;
            ucChat.Dock = DockStyle.Fill;
            ucBookDetail.Dock = DockStyle.Fill;
            ucReading.Dock = DockStyle.Fill;
            ucNotification.Dock = DockStyle.Fill;

            // Wire up notification click programmatically
            btnNotification.Click += btnNotification_Click;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowControl(ucHome);
        }

        public void ShowControl(UserControl uc)
        {
            panelContent.Controls.Clear();
            panelContent.Controls.Add(uc);
            uc.BringToFront();
        }

        public void GoToHome()
        {
            ShowControl(ucHome);
            // Refresh home data
            ucHome.LoadBooksAsync();
        }

        public void ShowBookDetail(BookModel book)
        {
            ucBookDetail.SetBook(book);
            ShowControl(ucBookDetail);
        }

        public void ShowReadingScreen(BookModel book, string chapterNum)
        {
            ucReading.SetBook(book, chapterNum);
            ShowControl(ucReading);
        }

        public void ShowChatWithUser(string userId, string username)
        {
            ShowControl(ucChat);
            ucChat.SelectUserById(userId);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            GoToHome();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ShowControl(ucSearch);
        }

        private async void btnLibrary_Click(object sender, EventArgs e)
        {
            await ucLibrary.RefreshLibraryData();
            ShowControl(ucLibrary);
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            ShowControl(ucUpload);
        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            ShowControl(ucChat);
        }

        private async void btnNotification_Click(object sender, EventArgs e)
        {
            await ucNotification.RefreshNotifications();
            ShowControl(ucNotification);
        }
    }
}