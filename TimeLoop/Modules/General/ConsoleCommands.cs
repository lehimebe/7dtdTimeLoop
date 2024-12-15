#if XML_SERIALIZATION
using ContentData = TimeLoop.Functions.XmlContentData;
#else
using ContentData = TimeLoop.Functions.JsonContentData;
#endif
using System.Collections.Generic;
using TimeLoop.Functions;
using Platform;

namespace TimeLoop.Modules
{
    public class EnableTimeLoop : IConsoleCommand
    {
        public static ContentData ContentData;

        public bool IsExecuteOnClient => false;

        public int DefaultPermissionLevel => 0;

        public bool AllowedInMainMenu => false;

        public DeviceFlag AllowedDeviceTypes => DeviceFlag.StandaloneWindows;

        public DeviceFlag AllowedDeviceTypesClient => DeviceFlag.StandaloneWindows;

        public void Execute(List<string> commandParams, CommandSenderInfo senderInfo)
        {
            if (!ContentData)
            {
                Log.Warning("Data was not loaded.");
                return;
            }

            switch (commandParams[0])
            {
                case "enable":
                    ContentData.EnableTimeLooper = true;
                    Log.Out("[TimeLoop] enabled!");
                    break;
                case "disable":
                    ContentData.EnableTimeLooper = false;
                    Log.Out("[TimeLoop] disabled!");
                    break;
                case "mode":
                    switch (commandParams[1])
                    {
                        case "none":
                        case "0":
                            ContentData.mode = ContentData.Mode.DISABLED;
                            Log.Out("[TimeLoop] Mod disabled!");
                            break;
                        case "whitelist":
                        case "1":
                            ContentData.mode = ContentData.Mode.WHITELIST;
                            Log.Out("[TimeLoop] Whitelist Mode enabled!");
                            break;
                        case "threshold":
                        case "2":
                            ContentData.mode = ContentData.Mode.MIN_PLAYER_COUNT;
                            Log.Out("[TimeLoop] Threshold Mode enabled!");
                            break;
                        case "whitelisted_threshold":
                        case "3":
                            ContentData.mode = ContentData.Mode.MIN_WHITELIST_PLAYER_COUNT;
                            Log.Out("[TimeLoop] Whitelisted Threshold Mode enabled!");
                            break;
                    }
                    break;
                case "player":
                    switch (commandParams[1])
                    {
                        case "min":
                            int.TryParse(commandParams[2], out ContentData.MinPlayers);
                            Log.Out($"[TimeLoop] Min player count has been set to {commandParams[2]}!");
                            break;
                        case "auth":
                            TimeLoop.Functions.PlayerData player = ContentData.PlayerData.Find(x => x.PlayerName == commandParams[2]);
                            if (player != null) player.SkipTimeLoop = true;
                            Log.Out($"[TimeLoop] Player {commandParams[2]} has been authorized!");
                            break;
                        case "refuse":
                            player = ContentData.PlayerData.Find(x => x.PlayerName == commandParams[2]);
                            if (player != null) player.SkipTimeLoop = false;
                            Log.Out($"[TimeLoop] Player {commandParams[2]} has been unauthorized!");
                            break;
                    }
                    break;
            }

            ContentData.SaveConfig();
        }

        public string[] GetCommands()
        {
            return new string[] { 
                    "timeloop",
                    "tilo"
                };
        }

        public string GetDescription()
        {
            return "Enabled funcionalities of the time loop mod";
        }

        public string GetHelp()
        {
            return "Syntax is: [timeloop|tilo] [(enable|disable)|mode|player] [(none|whitelist|threshold|whitelisted_threshold)|(min|auth|refuse)]";
        }
    }

}
