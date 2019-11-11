using System;
using System.Collections.ObjectModel;

namespace Vision.BL.Model
{
    public class Node : Entity
    {
        public NodeType NodeType { get; set; }
        public string Url { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime CreatedAt { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }

        public Node()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            CreatedAt = DateTime.Now;
            NodeType = NodeType.Folder;
            Nodes = new ObservableCollection<Node>();
        }

        public virtual Node Copy()
        {
            var copy = Create();

            copy.Name = Name;
            copy.Url = Url;
            copy.IsFavorite = IsFavorite;
            copy.NodeType = NodeType;

            return copy;
        }

        public virtual Node Create()
        {
            return new Node();
        }

    }

}
