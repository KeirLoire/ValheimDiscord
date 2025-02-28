using Discord.Commands;
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
                .ToArray();

            await ReplyAsync(players.Count() > 0 
                ? $"Players: {string.Join(", ", players)}" 
                : "There are no players online.");
        }
    }
}
