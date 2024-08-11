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
            Log.Out("[TimeLoop] Initializing ...");
            ModEvents.GameAwake.RegisterHandler(Awake);
            ModEvents.GameUpdate.RegisterHandler(Update);
            ModEvents.PlayerLogin.RegisterHandler(PlayerLogin);
            Log.Out("[TimeLoop] Initialized!");
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
                Serializer.Instance.CheckForUpdate();
                this.timeLooper?.Update();
            }
        }

        private bool PlayerLogin(ClientInfo cInfo, string message, StringBuilder stringBuild)
        {
            if (GameManager.Instance != null && GameManager.IsDedicatedServer && cInfo != null)
            {
                if (cInfo.CrossplatformId != null)
                {
                    if (Serializer.Instance.PlayerData?.Exists(x => x.CrossplatformId == cInfo.CrossplatformId.CombinedString) == false)
                    {
                        Serializer.Instance.PlayerData.Add(new PlayerData(cInfo));
                        #region WORKAROUND
                        // Workaround since something is broken here. With Unity I can usually use JsonUtility.ToJson to serialize lists of custom classes, not here though ....
                        Serializer.Instance.CrossplatformId.Add(cInfo.CrossplatformId.CombinedString);
                        Serializer.Instance.SkipTimeLoop.Add(false);
                        #endregion
                        Serializer.Instance.SaveConfig();
                        Log.Out($"[TimeLoop] Player added to config. {Serializer.Instance.PlayerData.Last().CrossplatformId}");
                    }
                }
            }

            return true;
        }
    }
}
