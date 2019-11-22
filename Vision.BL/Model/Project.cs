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
        public ObservableCollection<Link> Links { get; set; }

        public ObservableCollection<Guid> History { get; set; }
        public double TileSize { get; set; }
        public double FontSize { get; set; }
        public double SmallTileSize { get; set; }
        public double SmallFontSize { get; set; }

        public Project()
        {
            Layout = new Layout();
            Links = new ObservableCollection<Link>();
            History = new ObservableCollection<Guid>();
            TileSize = 160d;
            FontSize = 16d;
            SmallTileSize = 80d;
            SmallFontSize = 8d;
        }

        public Project(string fileName) : base()
        {
            Path = fileName;
        }
    }
}
