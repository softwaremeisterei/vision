using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Vision.Wpf.Model
{
    public class NodeView : INotifyPropertyChanged
    {
        private Guid id;
        private string name;
        private string url;
        private bool isFavorite;
        private int index;
        private string icon;
        private string imageSource;

        public event PropertyChangedEventHandler PropertyChanged;

        public Guid Id
        {
            get => id;
            set
            {
                id = value;
                NotifyPropertyChanged(nameof(Id));
            }
        }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }
        public string Url
        {
            get => url;
            set
            {
                url = value;
                NotifyPropertyChanged(nameof(Url));
            }
        }
        public bool IsFavorite
        {
            get => isFavorite;
            set
            {
                isFavorite = value;
                NotifyPropertyChanged(nameof(IsFavorite));
            }
        }
        public int Index
        {
            get => index;
            set
            {
                index = value;
                NotifyPropertyChanged(nameof(Index));
            }
        }
        public NodeViewType NodeType { get; set; }
        public ObservableCollection<NodeView> Nodes { get; set; }

        public string Icon
        {
            get => icon;
            set
            {
                icon = value;
                NotifyPropertyChanged(nameof(Icon));
            }
        }
        public string ImageSource
        {
            get => imageSource;
            set
            {
                imageSource = value;
                NotifyPropertyChanged(nameof(ImageSource));
            }
        }

        public object Tag { get; set; }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum NodeViewType { Folder, Link }

}
