using Softwaremeisterei.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using Vision.BL;
using Vision.BL.Model;
using Vision.Wpf.Converters;

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public ObservableCollection<RecentProject> RecentProjects { get; set; }

        private Persistor persistor;
        private SharedServices sharedServices;

        public StartPage()
        {
            InitializeComponent();

            persistor = new Persistor();
            sharedServices = new SharedServices();

            // Get recent projects
            var recentProjects = persistor.LoadRecentProjects();
            RecentProjects = new ObservableCollection<RecentProject>(recentProjects.Projects.OrderBy(p => p.LastUsageDate));
            icRecentProjects.ItemsSource = RecentProjects;
        }

        private void btnCreateNewProject_Click(object sender, RoutedEventArgs e)
        {
            var filePath = sharedServices.CreateNewProject(NavigationService);
            if (filePath != null)
            {
                persistor.AddRecentProject(filePath);
            }
        }

        private void projectLink_Click(object sender, RoutedEventArgs e)
        {
            var hyperLink = sender as Hyperlink;
            var recentProject = hyperLink.Tag as RecentProject;

            if (File.Exists(recentProject.Path))
            {
                recentProject.LastUsageDate = DateTime.Now;
                persistor.UpateRecentProject(recentProject);
                var project = persistor.LoadProject(recentProject.Path);
                NavigationService.Navigate(new ProjectPage(project));
            }
            else
            {
                if (sharedServices.AskYesNoCancel("File not found", "Remove this entry from recent project list?") == MessageBoxResult.Yes)
                {
                    persistor.RemoveRecentProject(recentProject);
                    RecentProjects.Remove(recentProject);
                }
            }
        }

        private void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            sharedServices.OpenProject(NavigationService);
        }


    }
}
