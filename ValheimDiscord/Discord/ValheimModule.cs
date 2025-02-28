using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Color = Discord.Color;

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

        [Command("kick")]
        [Summary("Kick a player from the server.")]
        public async Task KickAsync([Summary("The username or Steam ID")]string player)
        {
            if (ZNet.instance == null)
            {
                ValheimDiscord.Log("ZNet.instance is null.");
                return;
            }

            var embedBuilder = CreateEmbedBuilder();

            if (ValheimDiscord.PluginConfig.DiscordAdminList.Contains(Context.User.Id.ToString()))
            {
                if (ZNet.instance.GetPlayerList().Any(x => x.m_name == player || x.m_host == player))
                {
                    ZNet.instance.Kick(player);

                    embedBuilder.AddField("[Status]", $"Player '{player}' has been kicked.");
                }
                else
                    embedBuilder.AddField("[Status]", $"Player '{player}' not found.");
            }
            else
                embedBuilder.AddField("[Status]", "You do not have permission to run this command.");

            await ReplyAsync(null, false, embedBuilder.Build());
        }

        [Command("ban")]
        [Summary("Ban a player from the server.")]
        public async Task BanAsync([Summary("The username or Steam ID")] string player)
        {
            if (ZNet.instance == null)
            {
                ValheimDiscord.Log("ZNet.instance is null.");
                return;
            }

            var embedBuilder = CreateEmbedBuilder();

            if (ValheimDiscord.PluginConfig.DiscordAdminList.Contains(Context.User.Id.ToString()))
            {
                if (!ZNet.instance.Banned.Contains(player))
                {
                    ZNet.instance.Ban(player);

                    embedBuilder.AddField("[Status]", $"Player '{player}' has been banned.");
                }
                else
                {
                    embedBuilder.AddField("[Status]", $"Player '{player}' has already been banned.");
                }
            }
            else
                embedBuilder.AddField("[Status]", "You do not have permission to run this command.");

            await ReplyAsync(null, false, embedBuilder.Build());
        }

        [Command("unban")]
        [Summary("Unban a player from the server.")]
        public async Task UnbanAsync([Summary("The username or Steam ID")] string player)
        {
            if (ZNet.instance == null)
            {
                ValheimDiscord.Log("ZNet.instance is null.");
                return;
            }

            var embedBuilder = CreateEmbedBuilder();

            if (ValheimDiscord.PluginConfig.DiscordAdminList.Contains(Context.User.Id.ToString()))
            {
                if (ZNet.instance.Banned.Contains(player))
                {
                    ZNet.instance.Unban(player);

                    embedBuilder.AddField("[Status]", $"Player '{player}' has been unbanned.");
                }
                else
                {
                    embedBuilder.AddField("[Status]", $"Player '{player}' has already been unbanned.");
                }
            }
            else
                embedBuilder.AddField("[Status]", "You do not have permission to run this command.");

            await ReplyAsync(null, false, embedBuilder.Build());
        }

        [Command("broadcast")]
        [Summary("Broadcast a message to the server.")]
        public async Task BroadcastAsync([Summary("The message")] string message)
        {
            if (ZNet.instance == null)
            {
                ValheimDiscord.Log("ZNet.instance is null.");
                return;
            }

            var embedBuilder = CreateEmbedBuilder();

            if (ValheimDiscord.PluginConfig.DiscordAdminList.Contains(Context.User.Id.ToString()))
            {
                ValheimUtils.BroadcastMessage(message);
                ValheimUtils.SendIngameChat(message);

                embedBuilder.AddField("[Status]", $"Message '{message}' has been sent.");
            }
            else
                embedBuilder.AddField("[Status]", "You do not have permission to run this command.");

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
