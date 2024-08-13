using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace TimeLoop.Functions
{
#if XML_SERIALIZATION
    [XmlRoot("TimeLoopSettings")]
#else
    [Serializable]  
#endif
    public class Serializer
    {
#if XML_SERIALIZATION
        private static string fileLocation = "Mods/TimeLoop/TimeLooper.xml";
#else
        private static string fileLocation = "Mods/TimeLoop/TimeLooper.json";
#endif
        private static string absoluteFilePath;
        private int fileSize;

        private static Serializer instance;
#if XML_SERIALIZATION
        private XmlSerializer xmlSerializer;
#endif

        public bool EnableTimeLooper = true;

#if XML_SERIALIZATION
        [XmlArray("KnownPlayers")]
#endif
        public List<PlayerData> PlayerData = new List<PlayerData>() { new PlayerData() };

        private static string AbsolueFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(absoluteFilePath))
                {
                    string currentDirectory = Directory.GetParent(Application.dataPath).FullName;
                    absoluteFilePath = Path.Combine(currentDirectory, fileLocation);
                }
                return absoluteFilePath;
            }
        }

        public static Serializer Instance
        {
            get
            {
                if (!instance)
                {
                    instance = LoadInstance();
                    instance.UpdateFileSize();
                }
                return instance;
            }
        }

        private static Serializer LoadInstance()
        {
            if (Directory.Exists(Path.GetDirectoryName(AbsolueFilePath)) && File.Exists(AbsolueFilePath))
            {
                Log.Out("[TimeLoop] Loading Config ...");

#if XML_SERIALIZATION                
                using (FileStream fs = new FileStream(AbsolueFilePath, FileMode.Open))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Serializer));
                    Serializer instance = (Serializer)xmlSerializer.Deserialize(fs);
                    return instance;
                }
#else
                string jsonString = File.ReadAllText(AbsolueFilePath);
                Serializer instance = JsonUtility.FromJson<Serializer>(jsonString);
                return instance;
#endif

            }
            else
            {
                Log.Out("[TimeLoop] Creating New Config ...");
#if XML_SERIALIZATION
                using (TextWriter writer = new StreamWriter(AbsolueFilePath))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Serializer));
                    Serializer instance = new Serializer();
                    xmlSerializer.Serialize(writer, instance);
                    return instance;
                }
#else
                Serializer instance = new Serializer();
                string jsonString = JsonUtility.ToJson(instance, true);
                File.WriteAllText(AbsolueFilePath, jsonString);
                return instance;
#endif
            }
        }

        private bool UpdateFileSize()
        {
            int fileSizeOld = this.fileSize;
            this.fileSize = File.ReadAllBytes(AbsolueFilePath).Length;
            return fileSizeOld != this.fileSize;
        }

        public void CheckForUpdate()
        {
            if (UpdateFileSize())
            {
                ReloadConfig();
            }
        }

        public void ReloadConfig()
        {
            if (File.Exists(AbsolueFilePath))
            {
#if XML_SERIALIZATION
                using (FileStream fs = new FileStream(AbsolueFilePath, FileMode.Open))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Serializer));
                    Serializer newInstance = (Serializer)xmlSerializer.Deserialize(fs);
                    this.EnableTimeLooper = newInstance.EnableTimeLooper;
                    this.PlayerData = newInstance.PlayerData;
                }
#else
                string jsonString = File.ReadAllText(absoluteFilePath);
                JsonUtility.FromJsonOverwrite(jsonString, this);
#endif
                Log.Out("[TimeLoop] Config Updated!");
            }
        }

        public void SaveConfig()
        {
            if (File.Exists(AbsolueFilePath))
            {
#if XML_SERIALIZATION
                using (TextWriter writer = new StreamWriter(AbsolueFilePath))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Serializer));
                    xmlSerializer.Serialize(writer, this);
                }
#else
                string jsonString = JsonUtility.ToJson(this, true);
                File.WriteAllText(AbsolueFilePath, jsonString);
#endif
                UpdateFileSize();
            }
        }

        public static implicit operator bool(Serializer instance)
        {
            return instance != null;
        }
    }
}
