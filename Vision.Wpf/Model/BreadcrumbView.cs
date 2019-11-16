using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Wpf.Model
{
    public class BreadcrumbView
    {
        public LinkView LinkView { get; set; }

        public BreadcrumbView(LinkView linkView)
        {
            LinkView = linkView;
        }
    }
}
