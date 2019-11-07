using Softwaremeisterei.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Vision.BL;
using Vision.BL.Model;
using Vision.Wpf.Mappers;

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        private Persistor persistor;
        private SharedServices sharedServices;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            persistor = new Persistor();
            sharedServices = new SharedServices();

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
    }
}
