using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Vision.BL.Model;

namespace Vision.Wpf.Model
{
    public class LinkView : INotifyPropertyChanged
    {
        private Guid id;
        private string name;
        private string url;
        private bool isFavorite;
        private string icon;

        public event PropertyChangedEventHandler PropertyChanged;

        public Guid Id
        {
            get => id;
            set
            {
                id = value;
                if (Tag != null) Tag.Id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
            }
        }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                if (Tag != null) Tag.Name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public String Shortcut
        {
            get {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var chars = Name.Split(' ').Select(tok => tok[0]).Where(c => Char.IsLetter(c)).Take(3).Select(c => Char.ToUpper(c)).ToArray();
                    return new string(chars.Length > 0 ? chars : new[]{' '});
                }
                else { return "-"; }
            }
            set { }
        }

        public string Url
        {
            get => url;
            set
            {
                url = value;
                if (Tag != null) Tag.Url = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Url)));
            }
        }
        public bool IsFavorite
        {
            get => isFavorite;
            set
            {
                isFavorite = value;
                if (Tag != null) Tag.IsFavorite = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFavorite)));
            }
        }

        public ObservableCollection<LinkView> Links { get; set; }

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

        public Link Tag { get; set; }
    }

}
