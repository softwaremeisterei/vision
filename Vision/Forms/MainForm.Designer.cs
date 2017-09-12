namespace Vision.Forms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.dockContainer1 = new Docking.Controls.DockContainer();
            this.SuspendLayout();
            // 
            // dockContainer1
            // 
            this.dockContainer1.BackColor = System.Drawing.SystemColors.Window;
            this.dockContainer1.BottomPanelHeight = 150;
            this.dockContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockContainer1.LeftPanelWidth = 150;
            this.dockContainer1.Location = new System.Drawing.Point(0, 0);
            this.dockContainer1.MinimumSize = new System.Drawing.Size(504, 528);
            this.dockContainer1.Name = "dockContainer1";
            this.dockContainer1.RightPanelWidth = 150;
            this.dockContainer1.SelectToolWindowsOnHoover = false;
            this.dockContainer1.Size = new System.Drawing.Size(1320, 784);
            this.dockContainer1.TabButtonNotSelectedColor = System.Drawing.Color.DarkGray;
            this.dockContainer1.TabButtonSelectedBackColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(242)))), ((int)(((byte)(200)))));
            this.dockContainer1.TabButtonSelectedBackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(215)))), ((int)(((byte)(157)))));
            this.dockContainer1.TabButtonSelectedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(111)))));
            this.dockContainer1.TabButtonSelectedColor = System.Drawing.Color.Black;
            this.dockContainer1.TabButtonShowSelection = false;
            this.dockContainer1.TabIndex = 0;
            this.dockContainer1.TopPanelHeight = 150;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1320, 784);
            this.Controls.Add(this.dockContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Visi<o>n";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private global::Docking.Controls.DockContainer dockContainer1;
    }
}