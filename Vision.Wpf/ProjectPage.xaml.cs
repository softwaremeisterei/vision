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

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for ProjectPage.xaml
    /// </summary>
    public partial class ProjectPage : Page
    {
        public Project Project { get; set; }
        public ObservableCollection<Node> Roots { get; set; }

        private Persistor persistor;

        private NavigationService _NavigationService;

        public ProjectPage(Project project)
        {
            InitializeComponent();
            DataContext = this;

            persistor = new Persistor();

            this.Project = project;
            this.Roots = new ObservableCollection<Node>();
            this.Roots.Add(project.Root);

            this.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandAll(treeView1.Items.OfType<Node>().ToList());

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

        private void ExpandAll(IList<Node> folders)
        {
            foreach (Node folder in folders)
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
            Roots.First().Nodes.Add(new Node { Name = "NONAME" });
        }

        private void mnuAddTopLevelNode_Click(object sender, RoutedEventArgs e)
        {
            Roots.First().Nodes.Add(new Node { Name = "Noname" });
        }

        private void ContextMenuNode_Edit(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var node = (Node)menuItem.Tag;

            if (node.NodeType == NodeType.Folder)
            {
                var dlg = new EditFolderWindow(node)
                {
                    Owner = Window.GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                dlg.ShowDialog();
            }
            else if (node.NodeType == NodeType.Link)
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
            var folder = (Node)menuItem.Tag;
            folder.Nodes.Add(new Node { Name = "Noname", NodeType = NodeType.Link });
        }

        private void ContextMenuNode_AddFolder(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var folder = (Node)menuItem.Tag;
            folder.Nodes.Add(new Node { Name = "Noname", NodeType = NodeType.Folder });
        }


        private void ContextMenuNode_Delete(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var folder = (Node)menuItem.Tag;
            DeleteFolder(Project.Root, folder);
        }

        private bool DeleteFolder(Node parentFolder, Node folder)
        {
            var wasRemoved = parentFolder.Nodes.Remove(folder);
            if (!wasRemoved)
            {
                foreach (var subFolder in parentFolder.Nodes)
                {
                    if (DeleteFolder(subFolder, folder))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void ContextMenuNode_DeleteNode(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var node = (Node)menuItem.Tag;
            DeleteNode(Project.Root, node);
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

        private bool DeleteNode(Node parentFolder, Node node)
        {
            var wasRemoved = parentFolder.Nodes.Remove(node);
            if (!wasRemoved)
            {
                foreach (var subFolder in parentFolder.Nodes)
                {
                    if (DeleteNode(subFolder, node))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void Save()
        {
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
            if (source is Node)
            {
                var sourceEntity = source as Node;
                var dropItem = GetNearestContainer(e.OriginalSource as UIElement);
                if (dropItem != null)
                {
                    if (dropItem.Header is Node)
                    {
                        var dropEntity = dropItem.Header as Node;
                        if (source != dropEntity && !IsSubNode(sourceEntity, dropEntity))
                        {
                            var parentSourceFolder = GetParentFolder(sourceEntity);
                            dropEntity.Nodes.Insert(0, sourceEntity);
                            parentSourceFolder.Nodes.Remove(sourceEntity);
                        }
                    }
                }
            }
            else if (source is Node)
            {
                var sourceEntity = source as Node;
                var dropItem = GetNearestContainer(e.OriginalSource as UIElement);
                if (dropItem != null)
                {
                    if (dropItem.Header is Node)
                    {
                        var dropEntity = dropItem.Header as Node;
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

        private bool IsSubNode(Node node1, Node node2)
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

        private Node GetParentFolder(Node folder)
        {
            return GetParentFolder(Roots, folder);
        }

        private Node GetParentFolder(ObservableCollection<Node> folders, Node childFolder)
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
