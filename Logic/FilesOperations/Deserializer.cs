﻿using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Logic.FilesOperations
{
    class Deserializer
    {
        public static T DeserializeXML<T>(string filepath)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof (T));

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
            catch (SerializationException e)
            {
            }
            catch (Exception e)
            {
            }
            return default(T);
        }
    }
}
