namespace Vision.BL.Model
{
    public class Project : Entity
    {
        public string Path { get; set; }
        public bool AutoSave { get; set; }
        public bool Incognito { get; set; }
        public Layout Layout { get; set; }
        public Node Root { get; set; }

        public Project()
        {
            Root = new Node { Name = "Root" };
            Layout = new Layout();
        }

        public Project(string fileName) : base()
        {
            Path = fileName;
        }

        public Node AddNode(Node parentNode, string name)
        {
            var node = new Node { Name = name };

            if (parentNode != null)
                parentNode.Nodes.Add(node);
            else
                Root.Nodes.Add(node);

            return node;
        }

        public void RemoveNode(Node parentNode, Node node)
        {
            if (parentNode == null)
                Root.Nodes.Remove(node);
            else
                parentNode.Nodes.Remove(node);
        }
    }
}
