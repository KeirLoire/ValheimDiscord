using Splatform;
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
                UserId = new PlatformUserID("Discord", username)
            };

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", Vector3.zero, 0, userInfo, message);
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
                Name = "<color=#900D09>Server</color>",
                UserId = new PlatformUserID("Discord", "Server")
            };

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ShowMessage", 2, message);
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", Vector3.zero, 0, userInfo, message);
        }
    }
}
