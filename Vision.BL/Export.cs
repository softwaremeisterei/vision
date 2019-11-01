using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vision.BL.Model;
using Vision.Lib;

namespace Vision.BL
{
    public class Export
    {
        public static void ToTextFile(ObservableCollection<Node> nodes, string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    Write(nodes, writer, 0);
                }
            }
        }

        private static void Write(ObservableCollection<Node> nodes, StreamWriter writer, int indent)
        {
            var indentSpaces = new String(' ', indent * 4);
            var indentSpacesContent = new String(' ', (indent + 1) * 4);

            foreach (var node in nodes.OrderBy(n => n.Index))
            {
                writer.WriteLine("{0}+ {1}", indentSpaces, node.Name);

                if (!string.IsNullOrWhiteSpace(node.Url))
                {
                    writer.WriteLine("{0}{1}", indentSpacesContent, node.Url);
                }
            }
        }

        static readonly char[] WHITESPACE_CHARS = new char[] { ' ', '\n', '\t' };

        private static bool IsEmpty(string content)
        {
            if (content == null)
            {
                return true;
            }

            return (content.All(c => WHITESPACE_CHARS.Contains(c)));
        }
    }
}
