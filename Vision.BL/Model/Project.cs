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
    }
}
