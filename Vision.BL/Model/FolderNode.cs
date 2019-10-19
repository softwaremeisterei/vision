using System.Collections.ObjectModel;

namespace Vision.BL.Model
{
    public class FolderNode : Entity
    {
        public ObservableCollection<FolderNode> Folders { get; set; }

        public ObservableCollection<Node> Nodes { get; set; }

        public FolderNode()
        {
            Folders = new ObservableCollection<FolderNode>();
            Nodes = new ObservableCollection<Node>();
        }

        public FolderNode Copy()
        {
            var copy = new FolderNode();

            foreach (var folder in Folders)
            {
                copy.Folders.Add(folder.Copy());
            }

            foreach (var child in Nodes)
            {
                copy.Nodes.Add(child.Copy());
            }

            return copy;
        }
    }

}
