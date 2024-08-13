namespace TimeLoop.Functions
{
    public static class Message
    {
        public static void SendChat(string message)
        {
            GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, message, null, EMessageSender.None);

        }
    }
}
