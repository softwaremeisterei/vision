using System.Reflection;
using System.Windows;
using System.Windows.Navigation;
using Vision.BL;

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        private Persistor persistor;
        private ProjectService sharedServices;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            persistor = new Persistor();
            sharedServices = new ProjectService();

            Title = "Vision " + Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
        }

        private void mnuFileNewProject_Click(object sender, RoutedEventArgs e)
        {
            var filePath = sharedServices.CreateNewProject(NavigationService);

            if (filePath != null)
            {
                persistor.AddRecentProject(filePath);
            }
        }

        private void mnuFileOpenProject_Click(object sender, RoutedEventArgs e)
        {
            sharedServices.OpenProject(NavigationService);
        }

        private void NavigationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            frame.Navigate(new StartPage());
        }

        private void mnuGotoStart_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new StartPage());
        }
    }
}
