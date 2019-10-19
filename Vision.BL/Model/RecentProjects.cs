using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.BL.Model
{
    public class RecentProjects : Entity
    {
        public List<RecentProject> Projects { get; set; }

        public RecentProjects()
        {
            Projects = new List<RecentProject>();
        }
    }

    public class RecentProject : Entity
    {
        public string Path { get; set; }
        public DateTime LastUsageDate { get; set; }
    }
}
