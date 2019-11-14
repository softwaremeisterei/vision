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
        public static NodeView AddNewNode(Window owner)
        {
            var newNode = new Node
            {
                Name = "Noname",
            };
            var newNodeView = Global.Mapper.Map<NodeView>(newNode);
            newNodeView.Tag = newNode;
            Shared.EditNode(owner, newNodeView);
            return newNodeView;
        }

        public static void EditNode(Window owner, NodeView nodeView)
        {
            var dlg = new EditNodeWindow(nodeView)
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            dlg.ShowDialog();
            CopyToNodeBehind(nodeView);
        }

        public static void ToggleFavorite(NodeView nodeView)
        {
            nodeView.IsFavorite = !nodeView.IsFavorite;
            nodeView.ImageSource = nodeView.IsFavorite ? Global.FavoriteStarUri : "";
            CopyToNodeBehind(nodeView);
        }

        private static void CopyToNodeBehind(NodeView nodeView)
        {
            Global.Mapper.Map(nodeView, nodeView.Tag as Node);
        }

    }
}
