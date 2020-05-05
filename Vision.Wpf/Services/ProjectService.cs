using Microsoft.Win32;
using Softwaremeisterei.Lib;
using System;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
using Vision.BL;
using Vision.BL.Model;

namespace Vision.Wpf
{
    class ProjectService
    {
        private readonly Persistor persistor;

        public ProjectService()
        {
            this.persistor = new Persistor();
        }

        public void OpenProject(NavigationService navigationService)
        {
            var dlg = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = Persistor.ProjectFileExtension,
                Filter = ""
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var xml = File.ReadAllText(dlg.FileName);
                    var project = Serialization.ParseXml<Project>(xml);
                    persistor.AddRecentProject(dlg.FileName);
                    navigationService.Navigate(new ProjectPage(project));
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("Loading failed", ex);
                }
            }
        }

        internal string CreateNewProject(NavigationService navigationService)
        {
            var dlg = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = Persistor.ProjectFileExtension,
                Filter = $"Project files (*.{Persistor.ProjectFileExtension})|*.{Persistor.ProjectFileExtension}"
            };

            if (dlg.ShowDialog() == true)
            {
                var name = Path.GetFileNameWithoutExtension(dlg.FileName);
                var project = new Project
                {
                    Name = name,
                    Path = dlg.FileName
                };
                persistor.SaveProject(project);
                navigationService.Navigate(new ProjectPage(project));
                return project.Path;
            }

            return null;
        }

        public void ShowErrorMessage(string caption, Exception ex)
        {
            ShowErrorMessage(caption, ex.Message);
        }

        public void ShowErrorMessage(string caption, string message)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public MessageBoxResult PromptYesNoCancel(string caption, string message)
        {
            return MessageBox.Show(message, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }
    }
}
