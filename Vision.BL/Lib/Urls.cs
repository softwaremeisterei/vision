using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softwaremeisterei.Lib
{
    public static class Urls
    {
        public static string NormalizeUrl(string url)
        {
            return url == null || url.StartsWith("http://") || url.StartsWith("https://") ? url : $"http://{url}";
        }
    }
}
