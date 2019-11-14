using Microsoft.Expression.Interactivity.Core;
using Microsoft.Win32;
using Softwaremeisterei.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using Vision.BL;
using Vision.BL.Lib;
using Vision.BL.Model;
using Vision.Wpf.Mappers;
using Vision.Wpf.Model;

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for ProjectPage.xaml
    /// </summary>
    public partial class ProjectPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Project Project { get; set; }
        public ObservableCollection<NodeView> Nodes { get; set; }

        private Persistor persistor;

        private NavigationService _NavigationService;

        public string Url { get; set; }

        public ProjectPage(Project project)
        {
            InitializeComponent();
            DataContext = this;

            persistor = new Persistor();

            this.Project = project;

            this.Nodes = NodeMappers.MapToView(project.Nodes);
            this.TilesControl.Init(Nodes, Project);

            InputBindings.Add(new KeyBinding(new ActionCommand(() => { Search(); }),
                Key.F, ModifierKeys.Control));
            InputBindings.Add(new KeyBinding(new ActionCommand(() => { if (searchText == null) Search(); else FindNext(); }),
                Key.F3, ModifierKeys.None));

        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.NavigationService.RemoveBackEntry();
                ApplyLayout();
                UpdateEnablingWebBrowserNavButtons();

                var window = Window.GetWindow(this);
                window.SizeChanged += new SizeChangedEventHandler(Window_SizeChanged);
                window.Closing += new CancelEventHandler(Window_Closing);
                webBrowser.LoadCompleted += webBrowser_LoadCompleted;
                TilesControl.DataChanged += (_1, _2) => { Save(); };
                _NavigationService = this.NavigationService;
                _NavigationService.Navigating += NavigationService_Navigating;
                HideScriptErrors(webBrowser, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLayoutSize();
        }

        private void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                if (webBrowser.Source == null || webBrowser.Tag == null)
                {
                    return;
                }

                var nodeView = (NodeView)webBrowser.Tag;
                webBrowser.Tag = null;

                if (webBrowser.Document != null)
                {
                    var newName = ((dynamic)webBrowser.Document).Title;
                    nodeView.Name = newName;
                    (nodeView.Tag as Node).Name = newName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyLayout()
        {
            var window = Window.GetWindow(this);

            if (Project.Layout.WindowWidth > 0 && Project.Layout.WindowHeight > 0)
            {
                window.Width = Project.Layout.WindowWidth;
                window.Height = Project.Layout.WindowHeight;
                CenterWindowOnScreen(window);
            }

            if (Project.Layout.IsMaximized)
            {
                window.WindowState = WindowState.Maximized;
            }
        }

        private void UpdateLayoutSize()
        {
            Project.Layout.WindowWidth = this.WindowWidth;
            Project.Layout.WindowHeight = this.WindowHeight;
            Project.Layout.IsMaximized = Window.GetWindow(this).WindowState == WindowState.Maximized;
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
            try
            {
                _NavigationService.Navigating -= NavigationService_Navigating;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            try
            {
                Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WebBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            try
            {
                Url = e.Uri.AbsoluteUri;
                NotifyPropertyChanged(nameof(Url));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void mnuAddTopLevelNode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var node = new Node
                {
                    Name = "Noname",
                };
                Project.Nodes.Add(node);

                var nodeView = Global.Mapper.Map<NodeView>(node);
                Nodes.Add(nodeView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenuNode_Edit(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var node = (NodeView)menuItem.Tag;
                Shared.EditNode(Window.GetWindow(this), node);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenuNode_AddNode(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var parentFolderView = (NodeView)menuItem.Tag;
                var nodeView = Shared.AddNewNode(Window.GetWindow(this));
                Project.Nodes.Add(nodeView.Tag as Node);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenuNode_Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var nodeView = (NodeView)menuItem.Tag;
                Nodes.Remove(nodeView);
                Project.Nodes.Remove(nodeView.Tag as Node);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenuNode_ToggleFavorite(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var nodeView = (NodeView)menuItem.Tag;
                Shared.ToggleFavorite(nodeView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Node_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hyperlink = (Hyperlink)sender;
                var nodeView = (NodeView)hyperlink.Tag;
                OpenLinkInWebBrowser(nodeView);
                var tvItem = WpfHelper.GetParentOfType<TreeViewItem>(hyperlink.Parent);
                if (tvItem != null)
                {
                    tvItem.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Save()
        {
            persistor.SaveProject(Project);
        }

        private void mnuImportOldFormatProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new OpenFileDialog();
                dlg.Title = "Import old format project";

                if (dlg.ShowDialog() == true)
                {
                    var migration = new Migration1();
                    var newNodes = new List<Node>();
                    migration.Migrate(dlg.FileName, newNodes);
                    newNodes.ForEach(node =>
                    {
                        Project.Nodes.Add(node);
                        var nodeView = Global.Mapper.Map<NodeView>(node);
                        nodeView.Tag = node;
                        Nodes.Add(nodeView);
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBoxUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrEmpty(tbUrl.Text))
                {
                    var builder = new UriBuilder(tbUrl.Text);
                    webBrowser.Tag = null;
                    webBrowser.Navigate(builder.Uri);
                }
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string searchText = null;

        private void Search()
        {
            try
            {
                var dlg = new PromptWindow("Search", "Search text")
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                if (dlg.ShowDialog() == true)
                {
                    searchText = dlg.ResponseText;
                    FindNext();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FindNext()
        {
            if (searchText == null) { return; }

            TilesControl.FindNext(searchText);
        }

        private void mnuSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void mnuFindNext_Click(object sender, RoutedEventArgs e)
        {
            FindNext();
        }

        private void CenterWindowOnScreen(Window window)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = window.Width;
            double windowHeight = window.Height;
            window.Left = (screenWidth / 2) - (windowWidth / 2);
            window.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void TilesControl_LinkClicked(NodeView nodeView)
        {
            OpenLinkInWebBrowser(nodeView);
        }

        private void OpenLinkInWebBrowser(NodeView nodeView)
        {
            try
            {
                var url = Urls.NormalizeUrl(nodeView.Url);
                if (url != null)
                {
                    webBrowser.Tag = nodeView;
                    webBrowser.Navigate(url);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (webBrowser.CanGoBack)
            {
                webBrowser.GoBack();
            }
        }

        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (webBrowser.CanGoForward)
            {
                webBrowser.GoForward();
            }
        }

        private void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            UpdateEnablingWebBrowserNavButtons();
        }

        private void UpdateEnablingWebBrowserNavButtons()
        {
            btnBack.IsEnabled = webBrowser.CanGoBack;
            btnForward.IsEnabled = webBrowser.CanGoForward;
        }
    }
}
