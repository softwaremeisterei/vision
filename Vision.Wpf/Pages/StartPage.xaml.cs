using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Vision.BL;
using Vision.BL.Model;

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public ObservableCollection<RecentProject> RecentProjects { get; set; }

        private Persistor persistor;
        private ProjectService sharedServices;

        public StartPage()
        {
            InitializeComponent();

            persistor = new Persistor();
            sharedServices = new ProjectService();

            // Get recent projects
            var recentProjects = persistor.LoadRecentProjects();
            RecentProjects = new ObservableCollection<RecentProject>(recentProjects.Projects.OrderByDescending(p => p.LastUsageDate));
            icRecentProjects.ItemsSource = RecentProjects;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FocusFirstRecentProjectLink();
        }

        private void FocusFirstRecentProjectLink()
        {
            if (icRecentProjects.Items.Count > 0)
            {
                var presenter = (ContentPresenter)icRecentProjects.ItemContainerGenerator.ContainerFromIndex(0);
                var hyperLink = (Hyperlink)presenter.ContentTemplate.FindName("linkProject", presenter);
                hyperLink.Focus();
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
                if (sharedServices.PromptYesNoCancel("File not found", "Remove from the project list?") == MessageBoxResult.Yes)
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

        private void btnCreateNewProject_Click(object sender, RoutedEventArgs e)
        {
            var filePath = sharedServices.CreateNewProject(NavigationService);

            if (filePath != null)
            {
                persistor.AddRecentProject(filePath);
            }
        }

    }
}
