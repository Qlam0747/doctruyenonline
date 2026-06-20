namespace client_firebase
{
    partial class UC_Chat
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
            this.panelLeft = new System.Windows.Forms.Panel();
            this.flpContacts = new System.Windows.Forms.FlowLayoutPanel();
            this.txtSearchContacts = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelRight = new System.Windows.Forms.Panel();
            this.flpMessages = new System.Windows.Forms.FlowLayoutPanel();
            this.panelInput = new System.Windows.Forms.Panel();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblHeaderRole = new System.Windows.Forms.Label();
            this.btnHeaderOptions = new System.Windows.Forms.Button();
            this.lblHeaderName = new System.Windows.Forms.Label();
            this.pbHeaderAvatar = new System.Windows.Forms.PictureBox();
            this.panelLeft.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelInput.SuspendLayout();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbHeaderAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(248)))));
            this.panelLeft.Controls.Add(this.flpContacts);
            this.panelLeft.Controls.Add(this.txtSearchContacts);
            this.panelLeft.Controls.Add(this.lblTitle);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Padding = new System.Windows.Forms.Padding(10);
            this.panelLeft.Size = new System.Drawing.Size(260, 600);
            this.panelLeft.TabIndex = 0;
            // 
            // flpContacts
            // 
            this.flpContacts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpContacts.AutoScroll = true;
            this.flpContacts.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpContacts.Location = new System.Drawing.Point(10, 80);
            this.flpContacts.Name = "flpContacts";
            this.flpContacts.Size = new System.Drawing.Size(240, 510);
            this.flpContacts.TabIndex = 2;
            this.flpContacts.WrapContents = false;
            // 
            // txtSearchContacts
            // 
            this.txtSearchContacts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchContacts.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchContacts.ForeColor = System.Drawing.Color.Gray;
            this.txtSearchContacts.Location = new System.Drawing.Point(10, 45);
            this.txtSearchContacts.Name = "txtSearchContacts";
            this.txtSearchContacts.Size = new System.Drawing.Size(240, 25);
            this.txtSearchContacts.TabIndex = 1;
            this.txtSearchContacts.Text = "Tìm kiếm tin nhắn...";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(10, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(95, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "💬 Tin nhắn";
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.flpMessages);
            this.panelRight.Controls.Add(this.panelInput);
            this.panelRight.Controls.Add(this.panelHeader);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(260, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(540, 600);
            this.panelRight.TabIndex = 1;
            // 
            // flpMessages
            // 
            this.flpMessages.AutoScroll = true;
            this.flpMessages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.flpMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpMessages.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpMessages.Location = new System.Drawing.Point(0, 60);
            this.flpMessages.Name = "flpMessages";
            this.flpMessages.Padding = new System.Windows.Forms.Padding(10);
            this.flpMessages.Size = new System.Drawing.Size(540, 480);
            this.flpMessages.TabIndex = 2;
            this.flpMessages.WrapContents = false;
            // 
            // panelInput
            // 
            this.panelInput.BackColor = System.Drawing.Color.White;
            this.panelInput.Controls.Add(this.btnSend);
            this.panelInput.Controls.Add(this.txtInput);
            this.panelInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInput.Location = new System.Drawing.Point(0, 540);
            this.panelInput.Name = "panelInput";
            this.panelInput.Padding = new System.Windows.Forms.Padding(10);
            this.panelInput.Size = new System.Drawing.Size(540, 60);
            this.panelInput.TabIndex = 1;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BackColor = System.Drawing.Color.Transparent;
            this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend.FlatAppearance.BorderSize = 0;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSend.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(92)))), ((int)(((byte)(231)))));
            this.btnSend.Location = new System.Drawing.Point(490, 10);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(40, 40);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "➤";
            this.btnSend.UseVisualStyleBackColor = false;
            // 
            // txtInput
            // 
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInput.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInput.Location = new System.Drawing.Point(10, 12);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(470, 36);
            this.txtInput.TabIndex = 0;
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.lblHeaderRole);
            this.panelHeader.Controls.Add(this.btnHeaderOptions);
            this.panelHeader.Controls.Add(this.lblHeaderName);
            this.panelHeader.Controls.Add(this.pbHeaderAvatar);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(540, 60);
            this.panelHeader.TabIndex = 0;
            // 
            // lblHeaderRole
            // 
            this.lblHeaderRole.AutoSize = true;
            this.lblHeaderRole.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderRole.ForeColor = System.Drawing.Color.Gray;
            this.lblHeaderRole.Location = new System.Drawing.Point(60, 33);
            this.lblHeaderRole.Name = "lblHeaderRole";
            this.lblHeaderRole.Size = new System.Drawing.Size(45, 13);
            this.lblHeaderRole.TabIndex = 3;
            this.lblHeaderRole.Text = "Độc giả";
            // 
            // btnHeaderOptions
            // 
            this.btnHeaderOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHeaderOptions.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHeaderOptions.FlatAppearance.BorderSize = 0;
            this.btnHeaderOptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeaderOptions.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHeaderOptions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(92)))), ((int)(((byte)(231)))));
            this.btnHeaderOptions.Location = new System.Drawing.Point(495, 15);
            this.btnHeaderOptions.Name = "btnHeaderOptions";
            this.btnHeaderOptions.Size = new System.Drawing.Size(35, 30);
            this.btnHeaderOptions.TabIndex = 2;
            this.btnHeaderOptions.Text = "•••";
            this.btnHeaderOptions.UseVisualStyleBackColor = true;
            // 
            // lblHeaderName
            // 
            this.lblHeaderName.AutoSize = true;
            this.lblHeaderName.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderName.Location = new System.Drawing.Point(60, 12);
            this.lblHeaderName.Name = "lblHeaderName";
            this.lblHeaderName.Size = new System.Drawing.Size(102, 20);
            this.lblHeaderName.TabIndex = 1;
            this.lblHeaderName.Text = "Trần Văn B";
            // 
            // pbHeaderAvatar
            // 
            this.pbHeaderAvatar.Location = new System.Drawing.Point(15, 10);
            this.pbHeaderAvatar.Name = "pbHeaderAvatar";
            this.pbHeaderAvatar.Size = new System.Drawing.Size(40, 40);
            this.pbHeaderAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbHeaderAvatar.TabIndex = 0;
            this.pbHeaderAvatar.TabStop = false;
            // 
            // UC_Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLeft);
            this.Name = "UC_Chat";
            this.Size = new System.Drawing.Size(800, 600);
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.panelInput.ResumeLayout(false);
            this.panelInput.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbHeaderAvatar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtSearchContacts;
        private System.Windows.Forms.FlowLayoutPanel flpContacts;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblHeaderRole;
        private System.Windows.Forms.Button btnHeaderOptions;
        private System.Windows.Forms.Label lblHeaderName;
        private System.Windows.Forms.PictureBox pbHeaderAvatar;
        private System.Windows.Forms.Panel panelInput;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.FlowLayoutPanel flpMessages;
    }
}
