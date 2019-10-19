using Softwaremeisterei.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Vision.BL;
using Vision.BL.Model;

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for ProjectPage.xaml
    /// </summary>
    public partial class ProjectPage : Page
    {
        public Project Project { get; set; }
        public ObservableCollection<FolderNode> Roots { get; set; }

        private Persistor persistor;

        private NavigationService _NavigationService;

        public ProjectPage(Project project)
        {
            InitializeComponent();
            DataContext = this;

            persistor = new Persistor();

            this.Project = project;
            this.Roots = new ObservableCollection<FolderNode>();
            this.Roots.Add(project.Root);

            this.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandAll(treeView1.Items.OfType<FolderNode>().ToList());

            _NavigationService = this.NavigationService;
            _NavigationService.Navigating += NavigationService_Navigating;

            HideScriptErrors(webBrowser, true);
        }

        public void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => HideScriptErrors(wb, hide); //In case we are to early
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _NavigationService.Navigating -= NavigationService_Navigating;
        }

        private void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            Save();
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Save();
        }

        private void ExpandAll(IList<FolderNode> folders)
        {
            foreach (FolderNode folder in folders)
            {
                var item = treeView1.ItemContainerGenerator.ContainerFromItem(folder) as TreeViewItem;
                item.ExpandSubtree();
            }
        }

        private void WebBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
        }

        private void mnuAddTopLevelFolder_Click(object sender, RoutedEventArgs e)
        {
            Roots.First().Folders.Add(new FolderNode { Name = "NONAME" });
        }

        private void mnuAddTopLevelNode_Click(object sender, RoutedEventArgs e)
        {
            Roots.First().Nodes.Add(new Node { Name = "Noname" });
        }

        private void ContextMenuFolder_Edit(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var folder = (FolderNode)menuItem.Tag;
            var dlg = new EditFolderWindow(folder);
            dlg.Owner = Window.GetWindow(this);
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (dlg.ShowDialog() == true)
            {
                //
            }
        }

        private void ContextMenuFolder_AddNode(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var folder = (FolderNode)menuItem.Tag;
            folder.Nodes.Add(new Node { Name = "Noname" });
        }

        private void ContextMenuNode_Edit(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var node = (Node)menuItem.Tag;
            Edit(node);
        }

        private void Node_Click(object sender, RoutedEventArgs e)
        {
            var node = (Node)((Hyperlink)sender).Tag;
            var url = Urls.NormalizeUrl(node.Url);
            if (url != null)
            {
                webBrowser.Navigate(url);
            }
        }


        private void Save()
        {
            persistor.SaveProject(Project);
        }

        private void Edit(Node node)
        {
            var dlg = new EditNodeWindow(node);
            dlg.Owner = Window.GetWindow(this);
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (dlg.ShowDialog() == true)
            {
                //
            }
        }

    }
}
