namespace client_firebase
{
    partial class UC_BookDetail
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelCommentsCard = new System.Windows.Forms.Panel();
            this.flpComments = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPostComment = new System.Windows.Forms.Button();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.lblCommentsHeader = new System.Windows.Forms.Label();
            this.panelChaptersCard = new System.Windows.Forms.Panel();
            this.flpChapters = new System.Windows.Forms.FlowLayoutPanel();
            this.lblChaptersHeader = new System.Windows.Forms.Label();
            this.panelInfoCard = new System.Windows.Forms.Panel();
            this.btnChat = new System.Windows.Forms.Button();
            this.btnRate = new System.Windows.Forms.Button();
            this.btnBookmark = new System.Windows.Forms.Button();
            this.btnRead = new System.Windows.Forms.Button();
            this.lblRating = new System.Windows.Forms.Label();
            this.lblChaptersCount = new System.Windows.Forms.Label();
            this.lblLikesCount = new System.Windows.Forms.Label();
            this.lblViewsCount = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblAuthorRole = new System.Windows.Forms.Label();
            this.lblAuthorName = new System.Windows.Forms.Label();
            this.pbAuthor = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pbCover = new System.Windows.Forms.PictureBox();
            this.panelMain.SuspendLayout();
            this.panelCommentsCard.SuspendLayout();
            this.panelChaptersCard.SuspendLayout();
            this.panelInfoCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAuthor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCover)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.AutoScroll = true;
            this.panelMain.Controls.Add(this.panelCommentsCard);
            this.panelMain.Controls.Add(this.panelChaptersCard);
            this.panelMain.Controls.Add(this.panelInfoCard);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(15);
            this.panelMain.Size = new System.Drawing.Size(800, 689);
            this.panelMain.TabIndex = 0;
            // 
            // panelCommentsCard
            // 
            this.panelCommentsCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCommentsCard.BackColor = System.Drawing.Color.White;
            this.panelCommentsCard.Controls.Add(this.flpComments);
            this.panelCommentsCard.Controls.Add(this.btnPostComment);
            this.panelCommentsCard.Controls.Add(this.txtComment);
            this.panelCommentsCard.Controls.Add(this.lblCommentsHeader);
            this.panelCommentsCard.Location = new System.Drawing.Point(15, 534);
            this.panelCommentsCard.Name = "panelCommentsCard";
            this.panelCommentsCard.Size = new System.Drawing.Size(750, 522);
            this.panelCommentsCard.TabIndex = 2;
            // 
            // flpComments
            // 
            this.flpComments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpComments.AutoScroll = true;
            this.flpComments.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpComments.Location = new System.Drawing.Point(20, 150);
            this.flpComments.Name = "flpComments";
            this.flpComments.Size = new System.Drawing.Size(710, 357);
            this.flpComments.TabIndex = 3;
            this.flpComments.WrapContents = false;
            // 
            // btnPostComment
            // 
            this.btnPostComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPostComment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(92)))), ((int)(((byte)(231)))));
            this.btnPostComment.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPostComment.FlatAppearance.BorderSize = 0;
            this.btnPostComment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPostComment.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPostComment.ForeColor = System.Drawing.Color.White;
            this.btnPostComment.Location = new System.Drawing.Point(580, 105);
            this.btnPostComment.Name = "btnPostComment";
            this.btnPostComment.Size = new System.Drawing.Size(150, 35);
            this.btnPostComment.TabIndex = 2;
            this.btnPostComment.Text = "✉  Đăng bình luận";
            this.btnPostComment.UseVisualStyleBackColor = false;
            // 
            // txtComment
            // 
            this.txtComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtComment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(248)))));
            this.txtComment.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtComment.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtComment.ForeColor = System.Drawing.Color.Black;
            this.txtComment.Location = new System.Drawing.Point(20, 45);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(710, 50);
            this.txtComment.TabIndex = 1;
            this.txtComment.Text = "Viết bình luận của bạn...";
            // 
            // lblCommentsHeader
            // 
            this.lblCommentsHeader.AutoSize = true;
            this.lblCommentsHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCommentsHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(92)))), ((int)(((byte)(231)))));
            this.lblCommentsHeader.Location = new System.Drawing.Point(20, 15);
            this.lblCommentsHeader.Name = "lblCommentsHeader";
            this.lblCommentsHeader.Size = new System.Drawing.Size(101, 20);
            this.lblCommentsHeader.TabIndex = 0;
            this.lblCommentsHeader.Text = "💬 Bình luận";
            // 
            // panelChaptersCard
            // 
            this.panelChaptersCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelChaptersCard.BackColor = System.Drawing.Color.White;
            this.panelChaptersCard.Controls.Add(this.flpChapters);
            this.panelChaptersCard.Controls.Add(this.lblChaptersHeader);
            this.panelChaptersCard.Location = new System.Drawing.Point(15, 244);
            this.panelChaptersCard.Name = "panelChaptersCard";
            this.panelChaptersCard.Size = new System.Drawing.Size(750, 284);
            this.panelChaptersCard.TabIndex = 1;
            // 
            // flpChapters
            // 
            this.flpChapters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpChapters.AutoScroll = true;
            this.flpChapters.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpChapters.Location = new System.Drawing.Point(20, 34);
            this.flpChapters.Name = "flpChapters";
            this.flpChapters.Size = new System.Drawing.Size(710, 235);
            this.flpChapters.TabIndex = 1;
            this.flpChapters.WrapContents = false;
            // 
            // lblChaptersHeader
            // 
            this.lblChaptersHeader.AutoSize = true;
            this.lblChaptersHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChaptersHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(92)))), ((int)(((byte)(231)))));
            this.lblChaptersHeader.Location = new System.Drawing.Point(20, 7);
            this.lblChaptersHeader.Name = "lblChaptersHeader";
            this.lblChaptersHeader.Size = new System.Drawing.Size(164, 20);
            this.lblChaptersHeader.TabIndex = 0;
            this.lblChaptersHeader.Text = "📖 Danh sách chương";
            // 
            // panelInfoCard
            // 
            this.panelInfoCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInfoCard.BackColor = System.Drawing.Color.White;
            this.panelInfoCard.Controls.Add(this.btnChat);
            this.panelInfoCard.Controls.Add(this.btnRate);
            this.panelInfoCard.Controls.Add(this.btnBookmark);
            this.panelInfoCard.Controls.Add(this.btnRead);
            this.panelInfoCard.Controls.Add(this.lblRating);
            this.panelInfoCard.Controls.Add(this.lblChaptersCount);
            this.panelInfoCard.Controls.Add(this.lblLikesCount);
            this.panelInfoCard.Controls.Add(this.lblViewsCount);
            this.panelInfoCard.Controls.Add(this.lblDescription);
            this.panelInfoCard.Controls.Add(this.lblAuthorRole);
            this.panelInfoCard.Controls.Add(this.lblAuthorName);
            this.panelInfoCard.Controls.Add(this.pbAuthor);
            this.panelInfoCard.Controls.Add(this.lblTitle);
            this.panelInfoCard.Controls.Add(this.pbCover);
            this.panelInfoCard.Location = new System.Drawing.Point(15, 18);
            this.panelInfoCard.Name = "panelInfoCard";
            this.panelInfoCard.Size = new System.Drawing.Size(750, 220);
            this.panelInfoCard.TabIndex = 0;
            // 
            // btnChat
            // 
            this.btnChat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChat.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChat.Location = new System.Drawing.Point(505, 175);
            this.btnChat.Name = "btnChat";
            this.btnChat.Size = new System.Drawing.Size(100, 30);
            this.btnChat.TabIndex = 13;
            this.btnChat.Text = "💬 Chat";
            this.btnChat.UseVisualStyleBackColor = true;
            // 
            // btnRate
            // 
            this.btnRate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRate.Location = new System.Drawing.Point(395, 175);
            this.btnRate.Name = "btnRate";
            this.btnRate.Size = new System.Drawing.Size(100, 30);
            this.btnRate.TabIndex = 12;
            this.btnRate.Text = "★ Đánh giá";
            this.btnRate.UseVisualStyleBackColor = true;
            // 
            // btnBookmark
            // 
            this.btnBookmark.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBookmark.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBookmark.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBookmark.Location = new System.Drawing.Point(285, 175);
            this.btnBookmark.Name = "btnBookmark";
            this.btnBookmark.Size = new System.Drawing.Size(100, 30);
            this.btnBookmark.TabIndex = 11;
            this.btnBookmark.Text = "🔖 Bookmark";
            this.btnBookmark.UseVisualStyleBackColor = true;
            // 
            // btnRead
            // 
            this.btnRead.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(92)))), ((int)(((byte)(231)))));
            this.btnRead.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRead.FlatAppearance.BorderSize = 0;
            this.btnRead.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRead.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRead.ForeColor = System.Drawing.Color.White;
            this.btnRead.Location = new System.Drawing.Point(165, 175);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(110, 30);
            this.btnRead.TabIndex = 10;
            this.btnRead.Text = "📘 Đọc từ đầu";
            this.btnRead.UseVisualStyleBackColor = false;
            // 
            // lblRating
            // 
            this.lblRating.AutoSize = true;
            this.lblRating.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRating.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.lblRating.Location = new System.Drawing.Point(495, 145);
            this.lblRating.Name = "lblRating";
            this.lblRating.Size = new System.Drawing.Size(88, 15);
            this.lblRating.TabIndex = 9;
            this.lblRating.Text = "⭐ 5.0 ★★★★★";
            // 
            // lblChaptersCount
            // 
            this.lblChaptersCount.AutoSize = true;
            this.lblChaptersCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChaptersCount.ForeColor = System.Drawing.Color.Gray;
            this.lblChaptersCount.Location = new System.Drawing.Point(395, 145);
            this.lblChaptersCount.Name = "lblChaptersCount";
            this.lblChaptersCount.Size = new System.Drawing.Size(72, 15);
            this.lblChaptersCount.TabIndex = 8;
            this.lblChaptersCount.Text = "📖 1 chương";
            // 
            // lblLikesCount
            // 
            this.lblLikesCount.AutoSize = true;
            this.lblLikesCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLikesCount.ForeColor = System.Drawing.Color.Gray;
            this.lblLikesCount.Location = new System.Drawing.Point(285, 145);
            this.lblLikesCount.Name = "lblLikesCount";
            this.lblLikesCount.Size = new System.Drawing.Size(90, 15);
            this.lblLikesCount.TabIndex = 7;
            this.lblLikesCount.Text = "♡ 12k lượt thích";
            // 
            // lblViewsCount
            // 
            this.lblViewsCount.AutoSize = true;
            this.lblViewsCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblViewsCount.ForeColor = System.Drawing.Color.Gray;
            this.lblViewsCount.Location = new System.Drawing.Point(165, 145);
            this.lblViewsCount.Name = "lblViewsCount";
            this.lblViewsCount.Size = new System.Drawing.Size(95, 15);
            this.lblViewsCount.TabIndex = 6;
            this.lblViewsCount.Text = "👁 120k lượt xem";
            // 
            // lblDescription
            // 
            this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescription.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Location = new System.Drawing.Point(165, 90);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(565, 45);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Mô tả truyện...";
            // 
            // lblAuthorRole
            // 
            this.lblAuthorRole.AutoSize = true;
            this.lblAuthorRole.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuthorRole.ForeColor = System.Drawing.Color.Gray;
            this.lblAuthorRole.Location = new System.Drawing.Point(205, 65);
            this.lblAuthorRole.Name = "lblAuthorRole";
            this.lblAuthorRole.Size = new System.Drawing.Size(41, 13);
            this.lblAuthorRole.TabIndex = 4;
            this.lblAuthorRole.Text = "Tác giả";
            // 
            // lblAuthorName
            // 
            this.lblAuthorName.AutoSize = true;
            this.lblAuthorName.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuthorName.Location = new System.Drawing.Point(205, 45);
            this.lblAuthorName.Name = "lblAuthorName";
            this.lblAuthorName.Size = new System.Drawing.Size(83, 17);
            this.lblAuthorName.TabIndex = 3;
            this.lblAuthorName.Text = "J.K. Rowling";
            // 
            // pbAuthor
            // 
            this.pbAuthor.Location = new System.Drawing.Point(165, 45);
            this.pbAuthor.Name = "pbAuthor";
            this.pbAuthor.Size = new System.Drawing.Size(32, 32);
            this.pbAuthor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbAuthor.TabIndex = 2;
            this.pbAuthor.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(160, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(124, 25);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Harry Potter";
            // 
            // pbCover
            // 
            this.pbCover.BackColor = System.Drawing.Color.Gainsboro;
            this.pbCover.Location = new System.Drawing.Point(15, 15);
            this.pbCover.Name = "pbCover";
            this.pbCover.Size = new System.Drawing.Size(130, 180);
            this.pbCover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbCover.TabIndex = 0;
            this.pbCover.TabStop = false;
            // 
            // UC_BookDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(248)))));
            this.Controls.Add(this.panelMain);
            this.Name = "UC_BookDetail";
            this.Size = new System.Drawing.Size(800, 689);
            this.panelMain.ResumeLayout(false);
            this.panelCommentsCard.ResumeLayout(false);
            this.panelCommentsCard.PerformLayout();
            this.panelChaptersCard.ResumeLayout(false);
            this.panelChaptersCard.PerformLayout();
            this.panelInfoCard.ResumeLayout(false);
            this.panelInfoCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAuthor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCover)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelInfoCard;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox pbCover;
        private System.Windows.Forms.PictureBox pbAuthor;
        private System.Windows.Forms.Label lblAuthorName;
        private System.Windows.Forms.Label lblAuthorRole;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblViewsCount;
        private System.Windows.Forms.Label lblLikesCount;
        private System.Windows.Forms.Label lblChaptersCount;
        private System.Windows.Forms.Label lblRating;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.Button btnBookmark;
        private System.Windows.Forms.Button btnRate;
        private System.Windows.Forms.Button btnChat;
        private System.Windows.Forms.Panel panelChaptersCard;
        private System.Windows.Forms.Label lblChaptersHeader;
        private System.Windows.Forms.FlowLayoutPanel flpChapters;
        private System.Windows.Forms.Panel panelCommentsCard;
        private System.Windows.Forms.Label lblCommentsHeader;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.Button btnPostComment;
        private System.Windows.Forms.FlowLayoutPanel flpComments;
    }
}
