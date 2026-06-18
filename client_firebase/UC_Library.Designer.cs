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

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.flowBookmark = new System.Windows.Forms.FlowLayoutPanel();
            this.flowHistory = new System.Windows.Forms.FlowLayoutPanel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 600);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.flowBookmark);
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 566);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Bookmark";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.flowHistory);
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 566);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Lịch sử";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // flowBookmark
            // 
            this.flowBookmark.AutoScroll = true;
            this.flowBookmark.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowBookmark.Location = new System.Drawing.Point(3, 3);
            this.flowBookmark.Name = "flowBookmark";
            this.flowBookmark.Size = new System.Drawing.Size(786, 560);
            this.flowBookmark.TabIndex = 0;
            // 
            // flowHistory
            // 
            this.flowHistory.AutoScroll = true;
            this.flowHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowHistory.Location = new System.Drawing.Point(3, 3);
            this.flowHistory.Name = "flowHistory";
            this.flowHistory.Size = new System.Drawing.Size(786, 560);
            this.flowHistory.TabIndex = 0;
            // 
            // UC_Library
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.tabControl1);
            this.Name = "UC_Library";
            this.Size = new System.Drawing.Size(800, 600);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.FlowLayoutPanel flowBookmark;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.FlowLayoutPanel flowHistory;
    }
}
