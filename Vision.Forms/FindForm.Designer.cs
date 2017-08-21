namespace Vision.Forms
{
    partial class FindForm
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
            this.searchTextComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.findPrevButton = new System.Windows.Forms.Button();
            this.findNextButton = new System.Windows.Forms.Button();
            this.bookmarkAllButton = new System.Windows.Forms.Button();
            this.findAllButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // searchTextComboBox
            // 
            this.searchTextComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTextComboBox.FormattingEnabled = true;
            this.searchTextComboBox.Location = new System.Drawing.Point(6, 23);
            this.searchTextComboBox.Name = "searchTextComboBox";
            this.searchTextComboBox.Size = new System.Drawing.Size(336, 24);
            this.searchTextComboBox.TabIndex = 1;
            this.searchTextComboBox.TextUpdate += new System.EventHandler(this.searchTextComboBox_TextUpdate);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Find what:";
            // 
            // findPrevButton
            // 
            this.findPrevButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findPrevButton.Location = new System.Drawing.Point(98, 53);
            this.findPrevButton.Name = "findPrevButton";
            this.findPrevButton.Size = new System.Drawing.Size(119, 27);
            this.findPrevButton.TabIndex = 2;
            this.findPrevButton.Text = "Find &Prev";
            this.findPrevButton.UseVisualStyleBackColor = true;
            this.findPrevButton.Click += new System.EventHandler(this.findPrevButton_Click);
            // 
            // findNextButton
            // 
            this.findNextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findNextButton.Location = new System.Drawing.Point(223, 53);
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(119, 27);
            this.findNextButton.TabIndex = 3;
            this.findNextButton.Text = "&Find Next";
            this.findNextButton.UseVisualStyleBackColor = true;
            this.findNextButton.Click += new System.EventHandler(this.findNextButton_Click);
            // 
            // bookmarkAllButton
            // 
            this.bookmarkAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bookmarkAllButton.Location = new System.Drawing.Point(98, 82);
            this.bookmarkAllButton.Name = "bookmarkAllButton";
            this.bookmarkAllButton.Size = new System.Drawing.Size(119, 27);
            this.bookmarkAllButton.TabIndex = 4;
            this.bookmarkAllButton.Text = "&Bookmark All";
            this.bookmarkAllButton.UseVisualStyleBackColor = true;
            this.bookmarkAllButton.Visible = false;
            this.bookmarkAllButton.Click += new System.EventHandler(this.bookmarkAllButton_Click);
            // 
            // findAllButton
            // 
            this.findAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findAllButton.Location = new System.Drawing.Point(223, 82);
            this.findAllButton.Name = "findAllButton";
            this.findAllButton.Size = new System.Drawing.Size(119, 27);
            this.findAllButton.TabIndex = 5;
            this.findAllButton.Text = "Find &All";
            this.findAllButton.UseVisualStyleBackColor = true;
            this.findAllButton.Visible = false;
            this.findAllButton.Click += new System.EventHandler(this.findAllButton_Click);
            // 
            // FindForm
            // 
            this.AcceptButton = this.findNextButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 89);
            this.Controls.Add(this.findAllButton);
            this.Controls.Add(this.findNextButton);
            this.Controls.Add(this.bookmarkAllButton);
            this.Controls.Add(this.findPrevButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.searchTextComboBox);
            this.Name = "FindForm";
            this.Text = "Find";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FindForm_FormClosing);
            this.Load += new System.EventHandler(this.FindForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox searchTextComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button findPrevButton;
        private System.Windows.Forms.Button findNextButton;
        private System.Windows.Forms.Button bookmarkAllButton;
        private System.Windows.Forms.Button findAllButton;
    }
}