using UnityEngine;

namespace ValheimDiscord
{
    public static class Utils
    {
        public static void SendIngameChat(string message, string username = "Server")
        {
            if (ZNet.instance == null)
            {
                ValheimDiscord.Log("ZNet.instance is null.");
                return;
            }

            var userInfo = new UserInfo()
            {
                Name = $"<color=#7289da>{username}</color>",
                Gamertag = username,
                NetworkUserId = "Discord"
            };

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", Vector3.zero, 0, userInfo, message, userInfo.NetworkUserId);
        }

        public static void BroadcastMessage(string message)
        {
            if (ZNet.instance == null)
            {
                ValheimDiscord.Log("ZNet.instance is null.");
                return;
            }

            var userInfo = new UserInfo()
            {
                Name = "Server",
                Gamertag = "Server",
                NetworkUserId = "Discord"
            };

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ShowMessage", 2, message);
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", Vector3.zero, 0, userInfo, message, userInfo.NetworkUserId);
        }
    }
}
