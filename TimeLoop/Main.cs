#if XML_SERIALIZATION
using ContentData = TimeLoop.Functions.XmlContentData;
#else
using ContentData = TimeLoop.Functions.JsonContentData;
#endif
using System.Linq;
using System.Text;
using TimeLoop.Modules;
using TimeLoop.Functions;
using Platform.Steam;
using System.Collections.Generic;

namespace TimeLoop
{
    public class Main : IModApi
    {
        private TimeLooper timeLooper;
        private ContentData contentData;

        public void InitMod(Mod _modInstance)
        {
            Log.Out("[TimeLoop] Initializing ...");
            ModEvents.GameAwake.RegisterHandler(Awake);
            ModEvents.GameUpdate.RegisterHandler(Update);
            ModEvents.PlayerLogin.RegisterHandler(PlayerLogin);
            //SdtdConsole.Instance.RegisterCommands();
            Log.Out("[TimeLoop] Initialized!");
        }

        private void Awake()
        {
            if (GameManager.Instance != null && GameManager.IsDedicatedServer && !this.contentData)
            {
                // General Initialization
                this.contentData = ContentData.DeserializeInstance();
                EnableTimeLoop.ContentData = contentData;

                // Modules
                this.timeLooper = new TimeLooper(contentData);
            }
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.IsDedicatedServer)
            {
                contentData?.CheckForUpdate();
                if (contentData.EnableTimeLooper) this.timeLooper?.Update();
            }
        }

        private bool PlayerLogin(ClientInfo cInfo, string message, StringBuilder stringBuild)
        {
            if (GameManager.Instance != null && GameManager.IsDedicatedServer && cInfo != null)
            {
                if (cInfo.CrossplatformId != null)
                {
                    if (contentData?.PlayerData?.Exists(
                        x => (x.ID == cInfo.CrossplatformId.CombinedString)
                        || (cInfo.PlatformId != null && cInfo.PlatformId is UserIdentifierSteam && x.ID == (cInfo.PlatformId as UserIdentifierSteam).SteamId.ToString()))
                        == false)
                    {
                        contentData.PlayerData.Add(new PlayerData(cInfo));
                        contentData.SaveConfig();
                        Message.SendPrivateChat("Resetting day. Please wait for an admin in order to experience the normal time flow! Type !adminlist to see available admins.", cInfo);
                        Log.Out($"[TimeLoop] Player added to config. {contentData.PlayerData.Last().ID}");
                    }
                }
            }

            return true;
        }
    }
}
