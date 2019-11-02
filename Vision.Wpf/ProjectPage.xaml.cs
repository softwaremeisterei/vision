﻿using Softwaremeisterei.Lib;
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
        public ObservableCollection<NodeView> Roots { get; set; }

        private Persistor persistor;

        private NavigationService _NavigationService;

        public string Url { get; set; }

        public ProjectPage(Project project)
        {
            InitializeComponent();
            DataContext = this;

            persistor = new Persistor();

            this.Project = project;
            this.Roots = new ObservableCollection<NodeView>();
            this.Roots.Add(MapToView(project.Root));

            this.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private NodeView MapToView(Node root)
        {
            var mapper = Global.Mapper;
            var result = mapper.Map<NodeView>(root);
            return result;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _NavigationService = this.NavigationService;
                _NavigationService.Navigating += NavigationService_Navigating;
                HideScriptErrors(webBrowser, true);

                Roots.ToList().ForEach(n =>
                {
                    var item = treeView1.ItemContainerGenerator.ContainerFromItem(n) as TreeViewItem;
                    ApplyLayout(item, n);
                });
                WebBrowser wb = webBrowser;
                webBrowser.LoadCompleted += webBrowser_LoadCompleted;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            if (item != null && node != null)
            {
                if (Project.Layout.ExpandedNodes.Contains(node.Id))
                {
                    item.IsExpanded = true;
                    item.UpdateLayout(); // needed, otherwise ContainerFromItem(childnode) will return null

                    foreach (var childNode in node.Nodes)
                    {
                        var childItem = item.ItemContainerGenerator.ContainerFromItem(childNode) as TreeViewItem;
                        ApplyLayout(childItem as TreeViewItem, childNode);
                    }
                }
            }
        }

        private void UpdateLayoutRec(TreeViewItem item, NodeView node)
        {
            if (item != null)
            {
                if (item.IsExpanded)
                {
                    Project.Layout.ExpandedNodes.Add(node.Id);
                }

                foreach (var childNode in node.Nodes)
                {
                    UpdateLayoutRec(item.ItemContainerGenerator.ContainerFromItem(childNode) as TreeViewItem, childNode);
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

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
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
                var node = new Node { Name = "NONAME", NodeType = BL.Model.NodeType.Folder };
                (Roots.First().Tag as Node).Nodes.Add(node);

                var nodeView = Global.Mapper.Map<NodeView>(node);
                Roots.First().Nodes.Add(nodeView);
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
                var node = new Node { Name = "Noname", NodeType = BL.Model.NodeType.Link };
                (Roots.First().Tag as Node).Nodes.Add(node);

                var nodeView = Global.Mapper.Map<NodeView>(node);
                Roots.First().Nodes.Add(nodeView);
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

                EditNode(node);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditNode(NodeView node)
        {
            if (node.NodeType == NodeViewType.Folder)
            {
                var dlg = new EditFolderWindow(node)
                {
                    Owner = Window.GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                if (dlg.ShowDialog() == true)
                { }
            }
            else if (node.NodeType == NodeViewType.Link)
            {
                var dlg = new EditNodeWindow(node)
                {
                    Owner = Window.GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                dlg.ShowDialog();
            }
        }

        private void ContextMenuNode_AddNode(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var parentFolderView = (NodeView)menuItem.Tag;
                var newNode = new Node { Name = "Noname", NodeType = NodeType.Link };
                (parentFolderView.Tag as Node).Nodes.Add(newNode);
                var newNodeView = Global.Mapper.Map<NodeView>(newNode);
                parentFolderView.Nodes.Add(newNodeView);
                EditNode(newNodeView);
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
                var parentFolderView = (NodeView)menuItem.Tag;
                var newFolder = new Node { Name = "Noname", NodeType = NodeType.Folder };
                (parentFolderView.Tag as Node).Nodes.Add(newFolder);
                var newFolderView = Global.Mapper.Map<NodeView>(newFolder);
                parentFolderView.Nodes.Add(newFolderView);
                EditNode(newFolderView);
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
                var folder = (NodeView)menuItem.Tag;
                DeleteNodeView(Roots, folder.Id);
                DeleteNode(Project.Root, folder.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool DeleteNode(Node root, Guid nodeId)
        {
            var matchingNode = root.Nodes.FirstOrDefault(n => n.Id == nodeId);

            if (matchingNode != null)
            {
                root.Nodes.Remove(matchingNode);
                return true;
            }
            else
            {
                foreach (var node in root.Nodes)
                {
                    if (DeleteNode(node, nodeId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool DeleteNodeView(ObservableCollection<NodeView> nodes, Guid nodeId)
        {
            var matchingNode = nodes.FirstOrDefault(n => n.Id == nodeId);

            if (matchingNode != null)
            {
                nodes.Remove(matchingNode);
                return true;
            }
            else
            {
                foreach (var node in nodes)
                {
                    if (DeleteNodeView(node.Nodes, nodeId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ContextMenuNode_ToggleFavorite(object sender, RoutedEventArgs e)
        {
            try
            {
                var menuItem = (MenuItem)sender;
                var node = (NodeView)menuItem.Tag;
                node.IsFavorite = !node.IsFavorite;
                (node.Tag as Node).IsFavorite = !(node.Tag as Node).IsFavorite;
                node.ImageSource = node.IsFavorite ? Global.FavoriteStarUri : "";
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
                var node = (NodeView)hyperlink.Tag;
                var url = Urls.NormalizeUrl(node.Url);
                if (url != null)
                {
                    webBrowser.Tag = node;
                    webBrowser.Navigate(url);
                }
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
            Roots.ToList().ForEach(node => UpdateLayoutRec(treeView1.ItemContainerGenerator.ContainerFromItem(node) as TreeViewItem, node));
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
                if (source is NodeView)
                {
                    var sourceEntity = source as NodeView;
                    var dropItem = GetNearestContainer(e.OriginalSource as UIElement);
                    if (dropItem != null)
                    {
                        if (dropItem.Header is NodeView)
                        {
                            var dropEntity = dropItem.Header as NodeView;
                            if (source != dropEntity && !IsSubNode(sourceEntity, dropEntity))
                            {
                                var parentSourceFolder = GetParentFolder(sourceEntity);
                                dropEntity.Nodes.Insert(0, sourceEntity);
                                parentSourceFolder.Nodes.Remove(sourceEntity);

                                (dropEntity.Tag as Node).Nodes.Insert(0, sourceEntity.Tag as Node);
                                (parentSourceFolder.Tag as Node).Nodes.Remove(sourceEntity.Tag as Node);
                            }
                        }
                    }
                }
                else if (source is NodeView)
                {
                    var sourceEntity = source as NodeView;
                    var dropItem = GetNearestContainer(e.OriginalSource as UIElement);
                    if (dropItem != null)
                    {
                        if (dropItem.Header is NodeView)
                        {
                            var dropEntity = dropItem.Header as NodeView;
                            if (source != dropEntity)
                            {
                                var parentSourceFolder = GetParentFolder(sourceEntity);
                                dropEntity.Nodes.Insert(0, sourceEntity);
                                parentSourceFolder.Nodes.Remove(sourceEntity);

                                (dropEntity.Tag as Node).Nodes.Insert(0, sourceEntity.Tag as Node);
                                (parentSourceFolder.Tag as Node).Nodes.Remove(sourceEntity.Tag as Node);
                            }
                        }
                    }
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
            return GetParentFolder(Roots, folder);
        }

        private NodeView GetParentFolder(ObservableCollection<NodeView> folders, NodeView childFolder)
        {
            foreach (var folder in folders)
            {
                if (folder.Nodes.Contains(childFolder))
                {
                    return folder;
                }
                var result = GetParentFolder(folder.Nodes, childFolder);
                if (result != null)
                {
                    return result;
                }
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

                    webBrowser.Navigate(builder.Uri);
                }
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
