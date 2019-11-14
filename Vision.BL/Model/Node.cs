using System;
using System.Collections.ObjectModel;

namespace Vision.BL.Model
{
    public class Node : Entity
    {
        public string Url { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime CreatedAt { get; set; }
        public ObservableCollection<string> Tags  { get; set; }

        public Node()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            CreatedAt = DateTime.Now;
            Tags = new ObservableCollection<string>();
        }

        public virtual Node Copy()
        {
            var copy = Create();

            copy.Name = Name;
            copy.Url = Url;
            copy.IsFavorite = IsFavorite;
            copy.Tags = new ObservableCollection<string>(Tags);

            return copy;
        }

        public virtual Node Create()
        {
            return new Node();
        }

    }

}
