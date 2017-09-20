using Docking.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.BL.Model;

namespace Vision.Forms
{
    public partial class WebBrowserToolWindow : ToolWindow
    {
        public WebBrowserToolWindow()
        {
            InitializeComponent();
        }

        internal void Navigate(string url)
        {
            extWebBrowser1.Navigate(url);
        }

        internal void SetDocumentCompletedHandler(WebBrowserDocumentCompletedEventHandler handler)
        {
            extWebBrowser1.WebBrowser.DocumentCompleted += handler;
        }

        internal void SetTag(Object tag)
        {
            extWebBrowser1.WebBrowser.Tag = tag;
        }

        internal void FocusUrl()
        {
            extWebBrowser1.FocusUrl();
        }

        private void extWebBrowser1_DragOver(object sender, DragEventArgs e)
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

        private void extWebBrowser1_DragDrop(object sender, DragEventArgs e)
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
