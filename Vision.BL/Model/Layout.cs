using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.BL.Model
{
    public class Layout
    {
        public Guid SelectedNode { get; set; }
        public List<Guid> ExpandedNodes { get; set; }

        public Layout()
        {
            ExpandedNodes = new List<Guid>();
        }
    }
}
