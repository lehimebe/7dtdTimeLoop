using System.Collections.Generic;

namespace TimeLoop.Functions
{
    public static class Message
    {
        public static void SendGlobalChat(string message)
        {
            GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, message, null, EMessageSender.Server);
        }

        public static void SendPrivateChat(string message, ClientInfo recipient)
        {
            GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, message, new List<int>{ recipient.entityId }, EMessageSender.Server);
        }
    }
}
