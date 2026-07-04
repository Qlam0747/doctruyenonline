using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client_firebase
{
    public class FormFollows : Form
    {
        private TabControl tabControl;
        private TabPage tabPageFollowers;
        private TabPage tabPageFollowing;
        private FlowLayoutPanel flpFollowers;
        private FlowLayoutPanel flpFollowing;
        
        private List<string> myFollowingIds = new List<string>();

        public FormFollows(int initialTab = 0)
        {
            InitializeForm();
            tabControl.SelectedIndex = initialTab;
            this.Load += FormFollows_Load;
        }

        private void InitializeForm()
        {
            this.Text = "Người theo dõi & Đang theo dõi";
            this.Size = new Size(480, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            tabPageFollowers = new TabPage("Người theo dõi")
            {
                BackColor = Color.White
            };

            tabPageFollowing = new TabPage("Đang theo dõi")
            {
                BackColor = Color.White
            };

            flpFollowers = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(10)
            };

            flpFollowing = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(10)
            };

            tabPageFollowers.Controls.Add(flpFollowers);
            tabPageFollowing.Controls.Add(flpFollowing);
            tabControl.TabPages.Add(tabPageFollowers);
            tabControl.TabPages.Add(tabPageFollowing);

            this.Controls.Add(tabControl);
        }

        private async void FormFollows_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            flpFollowers.Controls.Clear();
            flpFollowing.Controls.Clear();

            Label lblLoading1 = new Label { Text = "Đang tải dữ liệu...", AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Italic) };
            Label lblLoading2 = new Label { Text = "Đang tải dữ liệu...", AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Italic) };
            flpFollowers.Controls.Add(lblLoading1);
            flpFollowing.Controls.Add(lblLoading2);

            try
            {
                string myId = AuthSession.FirebaseLocalId;
                
                // Fetch followings first so we can check relationship in followers tab
                myFollowingIds = await FirebaseDatabaseService.GetFollowingIdsAsync(myId);
                var followerIds = await FirebaseDatabaseService.GetFollowerIdsAsync(myId);

                flpFollowers.Controls.Clear();
                flpFollowing.Controls.Clear();

                // 1. Render Followers
                if (followerIds.Count == 0)
                {
                    flpFollowers.Controls.Add(new Label
                    {
                        Text = "Chưa có người theo dõi nào.",
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        Margin = new Padding(10)
                    });
                }
                else
                {
                    foreach (var id in followerIds)
                    {
                        var profile = await FirebaseDatabaseService.GetUserProfileAsync(id);
                        if (profile != null)
                        {
                            var row = CreateUserRow(profile, isFollowerTab: true);
                            flpFollowers.Controls.Add(row);
                        }
                    }
                }

                // 2. Render Following
                if (myFollowingIds.Count == 0)
                {
                    flpFollowing.Controls.Add(new Label
                    {
                        Text = "Bạn chưa theo dõi ai.",
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        Margin = new Padding(10)
                    });
                }
                else
                {
                    foreach (var id in myFollowingIds)
                    {
                        var profile = await FirebaseDatabaseService.GetUserProfileAsync(id);
                        if (profile != null)
                        {
                            var row = CreateUserRow(profile, isFollowerTab: false);
                            flpFollowing.Controls.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thông tin theo dõi: " + ex.Message, "Lỗi");
            }
        }

        private Panel CreateUserRow(UserModel user, bool isFollowerTab)
        {
            Panel panel = new Panel
            {
                Width = 430,
                Height = 60,
                BackColor = Color.FromArgb(250, 248, 255),
                Margin = new Padding(0, 5, 0, 5)
            };

            // Avatar
            PictureBox pbAvatar = new PictureBox
            {
                Size = new Size(40, 40),
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            DrawAvatar(pbAvatar, user.Username, user.Avatar);
            panel.Controls.Add(pbAvatar);

            // Username
            Label lblUser = new Label
            {
                Text = user.Username,
                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                Location = new Point(60, 20),
                Size = new Size(180, 20),
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panel.Controls.Add(lblUser);

            // Message Button
            Button btnMsg = new Button
            {
                Text = "💬 Nhắn",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Size = new Size(70, 30),
                Location = new Point(350, 15),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 92, 231),
                ForeColor = Color.White
            };
            btnMsg.FlatAppearance.BorderSize = 0;
            btnMsg.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.OK; // Signals calling form to handle redirect
                this.Close();

                // Open conversation in MainForm
                var mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
                if (mainForm != null)
                {
                    mainForm.ShowChatWithUser(user.LocalId, user.Username);
                }
            };
            panel.Controls.Add(btnMsg);

            // Follow Action Button
            Button btnFollowAction = new Button
            {
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Size = new Size(95, 30),
                Location = new Point(245, 15),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };

            if (isFollowerTab)
            {
                bool isFollowingBack = myFollowingIds.Contains(user.LocalId);
                
                Action updateFollowBackUI = () =>
                {
                    if (isFollowingBack)
                    {
                        btnFollowAction.Text = "Đang theo dõi";
                        btnFollowAction.BackColor = Color.Transparent;
                        btnFollowAction.ForeColor = Color.Gray;
                        btnFollowAction.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
                        btnFollowAction.FlatAppearance.BorderSize = 1;
                    }
                    else
                    {
                        btnFollowAction.Text = "Theo dõi lại";
                        btnFollowAction.BackColor = Color.FromArgb(168, 122, 255);
                        btnFollowAction.ForeColor = Color.White;
                        btnFollowAction.FlatAppearance.BorderSize = 0;
                    }
                };

                updateFollowBackUI();

                btnFollowAction.Click += async (s, e) =>
                {
                    btnFollowAction.Enabled = false;
                    if (isFollowingBack)
                    {
                        // Unfollow
                        bool success = await FirebaseDatabaseService.UnfollowAuthorAsync(user.LocalId);
                        if (success)
                        {
                            isFollowingBack = false;
                            myFollowingIds.Remove(user.LocalId);
                            updateFollowBackUI();
                        }
                    }
                    else
                    {
                        // Follow
                        bool success = await FirebaseDatabaseService.FollowAuthorAsync(user.LocalId);
                        if (success)
                        {
                            isFollowingBack = true;
                            myFollowingIds.Add(user.LocalId);
                            updateFollowBackUI();
                        }
                    }
                    btnFollowAction.Enabled = true;
                };
            }
            else
            {
                // Following tab: "Hủy theo dõi" button
                bool isFollowing = true;

                Action updateFollowingUI = () =>
                {
                    if (isFollowing)
                    {
                        btnFollowAction.Text = "Hủy theo dõi";
                        btnFollowAction.BackColor = Color.Transparent;
                        btnFollowAction.ForeColor = Color.Red;
                        btnFollowAction.FlatAppearance.BorderColor = Color.Red;
                        btnFollowAction.FlatAppearance.BorderSize = 1;
                    }
                    else
                    {
                        btnFollowAction.Text = "Theo dõi";
                        btnFollowAction.BackColor = Color.FromArgb(168, 122, 255);
                        btnFollowAction.ForeColor = Color.White;
                        btnFollowAction.FlatAppearance.BorderSize = 0;
                    }
                };

                updateFollowingUI();

                btnFollowAction.Click += async (s, e) =>
                {
                    btnFollowAction.Enabled = false;
                    if (isFollowing)
                    {
                        // Unfollow
                        bool success = await FirebaseDatabaseService.UnfollowAuthorAsync(user.LocalId);
                        if (success)
                        {
                            isFollowing = false;
                            updateFollowingUI();
                        }
                    }
                    else
                    {
                        // Re-follow
                        bool success = await FirebaseDatabaseService.FollowAuthorAsync(user.LocalId);
                        if (success)
                        {
                            isFollowing = true;
                            updateFollowingUI();
                        }
                    }
                    btnFollowAction.Enabled = true;
                };
            }

            panel.Controls.Add(btnFollowAction);

            return panel;
        }

        private void DrawAvatar(PictureBox pb, string name, string avatarBase64 = null)
        {
            pb.Paint -= pb_PaintAvatar;
            if (!string.IsNullOrEmpty(avatarBase64))
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(avatarBase64);
                    using (var ms = new MemoryStream(bytes))
                    {
                        pb.Image = Image.FromStream(ms);
                        pb.Image = (Image)pb.Image.Clone();
                    }
                    return; // Bypass dynamic Paint avatar
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error parsing user avatar: " + ex.Message);
                }
            }

            pb.Image = null;
            pb.Tag = name;
            pb.Paint += pb_PaintAvatar;
            pb.Invalidate();
        }

        private void pb_PaintAvatar(object sender, PaintEventArgs pe)
        {
            if (sender is PictureBox pb && pb.Tag != null)
            {
                string name = pb.Tag.ToString();
                pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                int hash = name.GetHashCode();
                Color bg = Color.FromArgb(100 + Math.Abs(hash % 100), 100 + Math.Abs((hash / 100) % 100), 220);
                using (Brush brush = new SolidBrush(bg))
                {
                    pe.Graphics.FillEllipse(brush, 0, 0, pb.Width - 1, pb.Height - 1);
                }

                string letter = string.IsNullOrEmpty(name) ? "?" : name.Substring(0, 1).ToUpper();
                using (Font f = new Font("Segoe UI", pb.Width > 25 ? 12 : 8, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    SizeF size = pe.Graphics.MeasureString(letter, f);
                    pe.Graphics.DrawString(letter, f, textBrush, (pb.Width - size.Width) / 2, (pb.Height - size.Height) / 2);
                }
            }
        }
    }
}
