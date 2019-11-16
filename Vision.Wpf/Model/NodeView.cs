using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

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

        public String Shortcut
        {
            get { return new string(Name.Split(' ').Select(tok => tok[0]).Where(c => Char.IsLetter(c)).Take(3).Select(c => Char.ToUpper(c)).ToArray());  }
            set { }
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

        public ObservableCollection<NodeView> Nodes { get; set; }

        public ObservableCollection<string> Tags { get; set; }

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

        public object Tag { get; set; }
    }

}
