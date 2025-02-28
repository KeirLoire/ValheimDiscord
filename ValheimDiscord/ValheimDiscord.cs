using BepInEx;
using Discord;
using Discord.WebSocket;
using System;
using System.Threading;
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

        private Configuration _config;
        private DiscordSocketClient _client;

        public void Log(string message) => Debug.Log($"[{_pluginName}] {message}");

        public void Awake()
        {
            _config = new Configuration(Config);

            this.Log("Loaded.");

            var thread = new Thread(StartClientAsync);
            thread.IsBackground = true;
            thread.Start();
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

                await _client.LoginAsync(TokenType.Bot, _config.DiscordBotToken);
                await _client.StartAsync();
            }
            catch (Exception exception)
            {
                this.Log("Error occured.");
                this.Log(exception.Message);
            }
        }
    }
}
