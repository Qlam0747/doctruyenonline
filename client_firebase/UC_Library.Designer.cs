namespace client_firebase
{
    partial class UC_Library
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.panelProfile = new System.Windows.Forms.Panel();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.lblFollowing = new System.Windows.Forms.Label();
            this.lblFollowers = new System.Windows.Forms.Label();
            this.lblPostedCount = new System.Windows.Forms.Label();
            this.lblBio = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.pbAvatar = new System.Windows.Forms.PictureBox();
            this.panelTabs = new System.Windows.Forms.Panel();
            this.btnTabFavorites = new System.Windows.Forms.Button();
            this.btnTabHistory = new System.Windows.Forms.Button();
            this.btnTabBookmark = new System.Windows.Forms.Button();
            this.lblBodyHeader = new System.Windows.Forms.Label();
            this.flpBooks = new System.Windows.Forms.FlowLayoutPanel();
            this.panelProfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAvatar)).BeginInit();
            this.panelTabs.SuspendLayout();
            this.SuspendLayout();
            
            
            
            this.panelProfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelProfile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(195)))), ((int)(((byte)(255)))));
            this.panelProfile.Controls.Add(this.btnSettings);
            this.panelProfile.Controls.Add(this.btnEdit);
            this.panelProfile.Controls.Add(this.lblFollowing);
            this.panelProfile.Controls.Add(this.lblFollowers);
            this.panelProfile.Controls.Add(this.lblPostedCount);
            this.panelProfile.Controls.Add(this.lblBio);
            this.panelProfile.Controls.Add(this.lblUsername);
            this.panelProfile.Controls.Add(this.pbAvatar);
            this.panelProfile.Location = new System.Drawing.Point(15, 15);
            this.panelProfile.Name = "panelProfile";
            this.panelProfile.Size = new System.Drawing.Size(770, 140);
            this.panelProfile.TabIndex = 0;
            
            
            
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettings.Location = new System.Drawing.Point(720, 15);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(35, 35);
            this.btnSettings.TabIndex = 7;
            this.btnSettings.Text = "⚙";
            this.btnSettings.UseVisualStyleBackColor = true;
            
            
            
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEdit.FlatAppearance.BorderSize = 0;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Location = new System.Drawing.Point(679, 15);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(35, 35);
            this.btnEdit.TabIndex = 6;
            this.btnEdit.Text = "📝";
            this.btnEdit.UseVisualStyleBackColor = true;
            
            
            
            this.lblFollowing.AutoSize = true;
            this.lblFollowing.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFollowing.Location = new System.Drawing.Point(380, 95);
            this.lblFollowing.Name = "lblFollowing";
            this.lblFollowing.Size = new System.Drawing.Size(95, 15);
            this.lblFollowing.TabIndex = 5;
            this.lblFollowing.Text = "100 Đang theo dõi";
            
            
            
            this.lblFollowers.AutoSize = true;
            this.lblFollowers.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFollowers.Location = new System.Drawing.Point(260, 95);
            this.lblFollowers.Name = "lblFollowers";
            this.lblFollowers.Size = new System.Drawing.Size(100, 15);
            this.lblFollowers.TabIndex = 4;
            this.lblFollowers.Text = "100 Người theo dõi";
            
            
            
            this.lblPostedCount.AutoSize = true;
            this.lblPostedCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPostedCount.Location = new System.Drawing.Point(145, 95);
            this.lblPostedCount.Name = "lblPostedCount";
            this.lblPostedCount.Size = new System.Drawing.Size(95, 15);
            this.lblPostedCount.TabIndex = 3;
            this.lblPostedCount.Text = "10 Truyện đã đăng";
            
            
            
            this.lblBio.AutoSize = true;
            this.lblBio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lblBio.Location = new System.Drawing.Point(145, 55);
            this.lblBio.Name = "lblBio";
            this.lblBio.Size = new System.Drawing.Size(188, 17);
            this.lblBio.TabIndex = 2;
            this.lblBio.Text = "Yêu thích đọc và sáng tác truyện";
            
            
            
            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsername.Location = new System.Drawing.Point(142, 20);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(135, 25);
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Text = "Nguyễn Văn A";
            
            
            
            this.pbAvatar.Location = new System.Drawing.Point(25, 20);
            this.pbAvatar.Name = "pbAvatar";
            this.pbAvatar.Size = new System.Drawing.Size(100, 100);
            this.pbAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbAvatar.TabIndex = 0;
            this.pbAvatar.TabStop = false;
            
            
            
            this.panelTabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTabs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.panelTabs.Controls.Add(this.btnTabFavorites);
            this.panelTabs.Controls.Add(this.btnTabHistory);
            this.panelTabs.Controls.Add(this.btnTabBookmark);
            this.panelTabs.Location = new System.Drawing.Point(15, 170);
            this.panelTabs.Name = "panelTabs";
            this.panelTabs.Padding = new System.Windows.Forms.Padding(4);
            this.panelTabs.Size = new System.Drawing.Size(770, 42);
            this.panelTabs.TabIndex = 1;
            
            
            
            this.btnTabFavorites.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTabFavorites.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnTabFavorites.FlatAppearance.BorderSize = 0;
            this.btnTabFavorites.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabFavorites.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabFavorites.Location = new System.Drawing.Point(516, 4);
            this.btnTabFavorites.Name = "btnTabFavorites";
            this.btnTabFavorites.Size = new System.Drawing.Size(250, 34);
            this.btnTabFavorites.TabIndex = 2;
            this.btnTabFavorites.Text = "♡  Yêu thích";
            this.btnTabFavorites.UseVisualStyleBackColor = true;
            
            
            
            this.btnTabHistory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTabHistory.FlatAppearance.BorderSize = 0;
            this.btnTabHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabHistory.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabHistory.Location = new System.Drawing.Point(260, 4);
            this.btnTabHistory.Name = "btnTabHistory";
            this.btnTabHistory.Size = new System.Drawing.Size(250, 34);
            this.btnTabHistory.TabIndex = 1;
            this.btnTabHistory.Text = "🕒  Lịch sử";
            this.btnTabHistory.UseVisualStyleBackColor = true;
            
            
            
            this.btnTabBookmark.BackColor = System.Drawing.Color.White;
            this.btnTabBookmark.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTabBookmark.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnTabBookmark.FlatAppearance.BorderSize = 0;
            this.btnTabBookmark.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabBookmark.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabBookmark.Location = new System.Drawing.Point(4, 4);
            this.btnTabBookmark.Name = "btnTabBookmark";
            this.btnTabBookmark.Size = new System.Drawing.Size(250, 34);
            this.btnTabBookmark.TabIndex = 0;
            this.btnTabBookmark.Text = "🔖  Bookmark";
            this.btnTabBookmark.UseVisualStyleBackColor = false;
            
            
            
            this.lblBodyHeader.AutoSize = true;
            this.lblBodyHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBodyHeader.Location = new System.Drawing.Point(15, 230);
            this.lblBodyHeader.Name = "lblBodyHeader";
            this.lblBodyHeader.Size = new System.Drawing.Size(161, 21);
            this.lblBodyHeader.TabIndex = 2;
            this.lblBodyHeader.Text = "Truyện đã bookmark";
            
            
            
            this.flpBooks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpBooks.AutoScroll = true;
            this.flpBooks.Location = new System.Drawing.Point(15, 260);
            this.flpBooks.Name = "flpBooks";
            this.flpBooks.Size = new System.Drawing.Size(770, 325);
            this.flpBooks.TabIndex = 3;
            
            
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.flpBooks);
            this.Controls.Add(this.lblBodyHeader);
            this.Controls.Add(this.panelTabs);
            this.Controls.Add(this.panelProfile);
            this.Name = "UC_Library";
            this.Size = new System.Drawing.Size(800, 600);
            this.panelProfile.ResumeLayout(false);
            this.panelProfile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAvatar)).EndInit();
            this.panelTabs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelProfile;
        private System.Windows.Forms.PictureBox pbAvatar;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblBio;
        private System.Windows.Forms.Label lblPostedCount;
        private System.Windows.Forms.Label lblFollowers;
        private System.Windows.Forms.Label lblFollowing;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Panel panelTabs;
        private System.Windows.Forms.Button btnTabBookmark;
        private System.Windows.Forms.Button btnTabHistory;
        private System.Windows.Forms.Button btnTabFavorites;
        private System.Windows.Forms.Label lblBodyHeader;
        private System.Windows.Forms.FlowLayoutPanel flpBooks;
    }
}
