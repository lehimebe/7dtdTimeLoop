#if XML_SERIALIZATION
using ContentData = TimeLoop.Functions.XmlContentData;
#else
using ContentData = TimeLoop.Functions.JsonContentData;
#endif
using System.Linq;
using System.Collections.Generic;
using TimeLoop.Functions;
using Platform.Steam;


namespace TimeLoop.Modules
{
    public class TimeLooper
    {
        ContentData contentData;
        double unscaledTimeStamp;

        public TimeLooper(ContentData contentData)
        {
            this.contentData = contentData;
        }

        public void Update()
        {
            bool updateTime;

            switch (this.contentData.mode)
            { 
                case ContentData.Mode.WHITELIST:
                    updateTime = CheckIfPlayerOnline();
                    break;
                case ContentData.Mode.MIN_PLAYER_COUNT:
                    updateTime = CheckPlayerCount();
                    break;
                default:
                    updateTime = false;
                    break;
            }

            if (updateTime && unscaledTimeStamp != UnityEngine.Time.unscaledTimeAsDouble)
            {
                ulong worldTime = GameManager.Instance.World.GetWorldTime();
                ulong dayTime = worldTime % 24000;
                if (dayTime == 0)
                {
                    Log.Out("[TimeLoop] Time Reset.");
                    Message.SendGlobalChat($"Resetting day. Please wait for authorized personnel or enough players to stop the time loop.");
                    int previousDay = GameUtils.WorldTimeToDays(worldTime) - 1;
                    GameManager.Instance.World.SetTime(GameUtils.DaysToWorldTime(previousDay) + 2);
                }

                unscaledTimeStamp = UnityEngine.Time.unscaledTimeAsDouble;
            }
        }

        private bool CheckIfPlayerOnline()
        {
            List<ClientInfo> clients = GetConnectedClients();
            for (int i = 0; i < clients.Count; i++)
            {
                ClientInfo cInfo = clients[i];
                if (cInfo == null || !cInfo.loginDone || cInfo.CrossplatformId == null || cInfo.PlatformId == null)
                {
                    continue;
                }

                PlayerData data = this.contentData.PlayerData?.Find
                        (
                        x => x.ID == cInfo.CrossplatformId.CombinedString ||
                        (cInfo.PlatformId is UserIdentifierSteam &&
                        x.ID == (cInfo.PlatformId as UserIdentifierSteam).SteamId.ToString())
                        );
                if (data?.SkipTimeLoop == true)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckPlayerCount()
        {
            List<ClientInfo> clients = GetConnectedClients();
            return clients.Count < this.contentData.MinPlayers;
        }

        private List<ClientInfo> GetConnectedClients()
        {
            if (ConnectionManager.Instance.Clients != null && ConnectionManager.Instance.Clients.Count > 0)
            {
                return ConnectionManager.Instance.Clients.List.ToList();
            }
            else
            {
                return new List<ClientInfo>();
            }
        }


        public static implicit operator bool(TimeLooper instance)
        {
            return instance != null;
        }
    }
}
