﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
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
        [Aliases("schema")]
        [Description("Retrieve an item from the latest TF2 item schema")]
        public async Task TF2Item(CommandContext ctx,
            [Description("Item to find in the TF2 schema")] [RemainingText] string query = "The Scattergun")
        {
            var item = TeamFortressService.GetSchemaItemAsync(query);
            if (item is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                var textInfo = new CultureInfo("en-US", false).TextInfo;
                var output = new DiscordEmbedBuilder()
                    .WithTitle(item.ItemName)
                    .WithDescription(item.Description ?? string.Empty)
                    .WithImageUrl(item.ImageURL_Large ?? item.ImageURL)
                    .WithUrl("https://wiki.teamfortress.com/wiki/" + item.ItemName.Replace(' ', '_'))
                    .WithFooter("ID: " + item.DefIndex)
                    .WithColor(new DiscordColor("#E7B53B"));

                var classes = new StringBuilder();
                foreach (var className in item.UsedByClasses)
                    classes.Append(className + (!className.Equals(item.UsedByClasses.Last()) ?  ", " : string.Empty));
                output.AddField("Used by:", classes.ToString() ?? "Unknown", true);
                output.AddField("Item Slot:", textInfo.ToTitleCase(item.ItemSlot) ?? "Unknown", true);
                output.AddField("Item Type:", item.ItemType ?? "Unknown", true);
                output.AddField("Giftable:", (item.Capabilities.Giftable) ? "Yes" : "No", true);
                output.AddField("Nameable:", (item.Capabilities.Renamable) ? "Yes" : "No", true);
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
            if (results.MapName is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                double.TryParse(results.AvgPlayers, out var avg_players);
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.MapName)
                    .AddField("Official", results.OfficialMap ? "Yes" : "No", true)
                    .AddField("Game Mode", results.GameModes.First() ?? "Unknown", true)
                    //.AddField("First Seen", results.ServerTypes.ToString() ?? "Unknown", true)
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
            if (results is null || results.Count == 0)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                while (results.Count > 0)
                {
                    var output = new DiscordEmbedBuilder()
                        .WithFooter((results.Count > 5) ? "Type 'next' in the next 10 seconds for more news articles." : "This all the Team Fortress 2 community news at the moment.")
                        .WithColor(new DiscordColor("#E7B53B"));

                    foreach (var result in results.Take(5))
                        output.AddField(result.Title, result.Link);
                    var message = await ctx.RespondAsync("Latest news articles from teamwork.tf", embed: output.Build()).ConfigureAwait(false);

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && string.Equals(m.Content, "next", StringComparison.InvariantCultureIgnoreCase), TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                    if (interactivity.Result is null) break;
                    await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                    await BotServices.RemoveMessage(message).ConfigureAwait(false);
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
            var results = await TeamFortressService.GetServersAsync(query.Trim().Replace(' ', '-')).ConfigureAwait(false);
            if (results.Count <= 0)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                var random = new Random();
                results = results.OrderBy(_ => random.Next()).ToList();
                foreach (var server in results.Where(n => n.GameModes.Contains(query)))
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(server.Name)
                        .WithDescription("steam://connect/" + server.IP + ":" + server.Port)
                        .AddField("Provider", server.Provider ?? "Unknown", true)
                        .AddField("Player Count", (server.PlayerCount.ToString() ?? "Unknown") + "/" + (server.PlayerMax.ToString() ?? "Unknown"), true)
                        .AddField("Password Lock", (server.HasPassword == true) ? "Yes" : "No", true)
                        .AddField("Random Crits", server.HasRandomCrits == true ? "Yes" : "No", true)
                        .AddField("Instant Respawn", server.HasNoSpawnTimer ? "Yes" : "No", true)
                        .AddField("All Talk", server.HasAllTalk ? "Yes" : "No", true)
                        .AddField("Current Map", server.MapName ?? "Unknown", true)
                        .AddField("Next Map", server.NextMap ?? "Unknown", true)
                        .WithImageUrl("https://teamwork.tf" + server.MapThumbnail)
                        .WithFooter("Type 'next' within 10 seconds for the next server")
                        .WithColor(new DiscordColor("#E7B53B"));
                    var message = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && string.Equals(m.Content, "next", StringComparison.InvariantCultureIgnoreCase), TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                    if (interactivity.Result is null) break;
                    await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                    await BotServices.RemoveMessage(message).ConfigureAwait(false);
                }
            }
        }

        #endregion COMMAND_SERVERS
    }
}