using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;

namespace TimeLoop.Functions
{
    public class XmlSerializerWrapper
    {
        public static T FromXml<T>(string path)
        {
            StreamReader reader = new StreamReader(path);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            T obj = (T)xmlSerializer.Deserialize(reader);
            reader.Close();
            return obj;
        }

        public static void ToXml<T>(string path, T obj)
        {
            TextWriter writer = new StreamWriter(path);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(writer, obj);
            writer.Close();
        }

        public static void FromXmlOverwrite<T>(string path, T obj)
        {
            StreamReader reader = new StreamReader(path);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            T objNew = (T)xmlSerializer.Deserialize(reader);
            reader.Close();

            Type type = typeof(T);
            FieldInfo[] objFields = type.GetFields();
            for (int i = 0; i < objFields.Length; i++)
            {
                objFields[i].SetValue(obj, objFields[i].GetValue(objNew));
            }
        }


        public static implicit operator bool(XmlSerializerWrapper instance)
        {
            return instance != null;
        }
    }
}
