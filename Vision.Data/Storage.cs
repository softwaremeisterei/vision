using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Data
{
    public class Storage
    {
        public Storage()
        {
        }

        public void Save<T>(T obj, string filename)
        {
            var json = Serialization.ToJson(obj);
            File.WriteAllText(filename, json);
        }

        public T Load<T>(string filename) where T : class, new()
        {
            var json = File.ReadAllText(filename);
            var obj = Serialization.ParseJson<T>(json);
            return obj;
        }
    }
}
