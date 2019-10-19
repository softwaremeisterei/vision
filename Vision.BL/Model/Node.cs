using System;

namespace Vision.BL.Model
{
    public class Node : Entity
    {
        public string Url { get; set; }
        public string ImageId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Index { get; set; }
        public bool IsFavorite { get; set; }
        public DisplayType DisplayType { get; set; }

        public Node()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            Content = string.Empty;
            CreatedAt = DateTime.Now;
            DisplayType = DisplayType.Folder;
        }

        public virtual Node Copy()
        {
            var copy = Create();

            copy.Name = Name;
            copy.Url = Url;
            copy.Content = Content;
            copy.IsFavorite = IsFavorite;
            copy.DisplayType = DisplayType;

            return copy;
        }

        public virtual Node Create()
        {
            return new Node();
        }
    }

}
