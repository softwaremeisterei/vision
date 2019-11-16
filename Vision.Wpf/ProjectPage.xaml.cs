using Microsoft.Expression.Interactivity.Core;
using Microsoft.Win32;
using mshtml;
using Softwaremeisterei.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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

        public ObservableCollection<LinkView> Links { get; set; }

        private Persistor persistor;

        private NavigationService _NavigationService;

        public string Url { get; set; }

        public ProjectPage(Project project)
        {
            InitializeComponent();
            DataContext = this;

            persistor = new Persistor();

            Project = project;

            Links = LinkMappers.MapToView(project.Links);

            TilesControl.Init(Links, Project);

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

                foreach (var linkId in Project.History)
                {
                    var linkView = Links.FirstOrDefault(n => n.Id == linkId);
                    if (linkView != null)
                    {
                        TilesControl.Model.HistoryLinks.Add(linkView);
                    }
                }

                Task.Run(() =>
                {
                    Worker();
                });

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

                var linkView = (LinkView)webBrowser.Tag;
                webBrowser.Tag = null;

                if (webBrowser.Document != null)
                {
                    var newName = ((dynamic)webBrowser.Document).Title;
                    linkView.Name = newName;
                    (linkView.Tag as Link).Name = newName;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void mnuAddLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var link = new Link
                {
                    Name = "Noname",
                };
                Project.Links.Add(link);

                var linkView = Global.Mapper.Map<LinkView>(link);
                Links.Add(linkView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenuLink_Edit(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var link = (LinkView)menuItem.Tag;
                Shared.EditLink(Window.GetWindow(this), link);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenuLink_AddLink(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var parentFolderView = (LinkView)menuItem.Tag;
                var linkView = Shared.AddNewLink(Window.GetWindow(this));
                Project.Links.Add(linkView.Tag as Link);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenuLink_Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var linkView = (LinkView)menuItem.Tag;
                Links.Remove(linkView);
                Project.Links.Remove(linkView.Tag as Link);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenuLink_ToggleFavorite(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var linkView = (LinkView)menuItem.Tag;
                Shared.ToggleFavorite(linkView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Link_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hyperlink = (Hyperlink)sender;
                var linkView = (LinkView)hyperlink.Tag;
                OpenLinkInWebBrowser(linkView);
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
                if (!string.IsNullOrEmpty(tbLinkUrl.Text))
                {
                    var builder = new UriBuilder(tbLinkUrl.Text);
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

        private void TilesControl_LinkClicked(LinkView linkView)
        {
            Project.History.Remove(linkView.Id);
            Project.History.Insert(0, linkView.Id);
            while (Project.History.Count > 7)
            {
                var lastIndex = Project.History.Count - 1;
                Project.History.RemoveAt(lastIndex);
            }
            OpenLinkInWebBrowser(linkView);
        }

        private void OpenLinkInWebBrowser(LinkView linkView)
        {
            try
            {
                var url = Urls.NormalizeUrl(linkView.Url);
                if (url != null)
                {
                    webBrowser.Tag = linkView;
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
            try
            {
            }
            catch { }
            UpdateEnablingWebBrowserNavButtons();
        }

        private void Worker()
        {
            while (true)
            {
                try
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        try
                        {
                            var doc = webBrowser.Document as IHTMLDocument2;

                            if (doc != null)
                            {
                                if (tbLinkUrl.Text != webBrowser.Source.AbsoluteUri)
                                    tbLinkUrl.Text = webBrowser.Source.AbsoluteUri;
                                if (tbLinkTitle.Text != doc.title)
                                    tbLinkTitle.Text = doc.title;
                            }
                        }
                        catch { }
                    }));

                    Thread.Sleep(500);
                }
                catch { }
            }
        }

        private void UpdateEnablingWebBrowserNavButtons()
        {
            btnBack.IsEnabled = webBrowser.CanGoBack;
            btnForward.IsEnabled = webBrowser.CanGoForward;
        }

        private void BtnAddLink_Click(object sender, RoutedEventArgs e)
        {
            TilesControl.AddNewLink(tbLinkTitle.Text, tbLinkUrl.Text);
        }
    }
}
