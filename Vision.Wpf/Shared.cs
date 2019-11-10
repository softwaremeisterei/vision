using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Vision.BL.Model;
using Vision.Wpf.Model;
using Vision.Wpf.Styles;

namespace Vision.Wpf
{
    public class Shared
    {
        public static void EditNode(Window owner, NodeView node)
        {
            if (node.NodeType == NodeViewType.Folder)
            {
                var dlg = new EditFolderWindow(node)
                {
                    Owner = owner,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                if (dlg.ShowDialog() == true)
                { }
            }
            else if (node.NodeType == NodeViewType.Link)
            {
                var dlg = new EditNodeWindow(node)
                {
                    Owner = owner,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                dlg.ShowDialog();
            }
        }

        public static void AddNode(Window owner, NodeView parentFolderView)
        {
            try
            {
                var newNode = new Node
                {
                    Name = "Noname",
                    NodeType = NodeType.Link,
                    BackgroundColor = NodeStyles.LinkBackgroundColor,
                    ForegroundColor = NodeStyles.LinkForegroundColor
                };
                (parentFolderView.Tag as Node).Nodes.Add(newNode);
                var newNodeView = Global.Mapper.Map<NodeView>(newNode);
                parentFolderView.Nodes.Add(newNodeView);
                Shared.EditNode(owner, newNodeView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public static void DeleteNode(NodeView parentNodeView, NodeView nodeView)
        {
            try
            {
                parentNodeView.Nodes.Remove(nodeView);
                var parentNode = parentNodeView.Tag as Node;
                var node = nodeView.Tag as Node;
                parentNode.Nodes.Remove(node);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
