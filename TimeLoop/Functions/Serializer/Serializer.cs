using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TimeLoop.Functions
{
    [Serializable]
    public class Serializer
    {
        private static string fileLocation = "/Mods/TimeLoop/TimeLooper.json";
        private static string absoluteFilePath;

        private static Serializer instance;

        public bool EnableTimeLooper = true;

        public List<PlayerData> PlayerData;

        private static string AbsolueFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(absoluteFilePath))
                {
                    string currentDirectory = Directory.GetCurrentDirectory();
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
                }
                return instance;
            }
        }

        private static Serializer LoadInstance()
        {
            if (File.Exists(AbsolueFilePath))
            {
                string jsonString = File.ReadAllText(AbsolueFilePath);
                return JsonUtility.FromJson<Serializer>(jsonString);
            }
            else
            {
                Serializer instance = new Serializer();
                string jsonString = JsonUtility.ToJson(instance, true);
                File.WriteAllText(AbsolueFilePath, jsonString);
                return instance;
            }
        }

        public void ReloadConfig()
        {
            if (File.Exists(AbsolueFilePath))
            {
                string jsonString = File.ReadAllText(absoluteFilePath);
                JsonUtility.FromJsonOverwrite(jsonString, this);
            }
        }

        public void SaveConfig()
        {
            if (File.Exists(AbsolueFilePath))
            {
                string jsonString = JsonUtility.ToJson(instance, true);
                File.WriteAllText(AbsolueFilePath, jsonString);
            }
        }

        public static implicit operator bool(Serializer instance)
        {
            return instance != null;
        }
    }
}
