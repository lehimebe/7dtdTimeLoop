using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
using TimeLoop.Modules;
using TimeLoop.Functions;
using static UnityDistantTerrain;

namespace TimeLoop
{
    public class Main : IModApi
    {
        private TimeLooper timeLooper;

        public void InitMod(Mod _modInstance)
        {
            ModEvents.GameAwake.RegisterHandler(Awake);
            ModEvents.GameUpdate.RegisterHandler(Update);
            ModEvents.PlayerLogin.RegisterHandler(PlayerLogin);
        }

        private void Awake()
        {
            if (GameManager.Instance != null && GameManager.IsDedicatedServer)
            {
                if (Serializer.Instance.EnableTimeLooper) this.timeLooper = new TimeLooper();
            }
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.IsDedicatedServer)
            {
                this.timeLooper?.Update();
            }
        }

        private bool PlayerLogin(ClientInfo cInfo, string message, StringBuilder stringBuild)
        {
            if (GameManager.Instance != null && GameManager.IsDedicatedServer && cInfo != null && cInfo.loginDone)
            {
                if (cInfo.CrossplatformId != null)
                {
                    if (!Serializer.Instance.PlayerData.Exists(x => x.clientInfo.CrossplatformId.CombinedString == cInfo.CrossplatformId.CombinedString))
                    {
                        Serializer.Instance.PlayerData.Add(new PlayerData(cInfo));
                        Serializer.Instance.SaveConfig();
                    }
                }
            }

            return true;
        }
    }
}
