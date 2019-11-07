using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.BL.Model;
using Vision.Wpf.Model;

namespace Vision.Wpf.Mappers
{
    class NodeMappers
    {
        public static NodeView MapToView(Node root)
        {
            var mapper = Global.Mapper;
            var result = mapper.Map<NodeView>(root);
            return result;
        }
    }
}
