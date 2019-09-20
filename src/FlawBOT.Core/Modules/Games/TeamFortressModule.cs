using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Group("tf2")]
    [Description("Commands related to Team Fortress 2")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class TeamFortressModule : BaseCommandModule
    {
        #region COMMAND_SCHEMA

        [Command("item")]
        [Aliases("schema")]
        [Description("Retrieve an item from the latest TF2 item schema")]
        public async Task TF2Item(CommandContext ctx,
            [Description("Item to find in the TF2 schema")] [RemainingText] string query = "The Scattergun")
        {
            var item = TeamFortressService.GetSchemaItemAsync(query);
            if (item == null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing);
            else
            {
                var textInfo = new CultureInfo("en-US", false).TextInfo;
                var output = new DiscordEmbedBuilder()
                    .WithTitle(item.ItemName)
                    .WithDescription((!string.IsNullOrWhiteSpace(item.Description)) ? item.Description : "")
                    .AddField("Giftable", (item.Capabilities.Giftable) ? "Yes" : "No", true)
                    .AddField("Nameable", (item.Capabilities.Renamable) ? "Yes" : "No", true)
                    .AddField("Item Slot:", (!string.IsNullOrWhiteSpace(item.ItemSlot)) ? textInfo.ToTitleCase(item.ItemSlot) : "Unknown", true)
                    .WithImageUrl(item.ImageURL)
                    .WithUrl("https://wiki.teamfortress.com/wiki/" + item.ItemName.Replace(' ', '_'))
                    .WithFooter("ID: " + item.DefIndex)
                    .WithColor(new DiscordColor("#E7B53B"));

                var userClasses = new StringBuilder();
                foreach (var className in item.UsedByClasses)
                    userClasses.Append(className + "\n");
                output.AddField("Worn by:", userClasses.ToString() ?? "Unknown", true);

                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_SCHEMA

        #region COMMAND_MAP

        [Command("map")]
        [Aliases("maps")]
        [Description("Retrieve map information from teamwork.tf")]
        public async Task TF2Map(CommandContext ctx,
            [Description("Normalized map name, like pl_upward")] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = await TeamFortressService.GetMapStatsAsync(query.ToLowerInvariant());
            if (results.MapName == null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing);
            else
            {
                double.TryParse(results.AvgPlayers, out var avg_players);
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.MapName)
                    .AddField("Official", results.OfficialMap ? "YES" : "NO", true)
                    .AddField("Game Mode", results.GameModes.First() ?? "Unknown", true)
                    .AddField("First Seen", results.FirstSeen.ToString() ?? "Unknown", true)
                    .AddField("Avg. Players", Math.Round(avg_players, 2).ToString() ?? "Unknown", true)
                    .AddField("Highest Player Count", results.HighestPlayerCount.ToString() ?? "Unknown", true)
                    .AddField("Highest Server Count", results.HighestServerCount.ToString() ?? "Unknown", true)
                    .WithFooter("Statistics retrieved from teamwork.tf - refreshed every 5 minutes")
                    .WithImageUrl(results.Thumbnail)
                    .WithUrl("https://wiki.teamfortress.com/wiki/" + results.MapName)
                    .WithColor(new DiscordColor("#E7B53B"));

                var related_maps = new StringBuilder();
                foreach (var map in results.RelatedMaps.Take(5))
                    related_maps.Append(map + "\n");
                if (related_maps.Length > 0)
                    output.AddField("Related Map(s)", related_maps.ToString(), true);

                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_MAP

        #region COMMAND_NEWS

        [Command("news")]
        [Description("Retrieve the latest news article from teamwork.tf")]
        public async Task TF2News(CommandContext ctx)
        {
            var results = await TeamFortressService.GetNewsOverviewAsync();
            if (results == null || results.Count == 0)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing);
            else
            {
                while (results.Count > 0)
                {
                    var output = new DiscordEmbedBuilder()
                        .WithFooter("These are the latest news articles retrieved from teamwork.tf\nType next in the next 10 seconds for more news articles.")
                        .WithColor(new DiscordColor("#E7B53B"));

                    foreach (var result in results.Take(5))
                    {
                        output.AddField(result.Title, result.Link);
                        results.Remove(result);
                    }
                    var message = await ctx.RespondAsync(embed: output.Build());

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
                    if (interactivity.Result == null) break;
                    await BotServices.RemoveMessage(interactivity.Result);
                    await BotServices.RemoveMessage(message);
                }
            }
        }

        #endregion COMMAND_NEWS

        #region COMMAND_SERVERS

        [Command("server")]
        [Aliases("servers")]
        [Description("Retrieve a list of servers with given game-mode")]
        public async Task TF2Servers(CommandContext ctx,
            [Description("Name of the game-mode, like payload")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            query = TeamFortressService.NormalizedGameMode(query);
            var results = await TeamFortressService.GetServersAsync(query.Trim().Replace(' ', '-'));
            if (results.Count <= 0)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing);
            else
            {
                var random = new Random();
                results = results.OrderBy(x => random.Next()).ToList();
                foreach (var server in results.Where(n => n.GameModes.Contains(query)))
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(server.Name)
                        .WithDescription("steam://connect/" + server.IP + ":" + server.Port)
                        .AddField("Secure", server.ValveSecure ? "YES" : "NO", true)
                        //.AddField("Password", server.has_password ? "YES" : "NO", true)
                        .AddField("Max Players", (server.PlayerCount.ToString() ?? "Unknown") + "/" + (server.PlayerMax.ToString() ?? "Unknown"), true)
                        .AddField("Current Map", server.MapName ?? "Unknown", true)
                        .AddField("Next Map", server.NextMap ?? "Unknown", true)
                        .AddField("Provider", server.Provider ?? "Unknown", true)
                        .AddField("Roll the Dice>", server.HasRTD ? "Yes" : "No", true)
                        .AddField("Random Crits", server.HasRandomCrits == true ? "Yes" : "No", true)
                        .AddField("Respawn Timer", server.HasNoSpawnTimer ? "Yes" : "No", true)
                        .AddField("All Talk", server.HasAllTalk ? "Yes" : "No", true)
                        .WithImageUrl("https://teamwork.tf" + server.MapThumbnail)
                        .WithFooter("Type 'next' within 10 seconds for the next server")
                        .WithColor(new DiscordColor("#E7B53B"));
                    var message = await ctx.RespondAsync(embed: output.Build());

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
                    if (interactivity.Result == null) break;
                    await BotServices.RemoveMessage(interactivity.Result);
                    await BotServices.RemoveMessage(message);
                }
            }
        }

        #endregion COMMAND_SERVERS
    }
}