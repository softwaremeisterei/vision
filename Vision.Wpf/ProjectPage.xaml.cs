using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        public ProjectPage(Project project)
        {
            InitializeComponent();
            DataContext = this;

            persistor = new Persistor();

            this.Project = project;
            this.Roots = new ObservableCollection<FolderNode>();
            this.Roots.Add(project.Root);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //var uri = new Uri("http://www.youtube.com");
            //webBrowser.Navigate(uri);

            ExpandAll(treeView1.Items.OfType<FolderNode>().ToList());
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

        private Node FindNode(IList<Node> nodes, Guid id)
        {
            foreach (var node in nodes)
            {
                if (node.Id.Equals(id))
                {
                    return node;
                }

                //if (node is FolderNode)
                //{
                //    var child = FindNode((node as FolderNode).Nodes, id);

                //    if (child != null)
                //    {
                //        return child;
                //    }
                //}
            }

            return null;
        }
        private void mnuAddTopLevelFolder_Click(object sender, RoutedEventArgs e)
        {
            Roots.First().Folders.Add(new FolderNode { Name = "NONAME" });
        }

        private void mnuAddTopLevelNode_Click(object sender, RoutedEventArgs e)
        {
            Roots.First().Nodes.Add(new Node { Name = "Noname" });
        }

        private void mnuFileSave_Click(object sender, RoutedEventArgs e)
        {
            persistor.SaveProject(Project);
        }

        private void ContextMenuFolder_AddNode(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var folder = (FolderNode)menuItem.Tag;
            folder.Nodes.Add(new Node { Name = "Noname" });
        }


#if false
        private void SelectNode(Guid id)
        {
            if (id == Guid.Empty) { return; }

            var node = FindNode(Project.Nodes, id);

            if (node != null)
            {
                var item = treeView1.ItemContainerGenerator.ContainerFromItem(node) as TreeViewItem;

                if (item != null)
                {
                    item.IsSelected = true;
                }
            }
        }

        private void ReloadNodes(TreeNode parentNode, List<Node> nodes)
        {
            foreach (var node in nodes.OrderBy(n => n.Index))
            {
                var treeNode = new TreeNode { Text = node.Title, Tag = node };
                treeNode.ContextMenuStrip = _contextMenu;
                if (node.IsFavorite)
                {
                    treeNode.StateImageIndex = STATEIMAGE_FAVORITE;
                }

                if (parentNode != null)
                    parentNode.Nodes.Add(treeNode);
                else
                    treeView1.Nodes.Add(treeNode);

                if (node.Nodes.Any())
                {
                    ReloadNodes(treeNode, node.Nodes);
                }

                if (Project.Layout.ExpandedNodes.Contains(node.Id))
                {
                    treeNode.Expand();
                }

                ApplySpecialNodeStyles(treeNode);
            }
        }

        private void ApplySpecialNodeStyles(TreeNode treeNode)
        {
            var node = GetNode(treeNode);

            if (node.DisplayType == DisplayType.Browser)
            {
                treeNode.ForeColor = Color.Blue;
            }
            else if (node.DisplayType == DisplayType.Image)
            {
                treeNode.NodeFont = new Font(treeNode.NodeFont ?? treeView1.Font, FontStyle.Italic);
            }
        }

        private Node GetNode(object o)
        {
            if (o == null)
            {
                return null;
            }

            return (Node)o;
        }
#endif

    }
}
