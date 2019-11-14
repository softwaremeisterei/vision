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
    class NodeMappers
    {
        public static ObservableCollection<NodeView> MapToView(IEnumerable<Node> nodes)
        {
            var mapper = Global.Mapper;
            var nodeViews = mapper.Map<NodeView[]>(nodes);
            return new ObservableCollection<NodeView>(nodeViews);
        }
    }
}
