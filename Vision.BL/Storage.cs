using System.IO;
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
            var json = JsonSerialization.ToJson(obj);
            File.WriteAllText(filename, json);
        }

        public T Load<T>(string filename) where T : class, new()
        {
            var json = File.ReadAllText(filename);
            var obj = JsonSerialization.ParseJson<T>(json);
            return obj;
        }
    }
}
