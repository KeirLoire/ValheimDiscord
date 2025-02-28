using BepInEx.Configuration;

namespace ValheimDiscord.Model
{
    public sealed class Configuration
    {
        private readonly ConfigFile _configFile;
        private readonly ConfigEntry<string> _discordBotToken;
        private readonly ConfigEntry<string> _discordBotPrefix;

        public string DiscordBotToken => _discordBotToken.Value;
        public string DiscordBotPrefix => _discordBotPrefix.Value;

        public Configuration(ConfigFile configFile)
        {
            _configFile = configFile;

            _discordBotToken = _configFile.Bind("General", "DiscordBotToken", "", "Discord bot token.");
            _discordBotPrefix = _configFile.Bind("General", "DiscordBotPrefix", "$", "Discord bot prefix for running commands.");

            _configFile.Save();
        }
    }
}
