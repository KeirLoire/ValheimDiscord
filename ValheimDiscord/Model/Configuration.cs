using BepInEx.Configuration;

namespace ValheimDiscord.Model
{
    public sealed class Configuration
    {
        private readonly ConfigFile _configFile;
        private readonly ConfigEntry<string> _discordBotToken;
        private readonly ConfigEntry<string> _discordBotPrefix;
        private readonly ConfigEntry<string> _discordTextChannelId;

        public string DiscordBotToken => _discordBotToken.Value;
        public string DiscordBotPrefix => _discordBotPrefix.Value;
        public ulong DiscordTextChannelId => ulong.TryParse(_discordTextChannelId.Value, out ulong result) ? result : 0;

        public Configuration(ConfigFile configFile)
        {
            _configFile = configFile;

            _discordBotToken = _configFile.Bind("General", "DiscordBotToken", "", "Discord bot token.");
            _discordBotPrefix = _configFile.Bind("General", "DiscordBotPrefix", "$", "Discord bot prefix for running commands.");
            _discordTextChannelId = _configFile.Bind("General", "DiscordTextChannelId", "", "Discord text channel for listening chats.");

            _configFile.Save();
        }
    }
}
