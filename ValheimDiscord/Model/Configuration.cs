using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace ValheimDiscord.Model
{
    public sealed class Configuration
    {
        private readonly ConfigFile _configFile;
        private readonly ConfigEntry<string> _discordBotToken;
        private readonly ConfigEntry<string> _discordBotPrefix;
        private readonly ConfigEntry<string> _discordTextChannelId;
        private readonly ConfigEntry<string> _discordAdminList;
        private readonly ConfigEntry<string> _serverName;
        private readonly ConfigEntry<string> _serverImageUrl;
        private readonly ConfigEntry<string> _serverUrl;

        public string DiscordBotToken => _discordBotToken.Value;
        public string DiscordBotPrefix => _discordBotPrefix.Value;
        public ulong DiscordTextChannelId => ulong.TryParse(_discordTextChannelId.Value, out ulong result) ? result : 0;
        public List<string> DiscordAdminList
        {
            get => _discordAdminList.Value.Split(',').ToList();
            set => _discordAdminList.Value = string.Join(",", value);
        }
        public string ServerName => _serverName.Value;
        public string ServerImageUrl => _serverImageUrl.Value;
        public string ServerUrl => _serverUrl.Value;

        public Configuration(ConfigFile configFile)
        {
            _configFile = configFile;

            _discordBotToken = _configFile.Bind("General", "DiscordBotToken", "", "Discord bot token.");
            _discordBotPrefix = _configFile.Bind("General", "DiscordBotPrefix", "$", "Discord bot prefix for running commands.");
            _discordTextChannelId = _configFile.Bind("General", "DiscordTextChannelId", "", "Discord text channel for syncing in-game chats.");
            _discordAdminList = _configFile.Bind("General", "DiscordAdminList", "", "Discord list of admins.");
            _serverName = _configFile.Bind("General", "ServerName", "Valheim Server", "Server name to display in embed responses.");
            _serverImageUrl = _configFile.Bind("General", "ServerImageUrl", "https://pbs.twimg.com/profile_images/1355087978033504259/ghm06InW_400x400.png", "Server icon to display in embed responses.");
            _serverUrl = _configFile.Bind("General", "ServerUrl", "", "Server URL to display in embed responses.");

            _configFile.Save();
        }
    }
}
