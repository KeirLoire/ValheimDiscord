using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
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

        [HarmonyPatch(typeof(ZNet))]
        [HarmonyPatch(nameof(ZNet.Disconnect))]
        [HarmonyPatch(new[] { typeof(ZNetPeer) })]
        public static class Disconnect
        {
            public static void Prefix(ZNet __instance, ZNetPeer peer)
            {
                if (peer == null)
                    return;

                if (string.IsNullOrWhiteSpace(peer.m_playerName))
                    return;

                var type = __instance.GetType();
                var field = type.GetField("m_peers", BindingFlags.NonPublic | BindingFlags.Instance);
                var peers = field.GetValue(__instance) as List<ZNetPeer>;

                if (peers.Any(p => p.m_uid == peer.m_uid))
                    ValheimDiscord.SendDiscordChat($"{peer.m_playerName} has left the server.");
            }
        }
    }
}
