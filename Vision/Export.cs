using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vision.BL.Model;

namespace Vision.BL
{
    public class Export
    {
        public static void ToTextFile(List<Node> nodes, string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    Write(nodes, writer, 0);
                }
            }
        }

        private static void Write(List<Node> nodes, StreamWriter writer, int indent)
        {
            var indentSpaces = new String(' ', indent * 4);
            var indentSpacesContent = new String(' ', (indent + 1 )* 4);

            foreach (var node in nodes)
            {
                writer.WriteLine("{0}+ {1}", indentSpaces, node.Title);

                if (!string.IsNullOrEmpty(node.Content))
                {
                    var plainText = RichTextStripper.StripRichTextFormat(node.Content);

                    foreach (var line in plainText.Split('\n'))
                    {
                        writer.WriteLine("{0}{1}", indentSpacesContent, line);
                    }
                }

                if (node.Nodes.Any())
                {
                    Write(node.Nodes, writer, indent + 1);
                }
            }
        }
    }
}
