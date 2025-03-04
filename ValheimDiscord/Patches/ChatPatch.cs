using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace ValheimDiscord.Patches
{
    public sealed class ChatPatch
    {
        [HarmonyPatch]
        public static class RPC_ChatMessage
        {
            public static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("Chat");

                return AccessTools.Method(type, "RPC_ChatMessage");
            }

            public static void Postfix(Chat __instance, long sender, Vector3 position, int type, UserInfo userInfo, string text)
            {
                if (userInfo.UserId.m_platform != "Discord" && !string.IsNullOrWhiteSpace(text))
                {
                    ValheimDiscord.SendDiscordChat($"{userInfo.Name}: {text}");

                    ValheimDiscord.Log($"[In-game] {userInfo.Name} sent {text}");
                }
            }
        }
    }
}
