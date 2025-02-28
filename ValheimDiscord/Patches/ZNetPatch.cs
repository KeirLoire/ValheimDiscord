using HarmonyLib;
using System.Reflection;

namespace ValheimDiscord.Patches
{
    public sealed class ZNetPatch
    {
        [HarmonyPatch]
        public static class RPC_PeerInfo
        {
            public static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("ZNet");

                return AccessTools.Method(type, "RPC_PeerInfo");
            }

            public static void Postfix(ZNet __instance, ZRpc rpc, ZPackage pkg)
            {
                var type = __instance.GetType();
                var method = type.GetMethod("GetPeer", BindingFlags.NonPublic | BindingFlags.Instance);

                if (method == null)
                    return;

                var peer = method.Invoke(__instance, new object[] { rpc }) as ZNetPeer;

                if (peer == null)
                    return;

                if (string.IsNullOrWhiteSpace(peer.m_playerName))
                    return;

                ValheimDiscord.SendDiscordChat($"{peer.m_playerName} has joined the server.");
            }
        }

        [HarmonyPatch]
        public static class Disconnect
        {
            public static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("ZNet");

                return AccessTools.Method(type, "RPC_Disconnect");
            }

            public static void Prefix(ZNet __instance, ZRpc rpc)
            {
                var type = __instance.GetType();
                var method = type.GetMethod("GetPeer", BindingFlags.NonPublic | BindingFlags.Instance);

                if (method == null)
                    return;

                var peer = method.Invoke(__instance, new object[] { rpc }) as ZNetPeer;

                if (peer == null)
                    return;

                if (string.IsNullOrWhiteSpace(peer.m_playerName))
                    return;

                ValheimDiscord.SendDiscordChat($"{peer.m_playerName} has left the server.");
            }
        }
    }
}
