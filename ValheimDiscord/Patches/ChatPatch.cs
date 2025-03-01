﻿using HarmonyLib;
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

            public static void Postfix(Chat __instance, long sender, Vector3 position, int type, UserInfo userInfo, string text, string senderAccountId)
            {
                if (userInfo.NetworkUserId != "Discord" && !string.IsNullOrWhiteSpace(text))
                {
                    ValheimDiscord.SendDiscordChat($"{userInfo.Name}: {text}");

                    ValheimDiscord.Log($"[INGAME]{userInfo.Name} sent {text}");
                }
            }
        }
    }
}
