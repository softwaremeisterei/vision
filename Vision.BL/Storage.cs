using Softwaremeisterei.Lib;
using System.IO;
using System.Text;
using Vision.Lib;

namespace Vision.Data
{
    public class Storage
    {
        public Storage()
        {
        }

        public void Save<T>(T obj, string filename)
        {
            var xml = Serialization.ToXml(obj);
            File.WriteAllText(filename, xml, Encoding.UTF8);
        }

        public T Load<T>(string filename) where T : class, new()
        {
            var xml = File.ReadAllText(filename, Encoding.UTF8);
            var obj = Serialization.ParseXml<T>(xml);
            return obj;
        }
    }
}
