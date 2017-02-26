using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Logic.FilesOperations
{
    public static class Serializer
    {
        public static void XMLSerializeObjectToFile<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null)
            {
                return;
            }

            var serializer = new DataContractSerializer(typeof(T));
            var settings = new XmlWriterSettings { Indent = true };

            try
            {
                using (var writer = XmlWriter.Create(fileName, settings))
                {
                    serializer.WriteObject(writer, serializableObject);
                }
            }
            catch (Exception) { }
        }

        public static string XMLSerializeObject<T>(T serializableObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var reader = new StreamReader(memoryStream))
                {
                    var serializer = new DataContractSerializer(serializableObject.GetType());
                    serializer.WriteObject(memoryStream, serializableObject);
                    memoryStream.Position = 0;
                    return reader.ReadToEnd();
                }
            }
        }
    }
}