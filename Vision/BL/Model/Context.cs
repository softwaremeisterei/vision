using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.BL.Model;

namespace Vision.BL.Model
{
    public class Context
    {
        public List<Node> Nodes { get; set; }
        public Layout Layout { get; set; }

        public Context()
        {
            Nodes = new List<Node>();
            Layout = new Layout();
        }

        public Node AddNode(Node parentNode, string title)
        {
            var index = 0;

            var targetList = parentNode != null ? parentNode.Nodes : Nodes;

            if (targetList.Any())
            {
                index = targetList.OrderBy(n => n.Index).Last().Index + 1;
            }

            var node = new Node { Title = title, Index = index };

            if (parentNode != null)
                parentNode.Nodes.Add(node);
            else
                Nodes.Add(node);

            return node;
        }

        public void RemoveNode(Node parentNode, Node node)
        {
            if (parentNode == null)
                Nodes.Remove(node);
            else
                parentNode.Nodes.Remove(node);
        }
    }
}
