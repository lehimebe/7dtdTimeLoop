using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace TimeLoop.Functions
{
    [XmlRoot("TimeLoopSettings")]
    public class XmlContentData
    {
        public enum Mode
        {
            [XmlEnum(Name = "none")]
            DISABLED,
            [XmlEnum(Name = "whitelist")]
            WHITELIST,
            [XmlEnum(Name = "threshold")]
            MIN_PLAYER_COUNT
        }

        #region Singleton
        private static XmlContentData instance;
        public static XmlContentData Instance
        {
            get
            {
                if (!instance)
                {
                    instance = new XmlContentData();
                }
                return instance;
            }
        }
        #endregion

        protected const string fileLocation = "Mods/TimeLoop/TimeLooper.xml";
        protected string absoluteFilePath;
        protected DateTime lastModified;

        public bool EnableTimeLooper = true;
        public Mode mode = Mode.WHITELIST;

        [XmlArray("KnownPlayers")]
        public List<PlayerData> PlayerData = new List<PlayerData>();
        public int MinPlayers = 5;

        public static XmlContentData DeserializeInstance()
        {
            string currentDirectory = Directory.GetParent(Application.dataPath).FullName;
            string absoluteFilePath = Path.Combine(currentDirectory, fileLocation);
            XmlContentData data;

            if (Directory.Exists(Path.GetDirectoryName(absoluteFilePath)) && File.Exists(absoluteFilePath))
            {
                Log.Out("[TimeLoop] Loading Config ...");
                data = XmlSerializerWrapper.FromXml<XmlContentData>(absoluteFilePath);

            }
            else
            {
                Log.Out("[TimeLoop] Creating New Config ...");
                data = new XmlContentData();
                XmlSerializerWrapper.ToXml(absoluteFilePath, data);
            }

            data.absoluteFilePath = absoluteFilePath;
            data.UpdateLastModified();
            return data;
        }

        protected bool UpdateLastModified()
        {
            DateTime lastModifiedOld = this.lastModified;
            this.lastModified = new FileInfo(this.absoluteFilePath).LastWriteTime;
            return lastModifiedOld != this.lastModified;
        }

        public void CheckForUpdate()
        {
            if (UpdateLastModified())
            {
                ReloadConfig();
            }
        }

        public void ReloadConfig()
        {
            if (File.Exists(this.absoluteFilePath))
            {
                XmlSerializerWrapper.FromXmlOverwrite(this.absoluteFilePath, this);
                Log.Out("[TimeLoop] Config Updated!");
            }
        }

        public void SaveConfig()
        {
            if (File.Exists(this.absoluteFilePath))
            {
                XmlSerializerWrapper.ToXml(this.absoluteFilePath, this);
                UpdateLastModified();
            }
        }

        public static implicit operator bool(XmlContentData instance)
        {
            return instance != null;
        }
    }
}