using System;
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
        }

        private void EnableDoubleBuffer(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }
    }
}
