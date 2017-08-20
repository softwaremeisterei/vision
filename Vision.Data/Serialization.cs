using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Data
{
    class Serialization
    {
        public static string ToJson(Object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            using (MemoryStream ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(ms, obj);
                var json = ms.ToArray();
                ms.Close();
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }

        public static T ParseJson<T>(string json) where T:class,new()
        {
            var obj = new T();
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var ser = new DataContractJsonSerializer(obj.GetType());
            obj = ser.ReadObject(ms) as T;
            ms.Close();
            return obj;
        }
    }
}
