using Crom.Controls;
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

namespace Vision
{
    public partial class BrowserToolWindow : DockableToolWindow
    {
        public BrowserToolWindow()
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
    }
}
