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

        public MainForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            
            // Khởi tạo các UserControl
            ucHome = new UC_Home();
            ucSearch = new UC_Search();
            ucLibrary = new UC_Library();
            
            // Thêm vào panelContent và thiết lập Dock
            ucHome.Dock = DockStyle.Fill;
            ucSearch.Dock = DockStyle.Fill;
            ucLibrary.Dock = DockStyle.Fill;
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

        private void btnHome_Click(object sender, EventArgs e)
        {
            ShowControl(ucHome);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ShowControl(ucSearch);
        }

        private void btnLibrary_Click(object sender, EventArgs e)
        {
            ShowControl(ucLibrary);
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            // Tương lai sẽ thêm UC_Upload
        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            // Tương lai sẽ thêm UC_Chat
        }
    }
}