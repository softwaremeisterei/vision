using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Wpf.Model
{
    public class NodeView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsFavorite { get; set; }
        public int Index { get; set; }
        public NodeViewType NodeType { get; set; }
        public ObservableCollection<NodeView> Nodes { get; set; }

        public string Icon { get; set; }
        public string ImageSource { get; set; }

        public object Tag { get; set; }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum NodeViewType { Folder, Link }

}
