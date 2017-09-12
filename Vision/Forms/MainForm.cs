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

            var dockableExplorer = new ExplorerToolWindow();
            dockContainer1.DockToolWindow(dockableExplorer, global::Docking.Controls.DockMode.Left);
            dockableExplorer.Show();
            dockContainer1.LeftPanelWidth = 500;
        }

        public BrowserToolWindow OpenBrowserForm(string title, string url)
        {
            var browserForm = new BrowserToolWindow();
            browserForm.Text = title;
            dockContainer1.DockToolWindow(browserForm, global::Docking.Controls.DockMode.Fill);
            browserForm.Show();
            return browserForm;
        }
    }
}
