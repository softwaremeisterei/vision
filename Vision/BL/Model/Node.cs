using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.BL.Model
{
    public class Node
    {
        public Node()
        {
            Id = Guid.NewGuid();
            Title = string.Empty;
            Content = string.Empty;
            Nodes = new List<Node>();
            CreatedAt = DateTime.Now;
            DisplayType = DisplayType.Folder;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ImageId { get; set; }
        public string Content { get; set; }
        public List<Node> Nodes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Index { get; set; }
        public bool IsFavorite { get; set; }

        public DisplayType DisplayType { get; set; }

        public Node Copy()
        {
            var copy = new Node();
            copy.Title = Title;
            copy.Url = Url;
            copy.Content = Content;
            copy.IsFavorite = IsFavorite;
            copy.DisplayType = DisplayType;

            foreach (var child in Nodes)
            {
                copy.Nodes.Add(child.Copy());
            }

            return copy;
        }
    }
}
