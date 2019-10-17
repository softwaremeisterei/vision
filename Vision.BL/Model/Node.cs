using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.BL.Model
{
    public class Node
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ImageId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Index { get; set; }
        public bool IsFavorite { get; set; }
        public DisplayType DisplayType { get; set; }

        public Node()
        {
            Id = Guid.NewGuid();
            Title = string.Empty;
            Content = string.Empty;
            CreatedAt = DateTime.Now;
            DisplayType = DisplayType.Folder;
        }

        public virtual Node Copy()
        {
            var copy = Create();

            copy.Title = Title;
            copy.Url = Url;
            copy.Content = Content;
            copy.IsFavorite = IsFavorite;
            copy.DisplayType = DisplayType;

            return copy;
        }

        public virtual Node Create()
        {
            return new Node();
        }
    }

    public class FolderNode : Node
    {
        public ObservableCollection<Node> Nodes { get; set; }

        public FolderNode()
        {
            Nodes = new ObservableCollection<Node>();
        }

        public override Node Copy()
        {
            var copy = (FolderNode)base.Copy();

            foreach (var child in Nodes)
            {
                copy.Nodes.Add(child.Copy());
            }

            return copy;
        }

        public override Node Create()
        {
            return new FolderNode();
        }
    }
}
