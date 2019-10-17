using System;
using System.Collections.Generic;
using System.IO;
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
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Properties.Settings.Default.OpenProjectFiles == null)
            {
                Properties.Settings.Default.OpenProjectFiles = new System.Collections.Specialized.StringCollection();
            }

            if (Properties.Settings.Default.RecentProjectFiles == null)
            {
                Properties.Settings.Default.RecentProjectFiles = new System.Collections.Specialized.StringCollection();
            }

            if (args.Any())
            {
                var projectFileName = args[0];
                if (projectFileName.EndsWith(".visx") && File.Exists(projectFileName))
                {
                    Forms.MainForm.AddToRecentProjectFiles(projectFileName);
                }
            }

            Forms.MainForm.GetInstance().RestoreLastWindowLayout();
            Forms.MainForm.GetInstance().StartPosition = FormStartPosition.CenterScreen;

            Application.Run(Forms.MainForm.GetInstance());
        }
    }
}
