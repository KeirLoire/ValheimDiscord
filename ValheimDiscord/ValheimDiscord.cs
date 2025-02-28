using BepInEx;
using Discord;
using Discord.WebSocket;
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

        private Configuration _config;
        private DiscordSocketClient _client;

        public void Log(string message) => Debug.Log($"[{_pluginName}] {message}");

        public void Awake()
        {
            _config = new Configuration(Config);

            Log("Loaded.");

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
                _client.MessageReceived += MessageReceivedAsync;

                await _client.LoginAsync(TokenType.Bot, _config.DiscordBotToken);
                await _client.StartAsync();
            }
            catch (Exception exception)
            {
                Log("Error occured.");
                Log(exception.Message);
            }
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.IsBot)
                return;

            var user = message.Author.GlobalName;
            var content = message.Content;

            if (message.Content.StartsWith(_config.DiscordBotPrefix))
                HandleCommandAsync(message);
            else if (message.Channel.Id == _config.DiscordTextChannelId)
                SendIngameChat(user, content);

            Log($"{message.Author.GlobalName} sent {message.Content}");
        }

        private async Task HandleCommandAsync(SocketMessage message)
        {
        }

        private void SendIngameChat(string username, string message)
        {
            if (ZNet.instance == null)
            {
                Log("ZNet.instance is null.");
                return;
            }

            var userInfo = new UserInfo
            {
                Name = username,
                Gamertag = username,
                NetworkUserId = "Discord"
            };

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", Vector3.zero, 0, userInfo, message, userInfo.NetworkUserId);
        }
    }
}
