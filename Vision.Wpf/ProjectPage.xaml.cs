using Softwaremeisterei.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Vision.BL;
using Vision.BL.Model;
using Vision.Wpf.Model;

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for ProjectPage.xaml
    /// </summary>
    public partial class ProjectPage : Page
    {
        public Project Project { get; set; }
        public ObservableCollection<NodeView> Roots { get; set; }

        private Persistor persistor;

        private NavigationService _NavigationService;

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
            var result = Global.Mapper.Map<NodeView>(root);
            return result;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _NavigationService = this.NavigationService;
            _NavigationService.Navigating += NavigationService_Navigating;
            HideScriptErrors(webBrowser, true);

            Roots.ToList().ForEach(n =>
            {
                var item = treeView1.ItemContainerGenerator.ContainerFromItem(n) as TreeViewItem;
                ApplyLayout(item, n);
            });
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

        private void WebBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
        }

        private void mnuAddTopLevelFolder_Click(object sender, RoutedEventArgs e)
        {
            var node = new Node { Name = "NONAME", NodeType = BL.Model.NodeType.Folder };
            (Roots.First().Tag as Node).Nodes.Add(node);

            var nodeView = Global.Mapper.Map<NodeView>(node);
            Roots.First().Nodes.Add(nodeView);
        }

        private void mnuAddTopLevelNode_Click(object sender, RoutedEventArgs e)
        {
            var node = new Node { Name = "Noname", NodeType = BL.Model.NodeType.Link };
            (Roots.First().Tag as Node).Nodes.Add(node);

            var nodeView = Global.Mapper.Map<NodeView>(node);
            Roots.First().Nodes.Add(nodeView);
        }

        private void ContextMenuNode_Edit(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var node = (NodeView)menuItem.Tag;

            if (node.NodeType == NodeViewType.Folder)
            {
                var dlg = new EditFolderWindow(node)
                {
                    Owner = Window.GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                if (dlg.ShowDialog() == true)
                {
                    node.NotifyPropertyChanged(nameof(NodeView.Name));
                }
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
            var menuItem = (MenuItem)sender;
            var folder = (NodeView)menuItem.Tag;
            var node = new Node { Name = "Noname", NodeType = NodeType.Link };
            (folder.Tag as Node).Nodes.Add(node);
            var nodeView = Global.Mapper.Map<NodeView>(node);
            folder.Nodes.Add(nodeView);
        }

        private void ContextMenuNode_AddFolder(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var folder = (NodeView)menuItem.Tag;
            var node = new Node { Name = "Noname", NodeType = NodeType.Folder };
            (folder.Tag as Node).Nodes.Add(node);
            var nodeView = Global.Mapper.Map<NodeView>(node);
            folder.Nodes.Add(nodeView);
        }


        private void ContextMenuNode_Delete(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var folder = (NodeView)menuItem.Tag;
            DeleteNodeView(Roots, folder.Id);
            DeleteNode(Project.Root, folder.Id);
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
            var menuItem = (MenuItem)sender;
            var node = (NodeView)menuItem.Tag;
            node.IsFavorite = !node.IsFavorite;
            (node.Tag as Node).IsFavorite = !(node.Tag as Node).IsFavorite;
            node.ImageSource = node.IsFavorite ? Global.FavoriteStarUri : "";
            node.NotifyPropertyChanged(nameof(NodeView.IsFavorite));
            node.NotifyPropertyChanged(nameof(NodeView.ImageSource));
        }


        private void Node_Click(object sender, RoutedEventArgs e)
        {
            var node = (NodeView)((Hyperlink)sender).Tag;
            var url = Urls.NormalizeUrl(node.Url);
            if (url != null)
            {
                webBrowser.Navigate(url);
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
            var dlg = new PromptWindow("Import old format project", "Path to project file");

            if (dlg.ShowDialog() == true)
            {
                var migration = new Migration1();
                migration.Migrate(dlg.ResponseText, Project);
            }
        }

        /// TREEVIEW DRAG & DROP ///

        Point _dragStartPoint;
        bool _IsTreeNodeDragging = false;
        readonly string DragDataFormat = "DDF000";

        private void TreeView1_PreviewMouseMove(object sender, MouseEventArgs e)
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

        private void TreeView1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void TreeView1_DragOver(object sender, DragEventArgs e)
        {
            var source = e.Data.GetData(DragDataFormat);
            var targetTreeViewItem = GetNearestContainer(e.OriginalSource as UIElement);
            e.Effects = (source != null && targetTreeViewItem?.Header != null && targetTreeViewItem.Header.GetType() == source.GetType())
                    ? DragDropEffects.Move : DragDropEffects.None;
            e.Handled = true;
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
                        }
                    }
                }
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

    }
}
