using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vision.Start
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Properties.Settings.Default.OpenProjectFiles == null)
            {
                Properties.Settings.Default.OpenProjectFiles = new System.Collections.Specialized.StringCollection();
            }

            if (Properties.Settings.Default.RecentProjectFiles == null)
            {
                Properties.Settings.Default.RecentProjectFiles = new System.Collections.Specialized.StringCollection();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Forms.MainForm.GetInstance());
        }
    }
}
