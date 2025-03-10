﻿using BepInEx;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HarmonyLib;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ValheimDiscord.Discord;
using ValheimDiscord.Model;

namespace ValheimDiscord
{
    [BepInPlugin(_pluginGuid, _pluginName, _pluginVersion)]
    public sealed class ValheimDiscord : BaseUnityPlugin
    {
        private const string _pluginGuid = "com.keirloire.valheimdiscord";
        private const string _pluginName = "ValheimDiscord";
        private const string _pluginVersion = "1.0.0";

        private static DiscordSocketClient _client;
        private static CommandService _commands;

        public static Configuration PluginConfig;

        public static void Log(string message) => Debug.Log($"[{_pluginName}] {message}");

        public void Awake()
        {
            PluginConfig = new Configuration(Config);

            var harmony = new Harmony(_pluginGuid);
            harmony.PatchAll();

            Log("Loaded.");

            var thread = new Thread(StartClientAsync);
            thread.IsBackground = true;
            thread.Start();
        }

        public static async void SendDiscordChat(string message)
        {
            var channel = await _client.GetChannelAsync(PluginConfig.DiscordTextChannelId) as IMessageChannel;

            if (channel == null)
                return;

            await channel.SendMessageAsync(message);
        }

        private async void StartClientAsync()
        {
            try
            {
                var discordConfig = new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
                    RestClientProvider = MonoRestClientProvider.Instance
                };

                _client = new DiscordSocketClient(discordConfig);
                _client.MessageReceived += MessageReceivedAsync;

                _commands = new CommandService();
                await _commands.AddModuleAsync<ValheimModule>(null);

                await _client.LoginAsync(TokenType.Bot, PluginConfig.DiscordBotToken);
                await _client.StartAsync();
            }
            catch (Exception exception)
            {
                Log("Error occured.");
                Log(exception.Message);
            }
        }

        private async Task MessageReceivedAsync(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;

            if (message == null)
                return;

            if (message.Author.IsBot)
                return;

            var user = message.Author.GlobalName;
            var content = message.Content;

            if (message.Content.StartsWith(PluginConfig.DiscordBotPrefix))
            {
                var context = new SocketCommandContext(_client, message);

                await _commands.ExecuteAsync(context, PluginConfig.DiscordBotPrefix.Length, null);
            }
            else if (message.Channel.Id == PluginConfig.DiscordTextChannelId)
                Utils.SendIngameChat(content, user);

            Log($"[Discord] {message.Author.GlobalName} sent {message.Content}");
        }
    }
}
