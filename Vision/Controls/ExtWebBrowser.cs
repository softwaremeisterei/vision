using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        }

        internal void Navigate(string url)
        {
            urlTextBox.Text = url;
            webBrowser1.Navigate(url);
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
    }
}
