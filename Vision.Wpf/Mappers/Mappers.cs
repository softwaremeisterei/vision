using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.BL.Model;
using Vision.Wpf.Model;

namespace Vision.Wpf.Mappers
{
    class LinkMappers
    {
        public static ObservableCollection<LinkView> MapToView(IEnumerable<Link> links)
        {
            var mapper = Global.Mapper;
            var linkViews = mapper.Map<LinkView[]>(links);
            return new ObservableCollection<LinkView>(linkViews);
        }

        public static LinkView MapToView(Link link)
        {
            var mapper = Global.Mapper;
            var linkView = mapper.Map<LinkView>(link);
            return linkView;
        }
    }
}
