using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.BL.Model;

namespace Vision.BL
{
    public class Export
    {
        public void ToTextFile(List<Node> nodes, string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    Write(nodes, writer, 0);
                }
            }
        }

        private void Write(List<Node> nodes, StreamWriter writer, int indent)
        {
            var indentSpaces = new String(' ', indent * 4);

            foreach (var node in nodes)
            {
                writer.WriteLine("{0}{1}", indentSpaces, node.Title);

                if (!string.IsNullOrEmpty(node.Content))
                {
                    writer.WriteLine(node.Content);
                }

                if (node.Nodes.Any())
                {
                    Write(node.Nodes, writer, indent + 1);
                }
            }
        }
    }
}
