using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using TeamworkTF.Sharp;

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
        public async Task Tf2Item(CommandContext ctx,
            [Description("Item to find in the TF2 schema")] [RemainingText] string query = "The Scattergun")
        {
            var item = TeamFortressService.GetSchemaItem(query);
            if (item is null)
            {
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing)
                    .ConfigureAwait(false);
            }
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
                output.AddField("Giftable:", item.Capabilities.CanGiftWrap == true ? "Yes" : "No", true);
                output.AddField("Nameable:", item.Capabilities.Nameable ? "Yes" : "No", true);
                output.AddField("Restriction:", item.HolidayRestriction ?? "None", true);
                await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_SCHEMA

        #region COMMAND_MAP

        [Command("map")]
        [Aliases("maps")]
        [Description("Retrieve map information from teamwork.tf")]
        public async Task Tf2Map(CommandContext ctx,
            [Description("Normalized map name, like pl_upward")] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = await TeamFortressService.GetMapStatsAsync(query.ToLowerInvariant()).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing)
                    .ConfigureAwait(false);
            }
            else
            {
                double.TryParse(results.AllTimeAvgPlayers, out var avgPlayers);
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.MapName)
                    .AddField("Highest Server Count", results.HighestServers.ToString() ?? "Unknown", true)
                    .AddField("Highest Player Count", results.HighestPlayers.ToString() ?? "Unknown", true)
                    .AddField("Avg. Players", Math.Round(avgPlayers, 2).ToString(CultureInfo.InvariantCulture) ?? "Unknown", true)
                    .WithFooter("Statistics retrieved from teamwork.tf - refreshed every 5 minutes")
                    .WithImageUrl(results.Thumbnail)
                    .WithUrl("https://wiki.teamfortress.com/wiki/" + results.MapName)
                    .WithColor(new DiscordColor("#E7B53B"));

                if (results.RelatedMaps.Count > 0)
                {
                    var maps = new StringBuilder();
                    foreach (var map in results.RelatedMaps.Take(4))
                        maps.Append(map + "\n");
                    output.AddField("Related Map(s)", maps.ToString(), true);
                }

                if (results.ExtraInfo != null)
                {
                    var links = new StringBuilder();
                    if (results.ExtraInfo.SteamWorkshopUrl != null)
                        links.Append($"[Steam Workshop]({results.ExtraInfo.SteamWorkshopUrl}) **|**");
                    if (results.ExtraInfo.TF2MapsUrl != null)
                        links.Append($"[TF2Maps]({results.ExtraInfo.TF2MapsUrl}) **|**");
                    if (results.ExtraInfo.GameBananaUrl != null)
                        links.Append($"[GameBanana]({results.ExtraInfo.GameBananaUrl}) **|**");
                    output.AddField("Links", links.ToString(), true);
                }

                if (results.GameModes.Count > 0)
                {
                    var desc = TeamFortressService.GetGameModeAsync(results.GameModes.FirstOrDefault()).Result;
                    output.WithDescription(desc.Title + " - " + desc.Description);
                    output.WithColor(new DiscordColor($"#{desc.Color}"));
                }

                await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_MAP

        #region COMMAND_NEWS

        [Command("news")]
        [Description("Retrieve the latest news article from teamwork.tf")]
        public async Task Tf2News(CommandContext ctx,
            [Description("Page number from which to retrieve the news")] int query = 0)
        {
            var results = await TeamFortressService.GetNewsOverviewAsync(query).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing)
                    .ConfigureAwait(false);
            }
            else
            {
                var output = new DiscordEmbedBuilder().WithColor(new DiscordColor("#E7B53B"));
                foreach (var result in results.Take(5))
                    output.AddField(result.CreatedAt.Date.ToString(), $"{result.Type}: [{result.Title}]({result.Link.AbsoluteUri})");
                await ctx.RespondAsync("Latest news articles from teamwork.tf", embed: output.Build())
                    .ConfigureAwait(false);
            }
        }

        #endregion COMMAND_NEWS

        #region COMMAND_CREATORS

        [Command("creator")]
        [Aliases("creators", "youtuber")]
        [Description("Retrieve a community creator profile from teamwork.tf")]
        public async Task Tf2Creators(CommandContext ctx,
            [Description("Name of the community creator to find")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var steamId = SteamService.GetSteamUserId(query).Result.Data;
            var results = await TeamFortressService.GetCreatorByIdAsync(steamId).ConfigureAwait(false);
            if (results is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing)
                    .ConfigureAwait(false);
            else
                foreach (var creator in results)
                {
                    var user = results.FirstOrDefault();
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(user?.Name)
                        .WithDescription("Main Class: " + user?.Main.ToString()?.ToUpper())
                        .WithThumbnail(user?.ThumbnailUrl)
                        .WithUrl(user?.Link)
                        .WithColor(new DiscordColor("#E7B53B"))
                        .WithFooter(!creator.Equals(results.Last())
                            ? "Type 'next' within 10 seconds for the next creator"
                            : "Data retrieved from teamwork.tf");

                    var links = new StringBuilder();
                    if (creator.DiscordGroup != null)
                        links.Append($"[Discord]({Resources.URL_DISCORD}{creator.DiscordGroup}) **|** ");
                    if (!string.IsNullOrWhiteSpace(creator.Steam))
                        links.Append($"[Steam]({Resources.URL_STEAM_USER}{creator.Steam}) **|** ");
                    if (creator.SteamGroup != null)
                        links.Append($"[Steam Group]({Resources.URL_STEAM_GROUP}{creator.SteamGroup}) **|** ");
                    if (creator.Twitch != null)
                        links.Append($"[Twitch]({Resources.URL_TWITCH_CHANNEL}{creator.Twitch}) **|** ");
                    if (!string.IsNullOrWhiteSpace(creator.Twitter))
                        links.Append($"[Twitter]({Resources.URL_YOUTUBE_CHANNEL}{creator.Twitter}) **|** ");
                    if (!string.IsNullOrWhiteSpace(creator.Youtube))
                        links.Append($"[YouTube]({Resources.URL_YOUTUBE_CHANNEL}{creator.Youtube})");
                    output.AddField("Links", links.ToString(), true);
                    var message = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                    if (results.Count == 1) continue;
                    var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                    if (interactivity.Result is null) break;
                    await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                    if (!creator.Equals(results.Last()))
                        await BotServices.RemoveMessage(message).ConfigureAwait(false);
                }
        }

        #endregion COMMAND_CREATORS

        #region COMMAND_SERVERS

        [Command("server")]
        [Aliases("servers")]
        [Description("Retrieve a list of servers with given game-mode")]
        public async Task Tf2ServerByMode(CommandContext ctx,
            [Description("Name of the game-mode, like payload")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = await TeamFortressService.GetGameModeServerAsync(query.Trim().Replace(' ', '-'))
                .ConfigureAwait(false);
            if (results is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing)
                    .ConfigureAwait(false);
            else
                await OutputServerInfo(ctx, results.OrderBy(_ => new Random().Next()).ToList()).ConfigureAwait(false);
        }

        [Command("ip")]
        [Aliases("find")]
        [Description("Retrieve a game server with given ip address")]
        public async Task Tf2ServerByIp(CommandContext ctx,
            [Description("Game server IP address, like 164.132.233.16")] string ip,
            [Description("Game server port, like 27022")] int port = 0)
        {
            if (!BotServices.CheckUserInput(ip)) return;
            var regex = new Regex(@"\s*(?'ip'\S+)\s*", RegexOptions.Compiled).Match(ip);
            if (!regex.Success) return;
            var results = await TeamFortressService.GetGameServerInfoAsync(ip, port).ConfigureAwait(false);
            if (results is null || results.Count <= 0)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing)
                    .ConfigureAwait(false);
            else
                await OutputServerInfo(ctx, results.OrderBy(_ => new Random().Next()).ToList()).ConfigureAwait(false);
        }

        public async Task OutputServerInfo(CommandContext ctx, List<Server> results)
        {
            foreach (var server in results)
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(server.Name)
                    .WithDescription("steam://connect/" + server.Ip + ":" + server.Port)
                    .AddField("Provider", server.Provider != null ? server.Provider.Name : "Unknown", true)
                    .AddField("Player Count", (server.Players.ToString() ?? "Unknown") + "/" + (server.MaxPlayers.ToString() ?? "Unknown"), true)
                    .AddField("Password Lock", server.HasPassword ? "Yes" : "No", true)
                    .AddField("Random Crits", server.HasRandomCrits == true ? "Yes" : "No", true)
                    .AddField("Instant Respawn", server.HasNoRespawnTime ? "Yes" : "No", true)
                    .AddField("All Talk", server.HasAllTalk ? "Yes" : "No", true)
                    .AddField("Current Map", server.MapName ?? "Unknown", true)
                    .AddField("Next Map", server.MapNameNext ?? "Unknown", true)
                    .WithFooter("Type 'next' within 10 seconds for the next server")
                    .WithColor(new DiscordColor("#E7B53B"));

                var thumbnailUrl = await TeamFortressService.GetMapThumbnailAsync(server.MapName).ConfigureAwait(false);
                output.WithImageUrl(thumbnailUrl.Name);

                var message = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                if (results.Count == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                if (!server.Equals(results.Last()))
                    await BotServices.RemoveMessage(message).ConfigureAwait(false);
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        [Command("server-list")]
        [Aliases("serverList")]
        [Description("Retrieve a curated list of servers")]
        public async Task Tf2ServerList(CommandContext ctx)
        {
            var results = await TeamFortressService.GetCustomServerListsAsync().ConfigureAwait(false);
            results = results.OrderBy(_ => new Random().Next()).ToList();
            foreach (var server in results)
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(server.Name)
                    .WithDescription(
                        Regex.Replace(
                            server.DescriptionLarge.Length <= 500
                                ? server.DescriptionLarge
                                : server.DescriptionLarge.Substring(0, 250) + "...", "<[^>]*>", "") ?? "Unknown")
                    .AddField("Created by", server.Creator.Name ?? "Unknown", true)
                    .AddField("Subscribers", server.Subscribed.ToString() ?? "Unknown", true)
                    .WithUrl(Resources.URL_TEAMWORK_SERVERS + server.Id)
                    .WithFooter("Type 'next' within 10 seconds for the next server list")
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

        #endregion COMMAND_SERVERS
    }
}