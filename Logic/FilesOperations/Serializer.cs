using System;
using System.Runtime.Serialization;
using System.Xml;

namespace Logic.FilesOperations
{
    public static class Serializer
    {
        public static void XMLSerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null)
            {
                return;
            }

            var serializer = new DataContractSerializer(typeof(T));
            var settings = new XmlWriterSettings { Indent = true };
            
            try
            {
                using (XmlWriter writer = XmlWriter.Create(fileName, settings))
                {
                    serializer.WriteObject(writer, serializableObject);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
