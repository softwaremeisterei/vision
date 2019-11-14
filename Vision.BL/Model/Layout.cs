using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.BL.Model
{
    public class Layout
    {
        public bool IsMaximized { get; set; }
        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }
        public Layout()
        {
        }
    }
}
