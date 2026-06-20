namespace client_firebase
{
    partial class UC_Notification
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
            this.lblHeader = new System.Windows.Forms.Label();
            this.panelTabs = new System.Windows.Forms.Panel();
            this.btnTabRead = new System.Windows.Forms.Button();
            this.btnTabUnread = new System.Windows.Forms.Button();
            this.flpNotifications = new System.Windows.Forms.FlowLayoutPanel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnMarkAll = new System.Windows.Forms.Button();
            this.panelTabs.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(20, 15);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(134, 25);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "🔔 Thông báo";
            // 
            // panelTabs
            // 
            this.panelTabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTabs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.panelTabs.Controls.Add(this.btnTabRead);
            this.panelTabs.Controls.Add(this.btnTabUnread);
            this.panelTabs.Location = new System.Drawing.Point(20, 50);
            this.panelTabs.Name = "panelTabs";
            this.panelTabs.Padding = new System.Windows.Forms.Padding(4);
            this.panelTabs.Size = new System.Drawing.Size(760, 42);
            this.panelTabs.TabIndex = 1;
            // 
            // btnTabRead
            // 
            this.btnTabRead.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTabRead.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnTabRead.FlatAppearance.BorderSize = 0;
            this.btnTabRead.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabRead.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabRead.Location = new System.Drawing.Point(380, 4);
            this.btnTabRead.Name = "btnTabRead";
            this.btnTabRead.Size = new System.Drawing.Size(376, 34);
            this.btnTabRead.TabIndex = 1;
            this.btnTabRead.Text = "Đã đọc";
            this.btnTabRead.UseVisualStyleBackColor = true;
            // 
            // btnTabUnread
            // 
            this.btnTabUnread.BackColor = System.Drawing.Color.White;
            this.btnTabUnread.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTabUnread.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnTabUnread.FlatAppearance.BorderSize = 0;
            this.btnTabUnread.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabUnread.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabUnread.Location = new System.Drawing.Point(4, 4);
            this.btnTabUnread.Name = "btnTabUnread";
            this.btnTabUnread.Size = new System.Drawing.Size(376, 34);
            this.btnTabUnread.TabIndex = 0;
            this.btnTabUnread.Text = "Chưa đọc";
            this.btnTabUnread.UseVisualStyleBackColor = false;
            // 
            // flpNotifications
            // 
            this.flpNotifications.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpNotifications.AutoScroll = true;
            this.flpNotifications.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpNotifications.Location = new System.Drawing.Point(20, 105);
            this.flpNotifications.Name = "flpNotifications";
            this.flpNotifications.Size = new System.Drawing.Size(760, 410);
            this.flpNotifications.TabIndex = 2;
            this.flpNotifications.WrapContents = false;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.btnMarkAll);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 530);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(800, 70);
            this.panelBottom.TabIndex = 3;
            // 
            // btnMarkAll
            // 
            this.btnMarkAll.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnMarkAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(92)))), ((int)(((byte)(231)))));
            this.btnMarkAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMarkAll.FlatAppearance.BorderSize = 0;
            this.btnMarkAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMarkAll.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMarkAll.ForeColor = System.Drawing.Color.White;
            this.btnMarkAll.Location = new System.Drawing.Point(250, 15);
            this.btnMarkAll.Name = "btnMarkAll";
            this.btnMarkAll.Size = new System.Drawing.Size(300, 40);
            this.btnMarkAll.TabIndex = 0;
            this.btnMarkAll.Text = "Đánh dấu tất cả là đã đọc";
            this.btnMarkAll.UseVisualStyleBackColor = false;
            // 
            // UC_Notification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.flpNotifications);
            this.Controls.Add(this.panelTabs);
            this.Controls.Add(this.lblHeader);
            this.Name = "UC_Notification";
            this.Size = new System.Drawing.Size(800, 600);
            this.panelTabs.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Panel panelTabs;
        private System.Windows.Forms.Button btnTabRead;
        private System.Windows.Forms.Button btnTabUnread;
        private System.Windows.Forms.FlowLayoutPanel flpNotifications;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnMarkAll;
    }
}
