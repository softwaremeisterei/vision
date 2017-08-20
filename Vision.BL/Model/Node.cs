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
            Nodes = new List<Node>();
            CreatedAt = UpdatedAt = DateTime.Now;
            Content = string.Empty;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<Node> Nodes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
