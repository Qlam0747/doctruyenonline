namespace client_firebase
{
    partial class UC_Search
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
            this.panelSearchInput = new System.Windows.Forms.Panel();
            this.lblSearchIcon = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnFilter = new System.Windows.Forms.Button();
            this.panelFilters = new System.Windows.Forms.Panel();
            this.lblGenreTitle = new System.Windows.Forms.Label();
            this.flpGenres = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSortTitle = new System.Windows.Forms.Label();
            this.flpSort = new System.Windows.Forms.FlowLayoutPanel();
            this.lblResultsTitle = new System.Windows.Forms.Label();
            this.flpResults = new System.Windows.Forms.FlowLayoutPanel();
            this.panelSearchInput.SuspendLayout();
            this.panelFilters.SuspendLayout();
            this.SuspendLayout();
            
            
            
            this.panelSearchInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSearchInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.panelSearchInput.Controls.Add(this.lblSearchIcon);
            this.panelSearchInput.Controls.Add(this.txtSearch);
            this.panelSearchInput.Location = new System.Drawing.Point(20, 20);
            this.panelSearchInput.Name = "panelSearchInput";
            this.panelSearchInput.Size = new System.Drawing.Size(635, 40);
            this.panelSearchInput.TabIndex = 0;
            
            
            
            this.lblSearchIcon.AutoSize = true;
            this.lblSearchIcon.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblSearchIcon.ForeColor = System.Drawing.Color.Gray;
            this.lblSearchIcon.Location = new System.Drawing.Point(10, 9);
            this.lblSearchIcon.Name = "lblSearchIcon";
            this.lblSearchIcon.Size = new System.Drawing.Size(26, 21);
            this.lblSearchIcon.TabIndex = 0;
            this.lblSearchIcon.Text = "🔍";
            
            
            
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(52)))), ((int)(((byte)(54)))));
            this.txtSearch.Location = new System.Drawing.Point(40, 10);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(585, 20);
            this.txtSearch.TabIndex = 1;
            
            
            
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(92)))), ((int)(((byte)(231)))));
            this.btnFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilter.FlatAppearance.BorderSize = 0;
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilter.ForeColor = System.Drawing.Color.White;
            this.btnFilter.Location = new System.Drawing.Point(670, 20);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(110, 40);
            this.btnFilter.TabIndex = 1;
            this.btnFilter.Text = "⛛  Bộ lọc";
            this.btnFilter.UseVisualStyleBackColor = false;
            
            
            
            this.panelFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelFilters.Controls.Add(this.lblGenreTitle);
            this.panelFilters.Controls.Add(this.flpGenres);
            this.panelFilters.Controls.Add(this.lblSortTitle);
            this.panelFilters.Controls.Add(this.flpSort);
            this.panelFilters.Location = new System.Drawing.Point(20, 75);
            this.panelFilters.Name = "panelFilters";
            this.panelFilters.Size = new System.Drawing.Size(760, 160);
            this.panelFilters.TabIndex = 2;
            
            
            
            this.lblGenreTitle.AutoSize = true;
            this.lblGenreTitle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGenreTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(52)))), ((int)(((byte)(54)))));
            this.lblGenreTitle.Location = new System.Drawing.Point(0, 0);
            this.lblGenreTitle.Name = "lblGenreTitle";
            this.lblGenreTitle.Size = new System.Drawing.Size(64, 20);
            this.lblGenreTitle.TabIndex = 0;
            this.lblGenreTitle.Text = "Thể loại";
            
            
            
            this.flpGenres.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpGenres.Location = new System.Drawing.Point(0, 25);
            this.flpGenres.Name = "flpGenres";
            this.flpGenres.Size = new System.Drawing.Size(760, 65);
            this.flpGenres.TabIndex = 1;
            
            
            
            this.lblSortTitle.AutoSize = true;
            this.lblSortTitle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSortTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(52)))), ((int)(((byte)(54)))));
            this.lblSortTitle.Location = new System.Drawing.Point(0, 95);
            this.lblSortTitle.Name = "lblSortTitle";
            this.lblSortTitle.Size = new System.Drawing.Size(95, 20);
            this.lblSortTitle.TabIndex = 2;
            this.lblSortTitle.Text = "Sắp xếp theo";
            
            
            
            this.flpSort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpSort.Location = new System.Drawing.Point(0, 120);
            this.flpSort.Name = "flpSort";
            this.flpSort.Size = new System.Drawing.Size(760, 35);
            this.flpSort.TabIndex = 3;
            
            
            
            this.lblResultsTitle.AutoSize = true;
            this.lblResultsTitle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResultsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(52)))), ((int)(((byte)(54)))));
            this.lblResultsTitle.Location = new System.Drawing.Point(20, 245);
            this.lblResultsTitle.Name = "lblResultsTitle";
            this.lblResultsTitle.Size = new System.Drawing.Size(161, 25);
            this.lblResultsTitle.TabIndex = 3;
            this.lblResultsTitle.Text = "Kết quả tìm được";
            
            
            
            this.flpResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpResults.AutoScroll = true;
            this.flpResults.Location = new System.Drawing.Point(20, 275);
            this.flpResults.Name = "flpResults";
            this.flpResults.Size = new System.Drawing.Size(760, 305);
            this.flpResults.TabIndex = 4;
            
            
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.flpResults);
            this.Controls.Add(this.lblResultsTitle);
            this.Controls.Add(this.panelFilters);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.panelSearchInput);
            this.Name = "UC_Search";
            this.Size = new System.Drawing.Size(800, 600);
            this.panelSearchInput.ResumeLayout(false);
            this.panelSearchInput.PerformLayout();
            this.panelFilters.ResumeLayout(false);
            this.panelFilters.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelSearchInput;
        private System.Windows.Forms.Label lblSearchIcon;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Panel panelFilters;
        private System.Windows.Forms.Label lblGenreTitle;
        private System.Windows.Forms.FlowLayoutPanel flpGenres;
        private System.Windows.Forms.Label lblSortTitle;
        private System.Windows.Forms.FlowLayoutPanel flpSort;
        private System.Windows.Forms.Label lblResultsTitle;
        private System.Windows.Forms.FlowLayoutPanel flpResults;
    }
}
