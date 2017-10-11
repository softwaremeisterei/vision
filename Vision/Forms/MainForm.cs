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
using Vision.BL;
using Vision.BL.Model;

namespace Vision.Forms
{
    public partial class MainForm : Form
    {
        private static readonly MainForm _instance;
        private bool _isLoaded;

        static MainForm()
        {
            _instance = new MainForm();
        }

        private MainForm()
        {
            InitializeComponent();
        }

        public static MainForm GetInstance()
        {
            return _instance;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            IsMdiContainer = true;
            RestoreLastWindowLayout();

            AddRecentProjectFilesToMenu();

            OpenLastProjects();

            _isLoaded = true;
        }

        private void AddRecentProjectFilesToMenu()
        {
            foreach (var fileName in Properties.Settings.Default.RecentProjectFiles)
            {
                var item = new ToolStripMenuItem()
                {
                    Text = fileName
                };
                item.Click += delegate { OpenExplorer(fileName); };
                fileToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        private void OpenLastProjects()
        {
            if (Properties.Settings.Default.OpenProjectFiles == null)
            {
                return;
            }

            foreach (var projectFile in Properties.Settings.Default.OpenProjectFiles)
            {
                var explorer = OpenExplorer(projectFile);
            }
        }

        private void RestoreLastWindowLayout()
        {
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
        }

        private void fileOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Vision projects (*.visionproj)|*.visionproj";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                var explorer = OpenExplorer(fileName);

                if (!explorer.IsIncognito())
                {
                    AddToRecentProjectFiles(fileName);
                }
            }
        }

        private static void AddToRecentProjectFiles(string fileName)
        {
            Properties.Settings.Default.RecentProjectFiles.Remove(fileName);
            Properties.Settings.Default.RecentProjectFiles.Insert(0, fileName);
            Properties.Settings.Default.Save();
        }

        private void fileNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Vision projects (*.visionproj)|*.visionproj";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            var dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult == DialogResult.Cancel)
            {
                return;
            }

            if (dialogResult == DialogResult.OK)
            {
                var explorer = OpenExplorer(saveFileDialog.FileName);
                AddToRecentProjectFiles(saveFileDialog.FileName);
            }
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

        private ProjectExplorerToolWindow OpenExplorer(string fileName)
        {
            var explorer = new ProjectExplorerToolWindow(fileName);
            dockContainer1.DockToolWindow(explorer, global::Docking.Controls.DockMode.Left);
            explorer.Show();
            explorer.IncognitoChanged += Explorer_IncognitoChanged;
            return explorer;
        }

        private void Explorer_IncognitoChanged(object sender, EventArgs e)
        {
            var context = (Context)sender;

            if (context.Incognito)
            {
                Properties.Settings.Default.RecentProjectFiles.Remove(context.FileName);
                Properties.Settings.Default.Save();
            }
        }

        public WebBrowserToolWindow OpenWebBrowser(DockMode dockMode = DockMode.Fill, string url = null)
        {
            var window = new WebBrowserToolWindow();

            DockAndShow(dockMode, window);

            if (url != null)
            {
                window.Navigate(url);
            }

            window.FocusUrl();
            return window;
        }

        public ImageViewerToolWindow OpenImageViewer(DockMode dockMode = DockMode.Fill, Image image = null)
        {
            var window = new ImageViewerToolWindow();

            DockAndShow(dockMode, window);

            if (image != null)
            {
                window.SetImage(image);
            }

            return window;
        }


        private void DockAndShow(DockMode dockMode, ToolWindow window)
        {
            if (dockMode == DockMode.None)
            {
                dockContainer1.AddToolWindow(window);
            }
            else
            {
                dockContainer1.DockToolWindow(window, dockMode);
            }

            window.Show();
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
            Properties.Settings.Default.OpenProjectFiles.Clear();

            foreach (var child in dockContainer1.GetToolWindows())
            {
                if (child is ProjectExplorerToolWindow)
                {
                    var explorer = child as ProjectExplorerToolWindow;

                    if (!explorer.IsIncognito())
                    {
                        Properties.Settings.Default.OpenProjectFiles.Add(explorer.FileName);
                    }
                }

                if (!child.TryClose())
                {
                    e.Cancel = true;
                    return;
                }
            }

            Properties.Settings.Default.Save();


            Properties.Settings.Default.LeftPanelWidth = dockContainer1.LeftPanelWidth;
            Properties.Settings.Default.Save();
        }

        internal void RedrawDockContainerTabButtons()
        {
            dockContainer1.RedrawDrawTabButtons();
        }

        internal bool FindWebBrowserAndShow(string url)
        {
            foreach (var webBrowserToolwindow in dockContainer1.GetToolWindows().OfType<WebBrowserToolWindow>())
            {
                if (webBrowserToolwindow.GetUrl().Equals(url, StringComparison.OrdinalIgnoreCase))
                {
                    dockContainer1.SelectToolWindow(webBrowserToolwindow);
                    return true;
                }
            }

            return false;
        }

    }
}
