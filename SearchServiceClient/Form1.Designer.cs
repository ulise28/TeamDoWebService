namespace SearchServiceClient
{
    partial class Form1
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
            this.btnDeleteResults = new System.Windows.Forms.Button();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnUpdateIndex = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDeleteResults
            // 
            this.btnDeleteResults.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnDeleteResults.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.btnDeleteResults.FlatAppearance.BorderSize = 0;
            this.btnDeleteResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteResults.Location = new System.Drawing.Point(718, 6);
            this.btnDeleteResults.Name = "btnDeleteResults";
            this.btnDeleteResults.Size = new System.Drawing.Size(142, 23);
            this.btnDeleteResults.TabIndex = 9;
            this.btnDeleteResults.Text = "Clear textbox";
            this.btnDeleteResults.UseVisualStyleBackColor = false;
            this.btnDeleteResults.Click += new System.EventHandler(this.btnDeleteResults_Click);
            // 
            // txtResults
            // 
            this.txtResults.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtResults.Location = new System.Drawing.Point(0, 62);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResults.Size = new System.Drawing.Size(952, 404);
            this.txtResults.TabIndex = 8;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(357, 9);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(169, 20);
            this.txtSearch.TabIndex = 7;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnSearch.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Location = new System.Drawing.Point(532, 6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(142, 23);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Text = "Search text";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnUpdateIndex
            // 
            this.btnUpdateIndex.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnUpdateIndex.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.btnUpdateIndex.FlatAppearance.BorderSize = 0;
            this.btnUpdateIndex.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateIndex.Location = new System.Drawing.Point(30, 7);
            this.btnUpdateIndex.Name = "btnUpdateIndex";
            this.btnUpdateIndex.Size = new System.Drawing.Size(142, 23);
            this.btnUpdateIndex.TabIndex = 5;
            this.btnUpdateIndex.Text = "Create/Update Index";
            this.btnUpdateIndex.UseVisualStyleBackColor = false;
            this.btnUpdateIndex.Click += new System.EventHandler(this.btnUpdateIndex_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(952, 466);
            this.Controls.Add(this.btnDeleteResults);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnUpdateIndex);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDeleteResults;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnUpdateIndex;
    }
}

