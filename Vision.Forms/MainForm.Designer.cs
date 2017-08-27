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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.collapseButton = new System.Windows.Forms.Button();
            this.expandButton = new System.Windows.Forms.Button();
            this.contentRichTextBox = new RichTextBoxExtended.RichTextBoxExtended();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.openFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToplevelNodeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSiblingNodeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addChildNodeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.findMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findPrevMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveNodeDownMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveNodeUpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.HideSelection = false;
            this.treeView1.LabelEdit = true;
            this.treeView1.Location = new System.Drawing.Point(0, 33);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(265, 346);
            this.treeView1.TabIndex = 2;
            this.treeView1.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_AfterLabelEdit);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 31);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.collapseButton);
            this.splitContainer1.Panel1.Controls.Add(this.expandButton);
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.contentRichTextBox);
            this.splitContainer1.Size = new System.Drawing.Size(807, 382);
            this.splitContainer1.SplitterDistance = 268;
            this.splitContainer1.TabIndex = 1;
            this.splitContainer1.TabStop = false;
            // 
            // collapseButton
            // 
            this.collapseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.collapseButton.Location = new System.Drawing.Point(173, 3);
            this.collapseButton.Name = "collapseButton";
            this.collapseButton.Size = new System.Drawing.Size(92, 27);
            this.collapseButton.TabIndex = 1;
            this.collapseButton.Text = "Collapse All";
            this.collapseButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.collapseButton.UseVisualStyleBackColor = true;
            this.collapseButton.Click += new System.EventHandler(this.collapseButton_Click);
            // 
            // expandButton
            // 
            this.expandButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.expandButton.Location = new System.Drawing.Point(75, 3);
            this.expandButton.Name = "expandButton";
            this.expandButton.Size = new System.Drawing.Size(92, 27);
            this.expandButton.TabIndex = 0;
            this.expandButton.Text = "Expand All";
            this.expandButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.expandButton.UseVisualStyleBackColor = true;
            this.expandButton.Click += new System.EventHandler(this.expandButton_Click);
            // 
            // contentRichTextBox
            // 
            this.contentRichTextBox.AcceptsTab = false;
            this.contentRichTextBox.AllowDrop = true;
            this.contentRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentRichTextBox.AutoWordSelection = true;
            this.contentRichTextBox.DetectURLs = true;
            this.contentRichTextBox.Location = new System.Drawing.Point(3, 3);
            this.contentRichTextBox.Name = "contentRichTextBox";
            this.contentRichTextBox.ReadOnly = false;
            // 
            // 
            // 
            this.contentRichTextBox.RichTextBox.AutoWordSelection = true;
            this.contentRichTextBox.RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentRichTextBox.RichTextBox.Location = new System.Drawing.Point(0, 26);
            this.contentRichTextBox.RichTextBox.Name = "rtb1";
            this.contentRichTextBox.RichTextBox.Size = new System.Drawing.Size(529, 350);
            this.contentRichTextBox.RichTextBox.TabIndex = 1;
            this.contentRichTextBox.RichTextBox.TextChanged += new System.EventHandler(this.contentRichTextBox_TextChanged);
            this.contentRichTextBox.ShowBold = true;
            this.contentRichTextBox.ShowCenterJustify = true;
            this.contentRichTextBox.ShowColors = true;
            this.contentRichTextBox.ShowCopy = true;
            this.contentRichTextBox.ShowCut = true;
            this.contentRichTextBox.ShowFont = true;
            this.contentRichTextBox.ShowFontSize = true;
            this.contentRichTextBox.ShowItalic = true;
            this.contentRichTextBox.ShowLeftJustify = true;
            this.contentRichTextBox.ShowOpen = true;
            this.contentRichTextBox.ShowPaste = true;
            this.contentRichTextBox.ShowRedo = true;
            this.contentRichTextBox.ShowRightJustify = true;
            this.contentRichTextBox.ShowSave = true;
            this.contentRichTextBox.ShowStamp = true;
            this.contentRichTextBox.ShowStrikeout = true;
            this.contentRichTextBox.ShowToolBarText = false;
            this.contentRichTextBox.ShowUnderline = true;
            this.contentRichTextBox.ShowUndo = true;
            this.contentRichTextBox.Size = new System.Drawing.Size(529, 376);
            this.contentRichTextBox.StampAction = RichTextBoxExtended.StampActions.EditedBy;
            this.contentRichTextBox.StampColor = System.Drawing.Color.Blue;
            this.contentRichTextBox.TabIndex = 0;
            // 
            // 
            // 
            this.contentRichTextBox.Toolbar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.contentRichTextBox.Toolbar.ButtonSize = new System.Drawing.Size(16, 16);
            this.contentRichTextBox.Toolbar.Divider = false;
            this.contentRichTextBox.Toolbar.DropDownArrows = true;
            this.contentRichTextBox.Toolbar.Location = new System.Drawing.Point(0, 0);
            this.contentRichTextBox.Toolbar.Name = "tb1";
            this.contentRichTextBox.Toolbar.ShowToolTips = true;
            this.contentRichTextBox.Toolbar.Size = new System.Drawing.Size(529, 26);
            this.contentRichTextBox.Toolbar.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 416);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(807, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.contentToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(807, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFileMenuItem,
            this.toolStripSeparator2,
            this.openFileMenuItem,
            this.toolStripSeparator1,
            this.saveFileMenuItem,
            this.exportMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newFileMenuItem
            // 
            this.newFileMenuItem.Name = "newFileMenuItem";
            this.newFileMenuItem.Size = new System.Drawing.Size(182, 26);
            this.newFileMenuItem.Text = "&New";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(179, 6);
            // 
            // openFileMenuItem
            // 
            this.openFileMenuItem.Name = "openFileMenuItem";
            this.openFileMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openFileMenuItem.Size = new System.Drawing.Size(182, 26);
            this.openFileMenuItem.Text = "&Open...";
            this.openFileMenuItem.Click += new System.EventHandler(this.openFileMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(179, 6);
            // 
            // saveFileMenuItem
            // 
            this.saveFileMenuItem.Name = "saveFileMenuItem";
            this.saveFileMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveFileMenuItem.Size = new System.Drawing.Size(182, 26);
            this.saveFileMenuItem.Text = "&Save...";
            this.saveFileMenuItem.Click += new System.EventHandler(this.saveFileMenuItem_Click);
            // 
            // exportMenuItem
            // 
            this.exportMenuItem.Name = "exportMenuItem";
            this.exportMenuItem.Size = new System.Drawing.Size(182, 26);
            this.exportMenuItem.Text = "E&xport...";
            this.exportMenuItem.Click += new System.EventHandler(this.exportMenuItem_Click);
            // 
            // contentToolStripMenuItem
            // 
            this.contentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToplevelNodeMenuItem,
            this.addSiblingNodeMenuItem,
            this.addChildNodeMenuItem,
            this.toolStripSeparator3,
            this.moveNodeUpMenuItem,
            this.moveNodeDownMenuItem,
            this.toolStripSeparator4,
            this.findMenuItem,
            this.findNextMenuItem,
            this.findPrevMenuItem});
            this.contentToolStripMenuItem.Name = "contentToolStripMenuItem";
            this.contentToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.contentToolStripMenuItem.Text = "Content";
            // 
            // addToplevelNodeMenuItem
            // 
            this.addToplevelNodeMenuItem.Name = "addToplevelNodeMenuItem";
            this.addToplevelNodeMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.addToplevelNodeMenuItem.Size = new System.Drawing.Size(284, 26);
            this.addToplevelNodeMenuItem.Text = "Add &Toplevel Node";
            this.addToplevelNodeMenuItem.Click += new System.EventHandler(this.addToplevelNodeMenuItem_Click);
            // 
            // addSiblingNodeMenuItem
            // 
            this.addSiblingNodeMenuItem.Name = "addSiblingNodeMenuItem";
            this.addSiblingNodeMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.addSiblingNodeMenuItem.Size = new System.Drawing.Size(284, 26);
            this.addSiblingNodeMenuItem.Text = "Add &Sibling Node";
            this.addSiblingNodeMenuItem.Click += new System.EventHandler(this.addSiblingNodeMenuItem_Click);
            // 
            // addChildNodeMenuItem
            // 
            this.addChildNodeMenuItem.Name = "addChildNodeMenuItem";
            this.addChildNodeMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Right)));
            this.addChildNodeMenuItem.Size = new System.Drawing.Size(284, 26);
            this.addChildNodeMenuItem.Text = "Add &Child Node";
            this.addChildNodeMenuItem.Click += new System.EventHandler(this.addChildNodeMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(281, 6);
            // 
            // findMenuItem
            // 
            this.findMenuItem.Name = "findMenuItem";
            this.findMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findMenuItem.Size = new System.Drawing.Size(284, 26);
            this.findMenuItem.Text = "&Find";
            this.findMenuItem.Click += new System.EventHandler(this.findMenuItem_Click);
            // 
            // findNextMenuItem
            // 
            this.findNextMenuItem.Name = "findNextMenuItem";
            this.findNextMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextMenuItem.Size = new System.Drawing.Size(284, 26);
            this.findNextMenuItem.Text = "Find &Next";
            this.findNextMenuItem.Click += new System.EventHandler(this.findNextMenuItem_Click);
            // 
            // findPrevMenuItem
            // 
            this.findPrevMenuItem.Name = "findPrevMenuItem";
            this.findPrevMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.findPrevMenuItem.Size = new System.Drawing.Size(284, 26);
            this.findPrevMenuItem.Text = "Find &Prev";
            this.findPrevMenuItem.Click += new System.EventHandler(this.findPrevMenuItem_Click);
            // 
            // moveNodeDownMenuItem
            // 
            this.moveNodeDownMenuItem.Name = "moveNodeDownMenuItem";
            this.moveNodeDownMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Down)));
            this.moveNodeDownMenuItem.Size = new System.Drawing.Size(284, 26);
            this.moveNodeDownMenuItem.Text = "Move Node &Down";
            this.moveNodeDownMenuItem.Click += new System.EventHandler(this.moveNodeDownMenuItem_Click);
            // 
            // moveNodeUpMenuItem
            // 
            this.moveNodeUpMenuItem.Name = "moveNodeUpMenuItem";
            this.moveNodeUpMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Up)));
            this.moveNodeUpMenuItem.Size = new System.Drawing.Size(284, 26);
            this.moveNodeUpMenuItem.Text = "Move Node &Up";
            this.moveNodeUpMenuItem.Click += new System.EventHandler(this.moveNodeUpMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(281, 6);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 438);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.splitContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Visi<o>n";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFileMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem openFileMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToplevelNodeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addChildNodeMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button expandButton;
        private RichTextBoxExtended.RichTextBoxExtended contentRichTextBox;
        private System.Windows.Forms.Button collapseButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem findMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findNextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findPrevMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSiblingNodeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveNodeDownMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveNodeUpMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}

