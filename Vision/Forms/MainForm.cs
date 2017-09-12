using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vision.Forms
{
    public partial class MainForm : Form
    {
        private static MainForm _instance;

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

            dockContainer1.LeftPanelWidth = 500;

            OpenExplorer();
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
            OpenBrowser();
        }

        private ProjectExplorerToolWindow OpenExplorer()
        {
            var explorer = new ProjectExplorerToolWindow();
            dockContainer1.DockToolWindow(explorer, global::Docking.Controls.DockMode.Left);
            explorer.Show();
            return explorer;
        }

        public BrowserToolWindow OpenBrowser()
        {
            var browserForm = new BrowserToolWindow();
            dockContainer1.DockToolWindow(browserForm, global::Docking.Controls.DockMode.Fill);
            browserForm.Show();
            return browserForm;
        }
    }
}
