using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.BL.Model;
using Vision.Wpf.Model;

namespace Vision.Wpf
{
    class BootStrapper
    {
        public static void Initialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Node, NodeView>()
                    .ForMember(nameof(NodeView.Icon), src => src.MapFrom(o => o.NodeType == NodeType.Folder ? "📁" : "≡"))
                    .ForMember(nameof(NodeView.ImageSource), src => src.MapFrom(o => o.IsFavorite ? Global.FavoriteStarUri : ""))
                    .ForMember(nameof(NodeView.Tag), src => src.MapFrom(o => o));

                cfg.CreateMap<NodeView, Node>();
            });

            Global.Mapper = config.CreateMapper();
        }
    }
}
