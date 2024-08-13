using System.Linq;
using System.Text;
using TimeLoop.Modules;
using TimeLoop.Functions;
using Platform.Steam;

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
                    if (Serializer.Instance.PlayerData?.Exists(
                        x => (cInfo.PlatformId is UserIdentifierSteam
                        && x.ID == (cInfo.PlatformId as UserIdentifierSteam).SteamId.ToString()) 
                        || x.ID == cInfo.CrossplatformId.CombinedString) == false)
                    {
                        Serializer.Instance.PlayerData.Add(new PlayerData(cInfo));
                        Serializer.Instance.SaveConfig();
                        Log.Out($"[TimeLoop] Player added to config. {Serializer.Instance.PlayerData.Last().ID}");
                    }
                }
            }

            return true;
        }
    }
}
