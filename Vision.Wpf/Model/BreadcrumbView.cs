using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Wpf.Model
{
    public class BreadcrumbView
    {
        public NodeView NodeView { get; set; }

        public BreadcrumbView(NodeView nodeView)
        {
            NodeView = nodeView;
        }
    }
}
