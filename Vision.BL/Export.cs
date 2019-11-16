using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Vision.BL.Model;

namespace Vision.BL
{
    public class Export
    {
        public static void ToTextFile(ObservableCollection<Link> link, string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    Write(link, writer, 0);
                }
            }
        }

        private static void Write(ObservableCollection<Link> links, StreamWriter writer, int indent)
        {
            var indentSpaces = new String(' ', indent * 4);
            var indentSpacesContent = new String(' ', (indent + 1) * 4);

            foreach (var link in links)
            {
                writer.WriteLine("{0}+ {1}", indentSpaces, link.Name);

                if (!string.IsNullOrWhiteSpace(link.Url))
                {
                    writer.WriteLine("{0}{1}", indentSpacesContent, link.Url);
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
