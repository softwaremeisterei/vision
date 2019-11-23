using Microsoft.Expression.Interactivity.Core;
using Microsoft.Win32;
using mshtml;
using Softwaremeisterei.Lib;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Timers;
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

        public string Url { get; set; }

        public string PageTitle { get; set; }

        public ProjectPage(Project project)
        {
            InitializeComponent();
            DataContext = this;

            persistor = new Persistor();

            Project = project;

            var linkViews = LinkMappers.MapToView(project.Links);
            linkViews = new ObservableCollection<LinkView>(linkViews.OrderBy(link => link.Name));

            Links = linkViews;

            TilesControl.Init(Links, Project);
        }


        private Persistor persistor;

        private NavigationService _NavigationService;

        private Timer timer;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = Window.GetWindow(this);

                this.NavigationService.RemoveBackEntry();
                _NavigationService = this.NavigationService;
                _NavigationService.Navigating += NavigationService_Navigating;

                ApplyLayout();

                UpdateEnablingWebBrowserNavButtons();
                HideScriptErrors(webBrowser, true);

                window.SizeChanged += new SizeChangedEventHandler(Window_SizeChanged);
                window.Closing += new CancelEventHandler(Window_Closing);

                webBrowser.LoadCompleted += webBrowser_LoadCompleted;
                TilesControl.DataChanged += (_1, _2) => { Save(); };

                foreach (var linkId in Project.History)
                {
                    var linkView = Links.FirstOrDefault(n => n.Id == linkId);
                    if (linkView != null)
                    {
                        TilesControl.Model.HistoryLinks.Add(linkView);
                    }
                }

                TilesControl.Focus();

                timer = new Timer(500);
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    var doc = webBrowser?.Document as IHTMLDocument2;
                    if (doc != null)
                    {
                        PageTitle = doc.title;
                        NotifyPropertyChanged(nameof(PageTitle));
                    }
                });
            }
            catch { }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                timer.Stop();
                Save();
                Environment.Exit(Environment.ExitCode);
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
                var doc = webBrowser.Document as IHTMLDocument2;

                if (doc != null)
                {
                    var newName = doc.title;
                    linkView.Name = newName;
                    (linkView.Tag as Link).Name = newName;
                    PageTitle = newName;
                    Url = doc.url;
                    NotifyPropertyChanged(nameof(PageTitle));
                    NotifyPropertyChanged(nameof(Url));
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void mnuImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new OpenFileDialog();
                dlg.Title = "Import";

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
                if (!string.IsNullOrEmpty(Url))
                {
                    webBrowser.Tag = null;
                    var builder = new UriBuilder(Url);
                    webBrowser.Navigate(builder.Uri);
                }
            }
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

        private void WebBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            SetPageTitle(e.Uri.AbsoluteUri);
        }

        private void SetPageTitle(string uri)
        {
            try
            {
                var doc = webBrowser.Document as IHTMLDocument2;
                Url = uri;
                PageTitle = string.Empty;
                NotifyPropertyChanged(nameof(Url));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            SetPageTitle(e.Uri.AbsoluteUri);
            UpdateEnablingWebBrowserNavButtons();
        }

        private void BtnAddLink_Click(object sender, RoutedEventArgs e)
        {
            TilesControl.AddNewLink(PageTitle, Url);
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

        private void OpenLinkInWebBrowser(LinkView linkView)
        {
            try
            {
                var url = Urls.NormalizeUrl(linkView.Url);
                if (url != null)
                {
                    linkView.Url = url;
                    webBrowser.Tag = linkView;
                    webBrowser.Navigate(url);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateEnablingWebBrowserNavButtons()
        {
            btnBack.IsEnabled = webBrowser.CanGoBack;
            btnForward.IsEnabled = webBrowser.CanGoForward;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private void Save()
        {
            persistor.SaveProject(Project);
        }

    }
}
