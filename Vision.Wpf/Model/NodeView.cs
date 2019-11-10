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
        private string icon;
        private string imageSource;
        private string backgroundColor;
        private string foregroundColor;

        public event PropertyChangedEventHandler PropertyChanged;

        public Guid Id
        {
            get => id;
            set
            {
                id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
            }
        }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        public string Url
        {
            get => url;
            set
            {
                url = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Url)));
            }
        }
        public bool IsFavorite
        {
            get => isFavorite;
            set
            {
                isFavorite = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFavorite)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Icon)));
            }
        }
        public string ImageSource
        {
            get => imageSource;
            set
            {
                imageSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageSource)));
            }
        }

        public string BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundColor)));
            }
        }
        public string ForegroundColor
        {
            get => foregroundColor;
            set
            {
                foregroundColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForegroundColor)));
            }
        }

        public object Tag { get; set; }
    }

    public enum NodeViewType { Folder, Link }

}
