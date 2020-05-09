using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Softwaremeisterei.Lib
{
    public class Serialization
    {
        public static string ToXml(Object obj)
        {
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8
            };
            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
                {
                    var xmlSerializer = new XmlSerializer(obj.GetType());
                    xmlSerializer.Serialize(xmlWriter, obj);
                }
                memoryStream.Position = 0;
                using (StreamReader sr = new StreamReader(memoryStream))
                {
                    var result = sr.ReadToEnd();
                    return result;
                }
            }
        }

        public static T ParseXml<T>(string xml)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var textReader = new StringReader(xml))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }

        public static string ToJson(Object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            var serializer = new DataContractJsonSerializer(obj.GetType());

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                var json = ms.ToArray();
                ms.Close();
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }

        public static T ParseJson<T>(string json) where T : class, new()
        {
            var obj = new T();
            var ser = new DataContractJsonSerializer(obj.GetType());

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                obj = ser.ReadObject(ms) as T;
                ms.Close();
                return obj;
            }
        }
    }
}
