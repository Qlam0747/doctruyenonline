using System;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
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

        private Timer badgeTimer;
        private bool hasUnreadChats = false;
        private bool hasUnreadNotifications = false;

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

            // Initialize badge polling timer
            badgeTimer = new Timer();
            badgeTimer.Interval = 3000;
            badgeTimer.Tick += BadgeTimer_Tick;
            badgeTimer.Start();

            // Wire badge painters
            btnChat.Paint += BtnChat_Paint;
            btnNotification.Paint += BtnNotification_Paint;

            // Khởi tạo menu cho avatar user
            InitializeUserMenu();

            // Căn giữa thanh điều hướng panelNav
            panelNav.Left = (panelTop.Width - panelNav.Width) / 2;
            this.panelTop.SizeChanged += (s, e) => {
                panelNav.Left = (panelTop.Width - panelNav.Width) / 2;
            };
        }

        private Panel panelUserMenu;

        private void InitializeUserMenu()
        {
            panelUserMenu = new Panel();
            panelUserMenu.Size = new Size(200, 92); // 45px per button + 1px divider + 1px border
            panelUserMenu.BorderStyle = BorderStyle.FixedSingle;
            panelUserMenu.BackColor = Color.White;
            panelUserMenu.Visible = false;
            panelUserMenu.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Căn vị trí ngay bên dưới hình đại diện (btnProfile)
            panelUserMenu.Location = new Point(btnProfile.Left + btnProfile.Width - panelUserMenu.Width, panelTop.Height);

            var btnEditProfile = new Button
            {
                Text = "✏️  Chỉnh sửa thông tin",
                Height = 45,
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.75F, FontStyle.Regular),
                ForeColor = Color.FromArgb(45, 52, 54),
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };
            btnEditProfile.FlatAppearance.BorderSize = 0;
            btnEditProfile.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 238, 255); // Màu tím nhạt hover
            btnEditProfile.Click += btnEditProfile_Click;

            var divider = new Panel
            {
                Height = 1,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(230, 230, 230)
            };

            var btnLogout = new Button
            {
                Text = "🚪  Đăng xuất",
                Height = 45,
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                ForeColor = Color.FromArgb(235, 77, 75), // Đỏ hiện đại
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 240, 240); // Đỏ nhạt hover
            btnLogout.Click += btnLogout_Click;

            panelUserMenu.Controls.Add(btnLogout);
            panelUserMenu.Controls.Add(divider);
            panelUserMenu.Controls.Add(btnEditProfile);

            // Thêm vào controls của MainForm
            this.Controls.Add(panelUserMenu);

            // Đưa lên trên cùng để nổi trên các control khác
            panelUserMenu.BringToFront();

            // Click vào nội dung bên ngoài thì ẩn menu
            panelContent.Click += (s, e) => { panelUserMenu.Visible = false; };

            // Gán sự kiện click cho avatar
            btnProfile.Click += btnProfile_Click;
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            panelUserMenu.Visible = !panelUserMenu.Visible;
            if (panelUserMenu.Visible)
            {
                panelUserMenu.BringToFront();
            }
        }

        private void btnEditProfile_Click(object sender, EventArgs e)
        {
            panelUserMenu.Visible = false;

            // Mở dialog chỉnh sửa thông tin cá nhân
            using (var editForm = new FormEditProfile())
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Cập nhật thông tin cá nhân thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            panelUserMenu.Visible = false;
            var confirm = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                AuthSession.Clear();
                // Hiển thị lại màn hình đăng nhập
                dangnhap loginForm = new dangnhap();
                loginForm.Show();
                this.Hide();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            ShowControl(ucHome);
            await UpdateProfileAvatar();
        }

        public async Task UpdateProfileAvatar()
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return;
            try
            {
                var profile = await FirebaseDatabaseService.GetCurrentUserProfileAsync();
                if (profile != null && !string.IsNullOrEmpty(profile.Avatar))
                {
                    byte[] bytes = Convert.FromBase64String(profile.Avatar);
                    using (var ms = new System.IO.MemoryStream(bytes))
                    {
                        btnProfile.Image = Image.FromStream(ms);
                        btnProfile.Image = (Image)btnProfile.Image.Clone();
                    }
                }
                else
                {
                    btnProfile.Image = global::client_firebase.Properties.Resources.user;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error updating profile avatar: " + ex.Message);
            }
        }

        private async void BadgeTimer_Tick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return;
            try
            {
                bool prevChats = hasUnreadChats;
                bool prevNotis = hasUnreadNotifications;

                hasUnreadChats = await FirebaseDatabaseService.HasAnyUnreadChatsAsync();
                hasUnreadNotifications = await FirebaseDatabaseService.HasAnyUnreadNotificationsAsync();

                if (hasUnreadChats != prevChats)
                {
                    btnChat.Invalidate();
                }
                if (hasUnreadNotifications != prevNotis)
                {
                    btnNotification.Invalidate();
                }
            }
            catch {}
        }

        private void BtnChat_Paint(object sender, PaintEventArgs e)
        {
            if (hasUnreadChats)
            {
                int dotRadius = 4;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (Brush brush = new SolidBrush(Color.Red))
                {
                    e.Graphics.FillEllipse(brush, btnChat.Width - (dotRadius * 2) - 4, 4, dotRadius * 2, dotRadius * 2);
                }
            }
        }

        private void BtnNotification_Paint(object sender, PaintEventArgs e)
        {
            if (hasUnreadNotifications)
            {
                int dotRadius = 4;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (Brush brush = new SolidBrush(Color.Red))
                {
                    e.Graphics.FillEllipse(brush, btnNotification.Width - (dotRadius * 2) - 4, 4, dotRadius * 2, dotRadius * 2);
                }
            }
        }

        public void ShowControl(UserControl uc)
        {
            if (panelUserMenu != null) panelUserMenu.Visible = false;
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