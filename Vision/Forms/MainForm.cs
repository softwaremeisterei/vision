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
    public partial class MainForm : Form
    {
        private static MainForm _instance;
        private bool _isLoaded;

        public MainForm()
        {
            _instance = this;
            InitializeComponent();
        }

        public static MainForm GetInstance()
        {
            return _instance;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            IsMdiContainer = true;

            if (Properties.Settings.Default.WindowSize.Height > 0)
            {
                Size = Properties.Settings.Default.WindowSize;
            }

            if (Properties.Settings.Default.WindowIsMaximized)
            {
                WindowState = FormWindowState.Maximized;
            }

            if (Properties.Settings.Default.LeftPanelWidth > 0)
            {
                dockContainer1.LeftPanelWidth = Properties.Settings.Default.LeftPanelWidth;
            }

            OpenExplorer();

            _isLoaded = true;
        }

        private void fileOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Vision projects (*.visionproj)|*.visionproj";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                var explorer = OpenExplorer();
                explorer.OpenProject(fileName);
            }
        }

        private void fileNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenExplorer();
        }

        private void viewBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWebBrowser();
        }

        private void dockContainer1_DragEnter(object sender, DragEventArgs e)
        {
        }

        private void dockContainer1_DragOver(object sender, DragEventArgs e)
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

        private void dockContainer1_DragDrop(object sender, DragEventArgs e)
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
                var window = OpenWebBrowser(DockMode.None, url);
                var location = dockContainer1.PointToClient(new Point(e.X - window.Width / 2, e.Y - window.TitleBarScreenBounds.Height / 2));
                window.Location = location;
            }
        }

        private ProjectExplorerToolWindow OpenExplorer()
        {
            var explorer = new ProjectExplorerToolWindow();
            dockContainer1.DockToolWindow(explorer, global::Docking.Controls.DockMode.Left);
            explorer.Show();
            return explorer;
        }

        public WebBrowserToolWindow OpenWebBrowser(DockMode dockMode = DockMode.Fill, string url = null)
        {
            var browserForm = new WebBrowserToolWindow();

            if (dockMode == DockMode.None)
            {
                dockContainer1.AddToolWindow(browserForm);
            }
            else
            {
                dockContainer1.DockToolWindow(browserForm, dockMode);
            }

            browserForm.Show();

            if (url != null)
            {
                browserForm.Navigate(url);
            }

            browserForm.FocusUrl();
            return browserForm;
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            Properties.Settings.Default.WindowSize = Size;
            Properties.Settings.Default.Save();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (_isLoaded)
            {
                Properties.Settings.Default.WindowIsMaximized = (WindowState == FormWindowState.Maximized);
                Properties.Settings.Default.Save();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.LeftPanelWidth = dockContainer1.LeftPanelWidth;
            Properties.Settings.Default.Save();
        }
    }
}
