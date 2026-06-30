namespace client_firebase
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnProfile = new System.Windows.Forms.PictureBox();
            this.btnNotification = new System.Windows.Forms.PictureBox();
            this.panelNav = new System.Windows.Forms.Panel();
            this.btnChat = new System.Windows.Forms.PictureBox();
            this.btnUpload = new System.Windows.Forms.PictureBox();
            this.btnLibrary = new System.Windows.Forms.PictureBox();
            this.btnSearch = new System.Windows.Forms.PictureBox();
            this.btnHome = new System.Windows.Forms.PictureBox();
            this.lblLogo = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnProfile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNotification)).BeginInit();
            this.panelNav.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnChat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUpload)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLibrary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnHome)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(92)))), ((int)(((byte)(231)))));
            this.panelTop.Controls.Add(this.btnProfile);
            this.panelTop.Controls.Add(this.btnNotification);
            this.panelTop.Controls.Add(this.panelNav);
            this.panelTop.Controls.Add(this.lblLogo);
            this.panelTop.Controls.Add(this.picLogo);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(900, 60);
            this.panelTop.TabIndex = 1;
            // 
            // btnProfile
            // 
            this.btnProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProfile.BackColor = System.Drawing.Color.Transparent;
            this.btnProfile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProfile.Image = global::client_firebase.Properties.Resources.user;
            this.btnProfile.Location = new System.Drawing.Point(845, 15);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(32, 32);
            this.btnProfile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnProfile.TabIndex = 9;
            this.btnProfile.TabStop = false;
            // 
            // btnNotification
            // 
            this.btnNotification.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNotification.BackColor = System.Drawing.Color.Transparent;
            this.btnNotification.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNotification.Image = global::client_firebase.Properties.Resources.bell;
            this.btnNotification.Location = new System.Drawing.Point(795, 15);
            this.btnNotification.Name = "btnNotification";
            this.btnNotification.Size = new System.Drawing.Size(32, 32);
            this.btnNotification.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnNotification.TabIndex = 8;
            this.btnNotification.TabStop = false;
            // 
            // panelNav
            // 
            this.panelNav.BackColor = System.Drawing.Color.Transparent;
            this.panelNav.Controls.Add(this.btnChat);
            this.panelNav.Controls.Add(this.btnUpload);
            this.panelNav.Controls.Add(this.btnLibrary);
            this.panelNav.Controls.Add(this.btnSearch);
            this.panelNav.Controls.Add(this.btnHome);
            this.panelNav.Location = new System.Drawing.Point(280, 0);
            this.panelNav.Name = "panelNav";
            this.panelNav.Size = new System.Drawing.Size(340, 60);
            this.panelNav.TabIndex = 10;
            // 
            // btnChat
            // 
            this.btnChat.BackColor = System.Drawing.Color.Transparent;
            this.btnChat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnChat.Image = global::client_firebase.Properties.Resources.chat;
            this.btnChat.Location = new System.Drawing.Point(308, 14);
            this.btnChat.Name = "btnChat";
            this.btnChat.Size = new System.Drawing.Size(32, 32);
            this.btnChat.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnChat.TabIndex = 7;
            this.btnChat.TabStop = false;
            this.btnChat.Click += new System.EventHandler(this.btnChat_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.BackColor = System.Drawing.Color.Transparent;
            this.btnUpload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpload.Image = global::client_firebase.Properties.Resources.up_loading;
            this.btnUpload.Location = new System.Drawing.Point(231, 14);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(32, 32);
            this.btnUpload.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnUpload.TabIndex = 6;
            this.btnUpload.TabStop = false;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnLibrary
            // 
            this.btnLibrary.BackColor = System.Drawing.Color.Transparent;
            this.btnLibrary.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLibrary.Image = global::client_firebase.Properties.Resources.book__1_;
            this.btnLibrary.Location = new System.Drawing.Point(154, 14);
            this.btnLibrary.Name = "btnLibrary";
            this.btnLibrary.Size = new System.Drawing.Size(32, 32);
            this.btnLibrary.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnLibrary.TabIndex = 5;
            this.btnLibrary.TabStop = false;
            this.btnLibrary.Click += new System.EventHandler(this.btnLibrary_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.Image = global::client_firebase.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(77, 14);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(32, 32);
            this.btnSearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnSearch.TabIndex = 4;
            this.btnSearch.TabStop = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnHome
            // 
            this.btnHome.BackColor = System.Drawing.Color.Transparent;
            this.btnHome.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHome.Image = global::client_firebase.Properties.Resources.home;
            this.btnHome.Location = new System.Drawing.Point(0, 14);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(32, 32);
            this.btnHome.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnHome.TabIndex = 3;
            this.btnHome.TabStop = false;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // lblLogo
            // 
            this.lblLogo.AutoSize = true;
            this.lblLogo.BackColor = System.Drawing.Color.Transparent;
            this.lblLogo.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogo.ForeColor = System.Drawing.Color.White;
            this.lblLogo.Location = new System.Drawing.Point(55, 15);
            this.lblLogo.Name = "lblLogo";
            this.lblLogo.Size = new System.Drawing.Size(107, 30);
            this.lblLogo.TabIndex = 2;
            this.lblLogo.Text = "StoryHub";
            // 
            // picLogo
            // 
            this.picLogo.BackColor = System.Drawing.Color.Transparent;
            this.picLogo.Image = global::client_firebase.Properties.Resources.book;
            this.picLogo.Location = new System.Drawing.Point(14, 15);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(35, 32);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 2;
            this.picLogo.TabStop = false;
            // 
            // panelContent
            // 
            this.panelContent.AutoScroll = true;
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 60);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(900, 540);
            this.panelContent.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelTop);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "StoryHub - Ứng Dụng Đọc Truyện Online";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnProfile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNotification)).EndInit();
            this.panelNav.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnChat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUpload)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLibrary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnHome)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelNav;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label lblLogo;
        private System.Windows.Forms.PictureBox btnChat;
        private System.Windows.Forms.PictureBox btnUpload;
        private System.Windows.Forms.PictureBox btnLibrary;
        private System.Windows.Forms.PictureBox btnSearch;
        private System.Windows.Forms.PictureBox btnHome;
        private System.Windows.Forms.PictureBox btnNotification;
        private System.Windows.Forms.PictureBox btnProfile;
        private System.Windows.Forms.Panel panelContent;
    }
}