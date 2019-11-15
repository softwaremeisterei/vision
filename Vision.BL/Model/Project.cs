using System;
using System.Collections.ObjectModel;

namespace Vision.BL.Model
{
    public class Project : Entity
    {
        public string Path { get; set; }
        public bool AutoSave { get; set; }
        public bool Incognito { get; set; }
        public Layout Layout { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }

        public ObservableCollection<Guid> History { get; set; }

        public Project()
        {
            Layout = new Layout();
            Nodes = new ObservableCollection<Node>();
            History = new ObservableCollection<Guid>();
        }

        public Project(string fileName) : base()
        {
            Path = fileName;
        }
    }
}
