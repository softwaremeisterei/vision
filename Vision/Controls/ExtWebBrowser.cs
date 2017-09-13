using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.Lib;
using Vision.BL.Model;

namespace Vision
{
    public partial class ExtWebBrowser : UserControl
    {
        public WebBrowser WebBrowser { get; set; }

        public ExtWebBrowser()
        {
            InitializeComponent();
            WebBrowser = webBrowser1;
        }

        private void ExtWebBrowser_Load(object sender, EventArgs e)
        {
            FocusUrl();
        }

        private void urlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Navigate(urlTextBox.Text);
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void clearUrlButton_Click(object sender, EventArgs e)
        {
            urlTextBox.Clear();
            urlTextBox.Focus();
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            using (new WaitCursor())
            {
                webBrowser1.Navigate(urlTextBox.Text);
            }
        }

        internal void Navigate(string url)
        {
            urlTextBox.Text = url;
            webBrowser1.Navigate(url);
        }

        public void FocusUrl()
        {
            urlTextBox.Focus();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != webBrowser1.Url)
            {
                return;
            }

            urlTextBox.Text = e.Url.ToString();
        }

        private void urlTextBox_DragOver(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                e.Effect = DragDropEffects.Move;
            }
            else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void urlTextBox_DragDrop(object sender, DragEventArgs e)
        {
            string url = null;

            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                url = e.Data.GetData(DataFormats.Text).ToString();
            }
            else if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                var treeNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                var node = (Node)treeNode.Tag;
                url = node.Url;
            }

            if (url != null)
            {
                Navigate(url);
            }
        }
    }
}
