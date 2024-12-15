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
            ModEvents.PlayerDisconnected.RegisterHandler(PlayerDisconnected);
            //SdtdConsole.Instance.RegisterCommands();
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
                        contentData.PlayerData.Add(new TimeLoop.Functions.PlayerData(cInfo));
                        contentData.SaveConfig();
                        if (contentData.EnableTimeLooper) Message.SendPrivateChat($"Time loop is active. Therefore the time will reset every 24 hours until the precontition is met.", cInfo);
                        Log.Out($"[TimeLoop] Player added to config. {contentData.PlayerData.Last().ID}");
                    }
                }
            }

            return true;
        }

        private void PlayerDisconnected(ClientInfo cInfo, bool becauseShutdown)
        {
            if (GameManager.Instance != null && GameManager.IsDedicatedServer && cInfo != null)
            {
                if (cInfo.CrossplatformId != null)
                {
                    // TODO: Player disconnect created new party with new leader
                    if(GameManager.Instance.World.Players.dict.TryGetValue(cInfo.entityId, out EntityPlayer disconnectedPlayer) && disconnectedPlayer == disconnectedPlayer.party?.Leader)
                    {
                        List<int> partyMembers = new List<int>();
                        foreach (EntityPlayer player in disconnectedPlayer.party.MemberList)
                        {
                            if (player == disconnectedPlayer) continue;

                            partyMembers.Add(player.entityId);
                        }
                        PartyManager.Current.CreateClientParty(GameManager.Instance.World, PartyManager.Current.nextPartyID, 0, partyMembers.ToArray(), disconnectedPlayer.party.VoiceLobbyId);
                    }

                    // TODO: Window coloring
                    //foreach (var window in GameManager.Instance.windowManager.windows)
                    //{
                    //    Log.Out($"[TimeLoop] Time Reset {window.}.");
                    //}
                    //GameManager.Instance.nguiWindowManager.InGameHUD.
                    //GameManager.Instance.nguiWindowManager.playerUI
                    //GameManager.Instance.nguiWindowManager.Windows
                    //GameManager.Instance.nguiWindowManager.WindowManager.

                    // TODO: Respawn randomly somewhere on map and not at backpack
                    //GameEventManager.Current.
                    //GameOptionsManager.
                }
            }
        }
    }
}
