#define XML_SERIALIZATION

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
        public bool SkipTimeLoop;


        public PlayerData() 
        {
            this.ID = "12345678901234567";
            this.SkipTimeLoop = false;
        }


        public PlayerData(ClientInfo clientInfo)
        {
            if (clientInfo.PlatformId != null && clientInfo.PlatformId is UserIdentifierSteam)
            {
                UserIdentifierSteam steamID = clientInfo.PlatformId as UserIdentifierSteam;
                this.ID = steamID.SteamId.ToString();
            }
            else
            {
                this.ID = clientInfo.CrossplatformId.CombinedString;
            }
            this.SkipTimeLoop = false;
        }
    }
}
