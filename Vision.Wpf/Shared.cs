using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Vision.BL.Model;
using Vision.Wpf.Model;

namespace Vision.Wpf
{
    public class Shared
    {
        public static void AddNewNode(Window owner, NodeView parentNodeView)
        {
            var newNode = new Node
            {
                Name = "Noname",
                NodeType = NodeType.Link,
            };
            (parentNodeView.Tag as Node).Nodes.Add(newNode);
            var newNodeView = Global.Mapper.Map<NodeView>(newNode);
            parentNodeView.Nodes.Add(newNodeView);
            Shared.EditNode(owner, newNodeView);
        }

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

        public static void DeleteNode(NodeView parentNodeView, NodeView nodeView)
        {
            parentNodeView.Nodes.Remove(nodeView);
            var parentNode = parentNodeView.Tag as Node;
            var node = nodeView.Tag as Node;
            parentNode.Nodes.Remove(node);
        }

        public static void AddNewFolder(Window owner, NodeView parentNodeView)
        {
            var newFolderNode = new Node
            {
                Name = "Noname",
                NodeType = NodeType.Folder,
            };
            (parentNodeView.Tag as Node).Nodes.Add(newFolderNode);
            var newFolderView = Global.Mapper.Map<NodeView>(newFolderNode);
            parentNodeView.Nodes.Add(newFolderView);
            parentNodeView.NodeType = NodeViewType.Folder;
            (parentNodeView.Tag as Node).NodeType = NodeType.Folder;
            EditNode(owner, newFolderView);
        }

        internal static void ToggleFavorite(NodeView nodeView)
        {
            nodeView.IsFavorite = !nodeView.IsFavorite;
            (nodeView.Tag as Node).IsFavorite = !(nodeView.Tag as Node).IsFavorite;
            nodeView.ImageSource = nodeView.IsFavorite ? Global.FavoriteStarUri : "";
        }
    }
}
