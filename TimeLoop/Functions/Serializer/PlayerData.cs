using Platform.Steam;
using System;
using System.Xml.Serialization;

namespace TimeLoop.Functions
{
#if !XML_SERIALIZATION
    [Serializable]
#endif
    public class PlayerData
    {
#if XML_SERIALIZATION
        [XmlAttribute]
#endif
        public string ID;

#if XML_SERIALIZATION
        [XmlAttribute]
#endif
        public string PlayerName;

#if XML_SERIALIZATION
        [XmlAttribute]
#endif
        public bool SkipTimeLoop;


        public PlayerData() 
        {
            this.ID = "12345678901234567";
            this.PlayerName = "John Doe";
            this.SkipTimeLoop = false;            
        }


        public PlayerData(ClientInfo clientInfo)
        {
            this.ID = clientInfo.CrossplatformId.CombinedString;
            this.PlayerName = clientInfo.playerName;
            this.SkipTimeLoop = false;
        }
    }
}
