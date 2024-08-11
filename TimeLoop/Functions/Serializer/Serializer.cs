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
        private static string fileLocation = "Mods/TimeLoop/TimeLooper.json";
        private static string absoluteFilePath;
        private int fileSize;

        private static Serializer instance;

        public bool EnableTimeLooper = true;

        public List<PlayerData> PlayerData = new List<PlayerData>();
        public List<string> CrossplatformId = new List<string>();
        public List<bool> SkipTimeLoop = new List<bool>();

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
                string jsonString = File.ReadAllText(AbsolueFilePath);
                Serializer instance = JsonUtility.FromJson<Serializer>(jsonString);
                #region WORKAROUND
                for (int i = 0; i < instance.CrossplatformId.Count; i++)
                {
                    instance.PlayerData.Add(new PlayerData(instance.CrossplatformId[0], instance.SkipTimeLoop[0]));
                }
                #endregion
                return instance;
            }
            else
            {
                Log.Out("[TimeLoop] Creating New Config ...");
                Serializer instance = new Serializer();
                string jsonString = JsonUtility.ToJson(instance, true);
                File.WriteAllText(AbsolueFilePath, jsonString);
                return instance;
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
                string jsonString = File.ReadAllText(absoluteFilePath);
                JsonUtility.FromJsonOverwrite(jsonString, this);
                Log.Out("[TimeLoop] Config Updated!");
            }
        }

        public void SaveConfig()
        {
            if (File.Exists(AbsolueFilePath))
            {
                string jsonString = JsonUtility.ToJson(this, true);
                File.WriteAllText(AbsolueFilePath, jsonString);
                UpdateFileSize();
            }
        }

        public static implicit operator bool(Serializer instance)
        {
            return instance != null;
        }
    }
}
