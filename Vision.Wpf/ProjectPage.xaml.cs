﻿using Microsoft.Expression.Interactivity.Core;
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
using System.Windows.Media;
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
        public NodeView Root { get; set; }

        private Persistor persistor;

        private NavigationService _NavigationService;

        public string Url { get; set; }

        public ProjectPage(Project project)
        {
            InitializeComponent();
            DataContext = this;

            persistor = new Persistor();

            this.Project = project;

            this.Root = NodeMappers.MapToView(project.Root);
            this.TilesControl.Init(this.Root);

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

                var window = Window.GetWindow(this);
                window.SizeChanged += new SizeChangedEventHandler(Window_SizeChanged);
                window.Closing += new CancelEventHandler(Window_Closing);

                _NavigationService = this.NavigationService;
                _NavigationService.Navigating += NavigationService_Navigating;
                HideScriptErrors(webBrowser, true);

                Root.Nodes.ToList().ForEach(nodeView =>
                {
                    var item = treeView1.ItemContainerGenerator.ContainerFromItem(nodeView) as TreeViewItem;
                    ApplyLayout(item, nodeView);
                });

                webBrowser.LoadCompleted += webBrowser_LoadCompleted;

                UpdateEnablingWebBrowserNavButtons();

                treeView1.Focus();
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

                treeView1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyLayout(TreeViewItem item, NodeView node)
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

            if (item != null && node != null)
            {
                if (Project.Layout.ExpandedNodes.Contains(node.Id))
                {
                    item.IsExpanded = true;
                    item.UpdateLayout(); // needed, otherwise ContainerFromItem(childnode) will return null
                    item.UpdateLayout(); // needed, otherwise ContainerFromItem(childnode) will return null

                    foreach (var childNode in node.Nodes)
                    {
                        var childItem = item.ItemContainerGenerator.ContainerFromItem(childNode) as TreeViewItem;
                        ApplyLayout(childItem as TreeViewItem, childNode);
                    }
                }
            }
        }

        private void UpdateLayoutSize()
        {
            Project.Layout.WindowWidth = this.WindowWidth;
            Project.Layout.WindowHeight = this.WindowHeight;
            Project.Layout.IsMaximized = Window.GetWindow(this).WindowState == WindowState.Maximized;
        }

        private void UpdateLayoutExpandedNodes(TreeViewItem item, NodeView node)
        {
            if (item != null)
            {
                if (item.IsExpanded)
                {
                    Project.Layout.ExpandedNodes.Add(node.Id);
                }

                foreach (var childNode in node.Nodes)
                {
                    UpdateLayoutExpandedNodes(item.ItemContainerGenerator.ContainerFromItem(childNode) as TreeViewItem, childNode);
                }
            }
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

        private void mnuAddTopLevelFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var node = new Node
                {
                    Name = "NONAME",
                    NodeType = BL.Model.NodeType.Folder,
                };
                (Root.Tag as Node).Nodes.Add(node);

                var nodeView = Global.Mapper.Map<NodeView>(node);
                Root.Nodes.Add(nodeView);
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
                    NodeType = BL.Model.NodeType.Link,
                };
                (Root.Tag as Node).Nodes.Add(node);

                var nodeView = Global.Mapper.Map<NodeView>(node);
                Root.Nodes.Add(nodeView);
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
                Shared.AddNewNode(Window.GetWindow(this), parentFolderView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenuNode_AddFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var parentFolderNodeView = (NodeView)menuItem.Tag;
                Shared.AddNewFolder(Window.GetWindow(this), parentFolderNodeView);
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
                var parentNodeView = GetParentFolder(nodeView);
                Shared.DeleteNode(parentNodeView, nodeView);
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
            Project.Layout.ExpandedNodes.Clear();
            Root.Nodes.ToList().ForEach(nodeView => UpdateLayoutExpandedNodes(treeView1.ItemContainerGenerator.ContainerFromItem(nodeView) as TreeViewItem, nodeView));
            persistor.SaveProject(Project);
        }

        private void mnuImportOldFormatProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new PromptWindow("Import old format project", "Path to project file");

                if (dlg.ShowDialog() == true)
                {
                    var migration = new Migration1();
                    migration.Migrate(dlg.ResponseText, Project);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// TREEVIEW DRAG & DROP ///

        Point _dragStartPoint;
        bool _IsTreeNodeDragging = false;
        readonly string DragDataFormat = "DDF000";

        private void TreeView1_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed ||
                    e.RightButton == MouseButtonState.Pressed && !_IsTreeNodeDragging)
                {
                    var position = e.GetPosition(null);
                    if (Math.Abs(position.X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance
                        || Math.Abs(position.Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        StartDrag(e);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TreeView1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _dragStartPoint = e.GetPosition(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TreeView1_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                var source = e.Data.GetData(DragDataFormat);
                var targetTreeViewItem = GetNearestContainer(e.OriginalSource as UIElement);
                e.Effects = (source != null && targetTreeViewItem?.Header != null && targetTreeViewItem.Header.GetType() == source.GetType())
                        ? DragDropEffects.Move : DragDropEffects.None;
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartDrag(MouseEventArgs e)
        {
            _IsTreeNodeDragging = true;
            object selectedItem = this.treeView1.SelectedItem;
            if (selectedItem != null)
            {
                DataObject data = null;
                data = new DataObject(DragDataFormat, selectedItem);

                DragDropEffects dde = DragDropEffects.Move;
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    dde = DragDropEffects.All;
                }
                DragDropEffects de = DragDrop.DoDragDrop(this.treeView1, data, dde);
            }
            _IsTreeNodeDragging = false;
        }

        private void TreeView1_Drop(object sender, DragEventArgs e)
        {
            try
            {
                var source = e.Data.GetData(DragDataFormat);
                var target = GetNearestContainer(e.OriginalSource as UIElement);

                if (source == null || target == null) { return; }

                var targetNodeView = target.Header as NodeView;
                var sourceNodeView = source as NodeView;

                if (sourceNodeView == targetNodeView) { return; }

                if (targetNodeView.NodeType == NodeViewType.Folder) // drop on folder
                {
                    if (!IsSubNode(sourceNodeView, targetNodeView))
                    {
                        var parentSourceFolderView = GetParentFolder(sourceNodeView);
                        targetNodeView.Nodes.Insert(0, sourceNodeView);
                        parentSourceFolderView.Nodes.Remove(sourceNodeView);

                        (targetNodeView.Tag as Node).Nodes.Insert(0, sourceNodeView.Tag as Node);
                        (parentSourceFolderView.Tag as Node).Nodes.Remove(sourceNodeView.Tag as Node);
                    }
                }
                else if (targetNodeView.NodeType == NodeViewType.Link) // drop on link
                {
                    var parentSourceFolderView = GetParentFolder(sourceNodeView);
                    var parentTargetFolderView = GetParentFolder(targetNodeView);
                    var index = parentTargetFolderView.Nodes.IndexOf(targetNodeView);
                    parentSourceFolderView.Nodes.Remove(sourceNodeView);
                    parentTargetFolderView.Nodes.Insert(index, sourceNodeView);

                    (parentSourceFolderView.Tag as Node).Nodes.Remove(sourceNodeView.Tag as Node);
                    (parentTargetFolderView.Tag as Node).Nodes.Insert(index, sourceNodeView.Tag as Node);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsSubNode(NodeView node1, NodeView node2)
        {
            if (node1.Nodes.Contains(node2))
            {
                return true;
            }
            var parentOfNode2 = GetParentFolder(node2);
            if (parentOfNode2 != null)
            {
                return IsSubNode(node1, parentOfNode2);
            }
            return false;
        }

        private NodeView GetParentFolder(NodeView folder)
        {
            return GetParentFolder(Root.Nodes, folder);
        }

        private NodeView GetParentFolder(ObservableCollection<NodeView> folders, NodeView childFolder)
        {
            foreach (var folder in folders)
            {
                if (folder.Nodes.Contains(childFolder)) { return folder; }

                var result = GetParentFolder(folder.Nodes, childFolder);

                if (result != null) { return result; }
            }
            return null;
        }

        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
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

            var flatItems = FlattenTree(null, treeView1.Items);

            if (treeView1.SelectedItem != null)
            {
                var tvItem = treeView1
                       .ItemContainerGenerator
                       .ContainerFromItemRecursive(treeView1.SelectedItem);
                var index = flatItems.IndexOf(tvItem);
                index = (index + 1) % flatItems.Count; // start at next item
                flatItems = flatItems.Skip(index).Union(flatItems.Take(index)).ToList();
            }

            foreach (var tvItem in flatItems)
            {
                var nodeView = tvItem.Header as NodeView;
                if (nodeView.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    tvItem.IsSelected = true;
                    tvItem.BringIntoView();
                    break;
                }
            }
        }

        private List<TreeViewItem> FlattenTree(TreeViewItem parentTvItem, ItemCollection childItemCollection)
        {
            var result = new List<TreeViewItem>();

            if (parentTvItem != null)
            {
                result.Add(parentTvItem);
            }
            foreach (var childItem in childItemCollection)
            {
                var childTvItem =
                    parentTvItem != null
                    ? parentTvItem.ItemContainerGenerator.ContainerFromItem(childItem) as TreeViewItem
                    : treeView1.ItemContainerGenerator.ContainerFromItem(childItem) as TreeViewItem;
                childTvItem.IsExpanded = true;
                childTvItem.ExpandSubtree();
                var flatChildren = FlattenTree(childTvItem, childTvItem.Items);
                result.AddRange(flatChildren);
            }
            return result;
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
