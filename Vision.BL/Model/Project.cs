using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.BL.Model;

namespace Vision.BL.Model
{
    public class Project : Entity
    {
        public string Path { get; set; }
        public bool AutoSave { get; set; }
        public bool Incognito { get; set; }
        public Layout Layout { get; set; }
        public FolderNode Root { get; set; }

        public Project()
        {
            Root = new FolderNode { Name = "Root" };
            Layout = new Layout();
        }

        public Project(string fileName) : base()
        {
            Path = fileName;
        }

        public Node AddNode(FolderNode parentNode, string name)
        {
            var index = 0;

            var targetList = parentNode != null ? parentNode.Nodes : Root.Nodes;

            if (targetList.Any())
            {
                index = targetList.OrderBy(n => n.Index).Last().Index + 1;
            }

            var node = new Node { Name = name, Index = index };

            if (parentNode != null)
                parentNode.Nodes.Add(node);
            else
                Root.Nodes.Add(node);

            return node;
        }

        public void RemoveNode(FolderNode parentNode, Node node)
        {
            if (parentNode == null)
                Root.Nodes.Remove(node);
            else
                parentNode.Nodes.Remove(node);
        }
    }
}
