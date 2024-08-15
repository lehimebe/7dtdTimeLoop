using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TimeLoop.Functions
{

    [Serializable]  
    public class JsonContentData : XmlContentData
    {
        public new static JsonContentData DeserializeInstance()
        {
            string currentDirectory = Directory.GetParent(Application.dataPath).FullName;
            string absoluteFilePath = Path.Combine(currentDirectory, fileLocation);
            JsonContentData data;

            if (Directory.Exists(Path.GetDirectoryName(absoluteFilePath)) && File.Exists(absoluteFilePath))
            {
                Log.Out("[TimeLoop] Loading Config ...");
                string jsonString = File.ReadAllText(absoluteFilePath);
                data = JsonUtility.FromJson<JsonContentData>(jsonString);

            }
            else
            {
                Log.Out("[TimeLoop] Creating New Config ...");
                data = new JsonContentData();
                string jsonString = JsonUtility.ToJson(data, true);
                File.WriteAllText(absoluteFilePath, jsonString);
            }

            data.absoluteFilePath = absoluteFilePath;
            data.UpdateLastModified();
            return data;
        }

        public new void CheckForUpdate()
        {
            if (UpdateLastModified())
            {
                ReloadConfig();
            }
        }

        public new void ReloadConfig()
        {
            if (File.Exists(absoluteFilePath))
            {
                string jsonString = File.ReadAllText(absoluteFilePath);
                JsonUtility.FromJsonOverwrite(jsonString, this);
                Log.Out("[TimeLoop] Config Updated!");
            }
        }

        public new void SaveConfig()
        {
            if (File.Exists(absoluteFilePath))
            {
                string jsonString = JsonUtility.ToJson(this, true);
                File.WriteAllText(absoluteFilePath, jsonString);
                UpdateLastModified();
            }
        }

        public static implicit operator bool(JsonContentData instance)
        {
            return instance != null;
        }
    }
}