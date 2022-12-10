using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using FlawBOT.Services.Games;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Games
{
    [Group("tf2")]
    [Description("Commands related to Team Fortress 2.")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class TeamFortressModule : BaseCommandModule
    {
        #region COMMAND_SCHEMA

        [Command("item")]
        [Aliases("schema", "hat")]
        [Description("Retrieve an item from the latest TF2 item schema.")]
        public async Task Tf2Schema(CommandContext ctx,
            [Description("Item to find in the TF2 schema")] [RemainingText]
            string query = "The Scattergun")
        {
            await ctx.TriggerTypingAsync();
            var item = TeamFortressService.GetSchemaItem(query);
            if (item is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            var textInfo = new CultureInfo("en-US", false).TextInfo;
            var output = new DiscordEmbedBuilder()
                .WithTitle(item.ItemName)
                .WithDescription(item.ItemDescription ?? string.Empty)
                .AddField("Item Slot:", textInfo.ToTitleCase(item.ItemSlot) ?? "Unknown", true)
                .AddField("Giftable:", item.Capabilities.CanGiftWrap == true ? "Yes" : "No", true)
                .AddField("Nameable:", item.Capabilities.Nameable ? "Yes" : "No", true)
                .WithThumbnail(item.ImageUrlLarge ?? item.ImageUrl)
                .WithUrl(string.Format(Resources.URL_TF2Wiki, item.ItemName.Replace(' ', '_')))
                .WithFooter("ID: " + item.DefIndex)
                .WithColor(new DiscordColor("#E7B53B"));

            if (item.UsedByClasses != null)
            {
                var classes = new StringBuilder();
                foreach (var className in item.UsedByClasses)
                    classes.Append(className)
                        .Append(!className.Equals(item.UsedByClasses.Last()) ? ", " : string.Empty);
                output.AddField("Used by:", classes.ToString() ?? "Unknown");
            }
            else
            {
                output.AddField("Used by:", "All-Classes");
            }

            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_SCHEMA

        #region COMMAND_MAP

        [Command("map")]
        [Aliases("maps")]
        [Description("Retrieve map information from teamwork.tf")]
        public async Task Tf2Map(CommandContext ctx,
            [Description("Normalized map name, like pl_upward")]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            await ctx.TriggerTypingAsync();
            var results = await TeamFortressService
                .GetMapStatsAsync(Program.Settings.Tokens.TeamworkToken, query.ToLowerInvariant())
                .ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            _ = double.TryParse(results.AllTimeAvgPlayers, out var avgPlayers);
            var output = new DiscordEmbedBuilder()
                .WithTitle(results.MapName)
                .AddField("Highest Server Count", results.HighestServers.ToString() ?? "Unknown", true)
                .AddField("Highest Player Count", results.HighestPlayers.ToString() ?? "Unknown", true)
                .AddField("Avg. Players", Math.Round(avgPlayers, 2).ToString(CultureInfo.InvariantCulture) ?? "Unknown",
                    true)
                .WithFooter("Statistics retrieved from teamwork.tf - refreshed every 5 minutes")
                .WithImageUrl(results.Thumbnail)
                .WithUrl(string.Format(Resources.URL_TF2Wiki, results.MapName))
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
                if (results.ExtraInfo.Tf2MapsUrl != null)
                    links.Append($"[TF2Maps]({results.ExtraInfo.Tf2MapsUrl}) **|**");
                if (results.ExtraInfo.GameBananaUrl != null)
                    links.Append($"[GameBanana]({results.ExtraInfo.GameBananaUrl}");
                output.AddField("Links", links.ToString(), true);
            }

            if (results.GameModes.Count > 0)
            {
                var desc = TeamFortressService.GetGameModeInfoAsync(Program.Settings.Tokens.TeamworkToken,
                    results.GameModes.FirstOrDefault()).Result;
                output.WithDescription(desc.Title + " - " + desc.Description);
                output.WithColor(new DiscordColor($"#{desc.Color}"));
            }

            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_MAP

        #region COMMAND_NEWS

        [Command("news")]
        [Description("Retrieve the latest news article from teamwork.tf.")]
        public async Task Tf2News(CommandContext ctx,
            [Description("Page number from which to retrieve the news")]
            int query = 0)
        {
            await ctx.TriggerTypingAsync();
            var results = await TeamFortressService.GetNewsArticlesAsync(Program.Settings.Tokens.TeamworkToken, query)
                .ConfigureAwait(false);
            if (results is null || results.Count == 0)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            while (results.Count > 0)
            {
                var output = new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#E7B53B"))
                    .WithFooter(results.Count - 5 >= 5
                        ? "Type 'next' within 10 seconds for the next five posts."
                        : "These are all the latest posts at this time.");

                foreach (var result in results.Take(5))
                {
                    output.AddField(result.CreatedAt.Date.ToString(),
                        $"{result.Provider ?? result.Type}: [{result.Title}]({result.Link.AbsoluteUri})");
                    results.Remove(result);
                }

                var message = await ctx.RespondAsync("Latest news articles from teamwork.tf", output)
                    .ConfigureAwait(false);

                if (results.Count < 5) break;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                await BotServices.RemoveMessage(message).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_NEWS

        #region COMMAND_CREATORS

        [Command("creator")]
        [Aliases("creators", "youtuber")]
        [Description("Retrieve a community creator profile from teamwork.tf.")]
        public async Task Tf2Creators(CommandContext ctx,
            [Description("Name of the community creator to find.")] [RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            await ctx.TriggerTypingAsync();
            var steamId = SteamService.GetSteamUserId(query).Result;
            if (steamId is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            var results = await TeamFortressService
                .GetContentCreatorAsync(Program.Settings.Tokens.TeamworkToken, steamId.Data).ConfigureAwait(false);
            if (results.Count == 0)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            foreach (var creator in results)
            {
                var user = results.FirstOrDefault();
                var output = new DiscordEmbedBuilder()
                    .WithTitle(user?.Name)
                    .WithDescription("Main Class: " + user?.Main?.ToString()?.ToUpper())
                    .WithThumbnail(user?.ThumbnailUrl)
                    .WithUrl(user?.Link)
                    .WithColor(new DiscordColor("#E7B53B"))
                    .WithFooter(!creator.Equals(results.Last())
                        ? "Type 'next' within 10 seconds for the next creator"
                        : "Data retrieved from teamwork.tf");

                var links = new StringBuilder();
                if (creator.DiscordGroup != null)
                    links.Append($"[Discord]({Resources.URL_Discord}{creator.DiscordGroup}) **|** ");
                if (!string.IsNullOrWhiteSpace(creator.Steam))
                    links.Append($"[Steam]({Resources.URL_Steam_User}{creator.Steam}) **|** ");
                if (creator.SteamGroup != null)
                    links.Append($"[Steam Group]({Resources.URL_Steam_Group}{creator.SteamGroup}) **|** ");
                if (creator.Twitch != null)
                    links.Append($"[Twitch]({Resources.URL_Twitch}{creator.Twitch}) **|** ");
                if (!string.IsNullOrWhiteSpace(creator.Twitter))
                    links.Append($"[Twitter]({Resources.URL_Twitter}{creator.Twitter}) **|** ");
                if (!string.IsNullOrWhiteSpace(creator.Youtube))
                    links.Append($"[YouTube]({string.Format(Resources.URL_YouTube_Channel, creator.Youtube)})");
                output.AddField("Links", links.ToString(), true);
                var message = await ctx.RespondAsync(output.Build()).ConfigureAwait(false);

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
        [Description("Retrieve a list of servers with given game-mode.")]
        public async Task Tf2ServerByMode(CommandContext ctx,
            [Description("Name of the game-mode, like payload.")] [RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            await ctx.TriggerTypingAsync();
            var results = await TeamFortressService
                .GetServersByGameModeAsync(Program.Settings.Tokens.TeamworkToken, query.Trim().Replace(' ', '-'))
                .ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            foreach (var server in results.OrderBy(_ => new Random().Next()).ToList())
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(server.Name)
                    .WithDescription("steam://connect/" + server.Ip + ":" + server.Port)
                    .AddField("Provider", server.Provider != null ? server.Provider.Name : "Unknown", true)
                    .AddField("Player Count",
                        (server.Players.ToString() ?? "Unknown") + "/" + (server.MaxPlayers.ToString() ?? "Unknown"),
                        true)
                    .AddField("Password Lock", server.HasPassword ? "Yes" : "No", true)
                    .AddField("Random Crits", server.HasRandomCrits == true ? "Yes" : "No", true)
                    .AddField("Instant Respawn", server.HasNoRespawnTime ? "Yes" : "No", true)
                    .AddField("All Talk", server.HasAllTalk ? "Yes" : "No", true)
                    .AddField("Current Map", server.MapName ?? "Unknown", true)
                    .AddField("Next Map", server.MapNameNext ?? "Unknown", true)
                    .WithFooter("Type 'next' within 10 seconds for the next server.")
                    .WithColor(new DiscordColor("#E7B53B"));

                var thumbnailUrl = await TeamFortressService
                    .GetMapThumbnailAsync(Program.Settings.Tokens.TeamworkToken, server.MapName).ConfigureAwait(false);
                output.WithImageUrl(thumbnailUrl.Name);

                var message = await ctx.RespondAsync(output.Build()).ConfigureAwait(false);

                if (results.Count == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                if (!server.Equals(results.Last()))
                    await BotServices.RemoveMessage(message).ConfigureAwait(false);
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        [Command("find")]
        [Aliases("ip", "banner")]
        [Description("Retrieve a game server with given ip address")]
        public async Task Tf2ServerByIp(CommandContext ctx,
            [Description("Game server IP address, like 164.132.233.16")] [RemainingText]
            string ip)
        {
            await ctx.TriggerTypingAsync();
            if (string.IsNullOrWhiteSpace(ip) || !IPAddress.TryParse(ip, out var address))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_INVALID_IP, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            await ctx.RespondAsync(string.Format(Resources.URL_Steam_Connect, address)).ConfigureAwait(false);
            await ctx.RespondAsync(
                    TeamFortressService.GetServerInfo(Program.Settings.Tokens.TeamworkToken, address.ToString()))
                .ConfigureAwait(false);
        }

        [Command("list")]
        [Aliases("serverList", "server-list", "custom")]
        [Description("Retrieve a curated list of servers")]
        public async Task Tf2ServerList(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var results = await TeamFortressService.GetCustomServerListsAsync(Program.Settings.Tokens.TeamworkToken)
                .ConfigureAwait(false);
            results = results.OrderBy(_ => new Random().Next()).ToList();
            while (results.Count > 0)
            {
                var output = new DiscordEmbedBuilder()
                    .WithFooter("Type 'next' within 10 seconds for the next set of server lists.")
                    .WithColor(new DiscordColor("#E7B53B"));

                foreach (var list in results.Take(4))
                {
                    var desc = Regex.Replace(
                        list.DescriptionLong.Length <= 400
                            ? list.DescriptionLong
                            : list.DescriptionLong.Substring(0, 200) + "...", "<[^>]*>", "");
                    output.AddField($"Created By: {list.Creator.Name ?? "Unknown"} \t Subscribers: {list.Subscribed}",
                        $"[{list.Name}]({Resources.URL_TeamworkTF + list.Id}) - {desc}");

                    results.Remove(list);
                }

                var message = await ctx
                    .RespondAsync("Community-Curated Server Lists from teamwork.tf", output.Build())
                    .ConfigureAwait(false);

                if (results.Count == 4) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                await BotServices.RemoveMessage(message).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_SERVERS
    }
}