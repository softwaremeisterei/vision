﻿namespace Vision
{
    partial class BrowserToolWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserToolWindow));
            this.extWebBrowser1 = new Vision.ExtWebBrowser();
            this.SuspendLayout();
            // 
            // extWebBrowser1
            // 
            this.extWebBrowser1.AllowDrop = true;
            this.extWebBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extWebBrowser1.Location = new System.Drawing.Point(0, 0);
            this.extWebBrowser1.Name = "extWebBrowser1";
            this.extWebBrowser1.Size = new System.Drawing.Size(443, 385);
            this.extWebBrowser1.TabIndex = 0;
            // 
            // BrowserToolWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 385);
            this.Controls.Add(this.extWebBrowser1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BrowserToolWindow";
            this.Text = "Browser";
            this.ResumeLayout(false);

        }

        #endregion

        private ExtWebBrowser extWebBrowser1;
    }
}