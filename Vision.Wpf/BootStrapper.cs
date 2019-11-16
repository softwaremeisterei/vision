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
                cfg.CreateMap<Link, LinkView>()
                    .ForMember(nameof(LinkView.Icon), src => src.MapFrom(o => "≡"))
                    .ForMember(nameof(LinkView.ImageSource), src => src.MapFrom(o => o.IsFavorite ? Global.FavoriteStarUri : ""))
                    .ForMember(nameof(LinkView.Tag), src => src.MapFrom(o => o));

                cfg.CreateMap<LinkView, Link>();
            });

            Global.Mapper = config.CreateMapper();
        }
    }
}
