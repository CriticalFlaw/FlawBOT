using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
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
    public class TeamFortressModule : ApplicationCommandModule
    {
        #region COMMAND_SCHEMA

        [SlashCommand("item", "Retrieve an item from the latest TF2 item schema.")]
        public async Task Tf2Schema(InteractionContext ctx, [Option("query", "Item to find in the TF2 schema")] string query = "The Scattergun")
        {
            var item = TeamFortressService.GetSchemaItem(query);
            if (item is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
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
                    classes.Append(className).Append(!className.Equals(item.UsedByClasses.Last()) ? ", " : string.Empty);
                output.AddField("Used by:", classes.ToString() ?? "Unknown");
            }
            else
            {
                output.AddField("Used by:", "All-Classes");
            }

            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_SCHEMA

        #region COMMAND_MAP

        [SlashCommand("map", "Retrieve map information from teamwork.tf")]
        public async Task Tf2Map(InteractionContext ctx, [Option("query", "Normalized map name, like pl_upward")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await TeamFortressService.GetMapStatsAsync(Program.Settings.Tokens.TeamworkToken, query.ToLowerInvariant()).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            _ = double.TryParse(results.AllTimeAvgPlayers, out var avgPlayers);
            var output = new DiscordEmbedBuilder()
                .WithTitle(results.MapName)
                .AddField("Highest Server Count", results.HighestServers.ToString() ?? "Unknown", true)
                .AddField("Highest Player Count", results.HighestPlayers.ToString() ?? "Unknown", true)
                .AddField("Avg. Players", Math.Round(avgPlayers, 2).ToString(CultureInfo.InvariantCulture) ?? "Unknown", true)
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

            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_MAP

        #region COMMAND_NEWS

        [SlashCommand("news", "Retrieve the latest news article from teamwork.tf")]
        public async Task Tf2News(InteractionContext ctx, [Option("query", "Page number from which to retrieve the news")] double query = 0)
        {
            var results = await TeamFortressService.GetNewsArticlesAsync(Program.Settings.Tokens.TeamworkToken, (int)query).ConfigureAwait(false);
            if (results is null || results.Count == 0)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
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
                
                await ctx.CreateResponseAsync("Latest news articles from teamwork.tf", output).ConfigureAwait(false);

                if (results.Count < 5) break;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_NEWS

        #region COMMAND_CREATORS

        [SlashCommand("creator", "Retrieve a community creator profile from teamwork.tf")]
        public async Task Tf2Creators(InteractionContext ctx, [Option("query", "Name of the community creator to find.")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var steamId = SteamService.GetSteamUserId(query).Result;
            if (steamId is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            var results = await TeamFortressService.GetContentCreatorAsync(Program.Settings.Tokens.TeamworkToken, steamId.Data).ConfigureAwait(false);
            if (results.Count == 0)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
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

                await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
                if (results.Count == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_CREATORS

        #region COMMAND_SERVERS

        [SlashCommand("server", "Retrieve a list of servers with given game-mode")]
        public async Task Tf2ServerByMode(InteractionContext ctx, [Option("query", "Name of the game-mode, like payload.")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await TeamFortressService.GetServersByGameModeAsync(Program.Settings.Tokens.TeamworkToken, query.Trim().Replace(' ', '-')).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            foreach (var server in results.OrderBy(_ => new Random().Next()).ToList())
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
                    .WithFooter("Type 'next' within 10 seconds for the next server.")
                    .WithColor(new DiscordColor("#E7B53B"));

                var thumbnailUrl = await TeamFortressService.GetMapThumbnailAsync(Program.Settings.Tokens.TeamworkToken, server.MapName).ConfigureAwait(false);
                output.WithImageUrl(thumbnailUrl.Name);
                await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);

                if (results.Count == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        [SlashCommand("find", "Retrieve a game server with given ip address.")]
        public async Task Tf2ServerByIp(InteractionContext ctx, [Option("query", "Game server IP address, like 164.132.233.16")] string ip)
        {
            if (string.IsNullOrWhiteSpace(ip) || !IPAddress.TryParse(ip, out var address))
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_INVALID_IP, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            await ctx.CreateResponseAsync(string.Format(Resources.URL_Steam_Connect, address)).ConfigureAwait(false);
            await ctx.CreateResponseAsync(TeamFortressService.GetServerInfo(Program.Settings.Tokens.TeamworkToken, address.ToString())).ConfigureAwait(false);
        }

        [SlashCommand("list", "Retrieve a curated list of servers.")]
        public async Task Tf2ServerList(InteractionContext ctx)
        {
            var results = await TeamFortressService.GetCustomServerListsAsync(Program.Settings.Tokens.TeamworkToken).ConfigureAwait(false);
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
                    output.AddField($"Created By: {list.Creator.Name ?? "Unknown"} \t Subscribers: {list.Subscribed}", $"[{list.Name}]({Resources.URL_TeamworkTF + list.Id}) - {desc}");

                    results.Remove(list);
                }
                await ctx.CreateResponseAsync("Community-Curated Server Lists from teamwork.tf", output.Build()).ConfigureAwait(false);

                if (results.Count == 4) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_SERVERS
    }
}