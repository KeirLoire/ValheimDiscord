using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ValheimDiscord.Discord
{
    public sealed class ValheimModule : ModuleBase<SocketCommandContext>
    {
        [Command("playerlist")]
        [Summary("List all players in the server.")]
        public async Task PlayerListAsync()
        {
            if (ZNet.instance == null)
            {
                ValheimDiscord.Log("ZNet.instance is null.");
                return;
            }

            var players = ZNet.instance
                .GetPlayerList()
                .Select(x => x.m_name)
                .OrderByDescending(x => x)
                .ToArray();

            var embedBuilder = CreateEmbedBuilder();

            embedBuilder.AddField("[Players]", players.Count() > 0
                ? $"Players: {string.Join("\n", players)}"
                : "There are no players online.");

            await ReplyAsync(null, false, embedBuilder.Build());
        }

        private EmbedBuilder CreateEmbedBuilder()
        {
            var embedBuilder = new EmbedBuilder()
            {
                Color = Color.DarkOrange,
                Author = new EmbedAuthorBuilder()
                {
                    Name = ValheimDiscord.PluginConfig.ServerName,
                    IconUrl = ValheimDiscord.PluginConfig.ServerImageUrl
                },
                Timestamp = DateTimeOffset.Now
            };

            if (!string.IsNullOrWhiteSpace(ValheimDiscord.PluginConfig.ServerUrl))
            {
                embedBuilder.Footer = new EmbedFooterBuilder()
                {
                    Text = ValheimDiscord.PluginConfig.ServerUrl
                };
            }

            return embedBuilder;
        }
    }
}
