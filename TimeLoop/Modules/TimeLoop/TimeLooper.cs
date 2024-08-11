using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Remoting.Messaging;
using TimeLoop.Functions;


namespace TimeLoop.Modules
{
    public class TimeLooper
    {
        public TimeLooper()
        {

        }

        public void Update()
        {
            //Log.Out("Game Update!");


            List<ClientInfo> clients = GetConnectedClients();
            for (int i = 0; i < clients.Count; i++)
            {
                ClientInfo cInfo = clients[i];
                if (cInfo == null || !cInfo.loginDone || cInfo.CrossplatformId == null || cInfo.PlatformId == null)
                {
                    continue;
                }

                PlayerData data = Serializer.Instance.PlayerData.Find(x => x.clientInfo.PlatformId.CombinedString == cInfo.PlatformId.CombinedString 
                                                                        || x.clientInfo.CrossplatformId.CombinedString == cInfo.CrossplatformId.CombinedString);
                if (data?.skipTimeLoop == true)
                {
                    return;
                }
            }


            ulong dayTime = GameManager.Instance.World.GetWorldTime() % 24000;
            if (dayTime == 0)
            {
                Message.SendChat("Resetting day. Please wait for an admin in order to experience the normal time flow! Type !adminlist to see available admins.");
                int previousDay = GameUtils.WorldTimeToDays(GameManager.Instance.World.GetWorldTime()) - 1;
                GameManager.Instance.World.SetTime(GameUtils.DaysToWorldTime(previousDay) + 1);
            }
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
    }
}
