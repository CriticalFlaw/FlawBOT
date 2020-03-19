using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Group("tf2")]
    [Description("Commands related to Team Fortress 2")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class TeamFortressModule : BaseCommandModule
    {
        #region COMMAND_SCHEMA

        [Command("item")]
        [Aliases("schema", "hat")]
        [Description("Retrieve an item from the latest TF2 item schema")]
        public async Task TF2Item(CommandContext ctx,
            [Description("Item to find in the TF2 schema")] [RemainingText] string query = "The Scattergun")
        {
            var item = TeamFortressService.GetSchemaItem(query);
            if (item is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                var textInfo = new CultureInfo("en-US", false).TextInfo;
                var output = new DiscordEmbedBuilder()
                    .WithTitle(item.ItemName)
                    .WithDescription(item.ItemDescription ?? string.Empty)
                    .WithImageUrl(item.ImageUrlLarge ?? item.ImageUrl)
                    .WithUrl("https://wiki.teamfortress.com/wiki/" + item.ItemName.Replace(' ', '_'))
                    .WithFooter("ID: " + item.DefIndex)
                    .WithColor(new DiscordColor("#E7B53B"));

                var classes = new StringBuilder();
                foreach (var className in item.UsedByClasses)
                    classes.Append(className).Append(!className.Equals(item.UsedByClasses.Last()) ? ", " : string.Empty);
                output.AddField("Used by:", classes.ToString() ?? "Unknown", true);
                output.AddField("Item Slot:", textInfo.ToTitleCase(item.ItemSlot) ?? "Unknown", true);
                output.AddField("Item Type:", item.ItemTypeName ?? "Unknown", true);
                output.AddField("Giftable:", (item.Capabilities.CanGiftWrap == true) ? "Yes" : "No", true);
                output.AddField("Nameable:", (item.Capabilities.Nameable) ? "Yes" : "No", true);
                output.AddField("Restriction:", item.HolidayRestriction ?? "None", true);
                await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
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
            var results = await TeamFortressService.GetMapStatsAsync(query.ToLowerInvariant()).ConfigureAwait(false);
            if (results is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                double.TryParse(results.AvgPlayers, out var avg_players);
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.Name)
                    .AddField("Official", results.OfficialMap ? "Yes" : "No", true)
                    .AddField("Game Mode", results.GameModes[0] ?? "Unknown", true)
                    .AddField("Highest Server Count", results.HighestServerCount.ToString() ?? "Unknown", true)
                    .AddField("Highest Player Count", results.HighestPlayerCount.ToString() ?? "Unknown", true)
                    .AddField("Avg. Players", Math.Round(avg_players, 2).ToString() ?? "Unknown", true)
                    .WithFooter("Statistics retrieved from teamwork.tf - refreshed every 5 minutes")
                    .WithImageUrl(results.Thumbnail)
                    .WithUrl("https://wiki.teamfortress.com/wiki/" + results.Name)
                    .WithColor(new DiscordColor("#E7B53B"));

                if (results.RelatedMaps.Count > 0)
                {
                    var maps = new StringBuilder();
                    foreach (var map in results.RelatedMaps.Take(4))
                        maps.Append(map + "\n");
                    output.AddField("Related Map(s)", maps.ToString(), true);
                }
                await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_MAP

        #region COMMAND_NEWS

        [Command("news")]
        [Description("Retrieve the latest news article from teamwork.tf")]
        public async Task TF2News(CommandContext ctx)
        {
            var results = await TeamFortressService.GetNewsOverviewAsync().ConfigureAwait(false);
            if (results is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                var output = new DiscordEmbedBuilder().WithColor(new DiscordColor("#E7B53B"));
                foreach (var result in results.Take(5))
                    output.AddField(result.Title, result.Link);
                await ctx.RespondAsync("Latest news articles from teamwork.tf", embed: output.Build()).ConfigureAwait(false);
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
            var results = await TeamFortressService.GetServersAsync(query.Trim().Replace(' ', '-')).ConfigureAwait(false);
            if (results is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                results = results.OrderBy(_ => new Random().Next()).ToList();
                foreach (var server in results.Where(n => n.GameModes.Contains(query)))
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(server.Name)
                        .WithDescription("steam://connect/" + server.IP + ":" + server.Port)
                        .AddField("Provider", server.Provider ?? "Unknown", true)
                        .AddField("Player Count", (server.PlayerCount.ToString() ?? "Unknown") + "/" + (server.PlayerCountMax.ToString() ?? "Unknown"), true)
                        .AddField("Password Lock", (server.HasPassword == true) ? "Yes" : "No", true)
                        .AddField("Random Crits", server.HasRandomCrits == true ? "Yes" : "No", true)
                        .AddField("Instant Respawn", server.HasNoSpawnTimer ? "Yes" : "No", true)
                        .AddField("All Talk", server.HasAllTalk ? "Yes" : "No", true)
                        .AddField("Current Map", server.MapName ?? "Unknown", true)
                        .AddField("Next Map", server.NextMap ?? "Unknown", true)
                        .WithImageUrl("https://teamwork.tf" + server.ThumbnailPath)
                        .WithFooter("Type 'next' within 10 seconds for the next server")
                        .WithColor(new DiscordColor("#E7B53B"));
                    var message = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                    if (results.Count == 1) continue;
                    var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                    if (interactivity.Result is null) break;
                    if (!server.Equals(results.Last()))
                        await BotServices.RemoveMessage(message).ConfigureAwait(false);
                    await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                }
            }
        }

        #endregion COMMAND_SERVERS
    }
}