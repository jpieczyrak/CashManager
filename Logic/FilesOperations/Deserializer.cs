using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace LogicOld.FilesOperations
{
    public class Deserializer
    {
        public static T DeserializeXMLFromFile<T>(string filepath) where T : new()
        {
            var serializer = new DataContractSerializer(typeof(T));

            try
            {
                if (File.Exists(filepath))
                {
                    using (var w = XmlReader.Create(filepath))
                    {
                        return (T) serializer.ReadObject(w);
                    }
                }
            }
            catch (SerializationException e) { }
            catch (Exception e) { }
            return new T();
        }

        public static object Deserialize(string xml, Type toType)
        {
            using (Stream stream = new MemoryStream())
            {
                var data = Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                var deserializer = new DataContractSerializer(toType);
                return deserializer.ReadObject(stream);
            }
        }
    }
}