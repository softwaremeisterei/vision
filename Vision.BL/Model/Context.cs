using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.BL.Model;

namespace Vision.BL.Model
{
    public class Context
    {
        public string FileName { get; set; }
        public bool AutoSave { get; set; }
        public bool Incognito { get; set; }
        public Layout Layout { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }

        public Context() { }

        public Context(string fileName)
        {
            FileName = fileName;
            Nodes = new ObservableCollection<Node>();
            Layout = new Layout();
        }

        public Node AddNode(FolderNode parentNode, string title)
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

        public void RemoveNode(FolderNode parentNode, Node node)
        {
            if (parentNode == null)
                Nodes.Remove(node);
            else
                parentNode.Nodes.Remove(node);
        }
    }
}
