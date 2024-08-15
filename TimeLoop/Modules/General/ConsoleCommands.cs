#if XML_SERIALIZATION
using ContentData = TimeLoop.Functions.XmlContentData;
#else
using ContentData = TimeLoop.Functions.JsonContentData;
#endif
using System.Collections.Generic;
using TimeLoop.Functions;

namespace TimeLoop.Modules
{
    public class EnableTimeLoop : IConsoleCommand
    {
        public static ContentData ContentData;

        public bool IsExecuteOnClient => false;

        public int DefaultPermissionLevel => 0;

        public bool AllowedInMainMenu => false;

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
                            ContentData.mode = ContentData.Mode.DISABLED;
                            break;
                        case "whitelist":
                            ContentData.mode = ContentData.Mode.WHITELIST;
                            break;
                        case "threshold":
                            ContentData.mode = ContentData.Mode.MIN_PLAYER_COUNT;
                            break;
                    }
                    break;
                case "player":
                    switch (commandParams[1])
                    {
                        case "min":
                            int.TryParse(commandParams[2], out ContentData.MinPlayers);
                            break;
                        case "auth":
                            PlayerData player = ContentData.PlayerData.Find(x => x.PlayerName == commandParams[2]);
                            if (player != null) player.SkipTimeLoop = true;
                            break;
                        case "refuse":
                            player = ContentData.PlayerData.Find(x => x.PlayerName == commandParams[2]);
                            if (player != null) player.SkipTimeLoop = false;
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
            return "Syntax is: [timeloop|tilo] [enable|disable|mode|player] [none|whitelist|threshold|min|auth|refuse]";
        }
    }

}
