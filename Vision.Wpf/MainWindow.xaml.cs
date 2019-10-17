using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Vision.BL;
using Vision.BL.Model;

namespace Vision.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Context Context { get; set; }

        private Persistor persistor;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            persistor = new Persistor();

            var filePath = @"E:\Work\sandbox.visx";
            Context = persistor.Load(filePath);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("http://www.youtube.com");
            webBrowser.Navigate(uri);

            ExpandAll(treeView1.Items.OfType<Node>().ToList());
        }

        private void ExpandAll(IList<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                var item = treeView1.ItemContainerGenerator.ContainerFromItem(node) as TreeViewItem;
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

                if (node is FolderNode)
                {
                    var child = FindNode((node as FolderNode).Nodes, id);

                    if (child != null)
                    {
                        return child;
                    }
                }
            }

            return null;
        }

#if false
        private void SelectNode(Guid id)
        {
            if (id == Guid.Empty) { return; }

            var node = FindNode(Context.Nodes, id);

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

                if (Context.Layout.ExpandedNodes.Contains(node.Id))
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
