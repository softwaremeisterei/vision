namespace Vision
{
    partial class ExtWebBrowser
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.forwardButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.goButton = new System.Windows.Forms.Button();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.clearUrlButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // forwardButton
            // 
            this.forwardButton.Location = new System.Drawing.Point(25, 3);
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(23, 25);
            this.forwardButton.TabIndex = 1;
            this.forwardButton.Text = "▶";
            this.forwardButton.UseVisualStyleBackColor = true;
            this.forwardButton.Click += new System.EventHandler(this.forwardButton_Click);
            // 
            // backButton
            // 
            this.backButton.Location = new System.Drawing.Point(3, 3);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(23, 25);
            this.backButton.TabIndex = 0;
            this.backButton.Text = "◀";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // goButton
            // 
            this.goButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.goButton.Location = new System.Drawing.Point(504, 3);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(42, 25);
            this.goButton.TabIndex = 4;
            this.goButton.Text = "Go!";
            this.goButton.UseVisualStyleBackColor = true;
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.urlTextBox.Location = new System.Drawing.Point(53, 4);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(423, 22);
            this.urlTextBox.TabIndex = 2;
            this.urlTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.urlTextBox_KeyDown);
            // 
            // webBrowser1
            // 
            this.webBrowser1.AllowWebBrowserDrop = false;
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(3, 33);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(543, 524);
            this.webBrowser1.TabIndex = 5;
            // 
            // clearUrlButton
            // 
            this.clearUrlButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearUrlButton.Location = new System.Drawing.Point(477, 3);
            this.clearUrlButton.Name = "clearUrlButton";
            this.clearUrlButton.Size = new System.Drawing.Size(24, 25);
            this.clearUrlButton.TabIndex = 3;
            this.clearUrlButton.Text = "✕";
            this.clearUrlButton.UseVisualStyleBackColor = true;
            this.clearUrlButton.Click += new System.EventHandler(this.clearUrlButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.backButton);
            this.panel1.Controls.Add(this.webBrowser1);
            this.panel1.Controls.Add(this.clearUrlButton);
            this.panel1.Controls.Add(this.goButton);
            this.panel1.Controls.Add(this.forwardButton);
            this.panel1.Controls.Add(this.urlTextBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(549, 560);
            this.panel1.TabIndex = 10;
            // 
            // ExtWebBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "ExtWebBrowser";
            this.Size = new System.Drawing.Size(549, 560);
            this.Load += new System.EventHandler(this.ExtWebBrowser_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button forwardButton;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button clearUrlButton;
        private System.Windows.Forms.Panel panel1;
    }
}
