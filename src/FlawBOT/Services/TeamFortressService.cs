using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Models.GameEconomy;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamworkTF.Sharp;

namespace FlawBOT.Services
{
    public class TeamFortressService : HttpHandler
    {
        private static SteamWebInterfaceFactory _steamInterface;
        private static List<SchemaItem> ItemSchemaList { get; } = new();

        public static async Task<DiscordEmbed> GetNewsArticlesAsync(string token, int page = 0, string provider = "")
        {
            List<News> results;
            if (page > 0)
                results = await new TeamworkAPI(token).GetNewsByPageAsync(page).ConfigureAwait(false);
            else if (provider != string.Empty)
                results = await new TeamworkAPI(token).GetNewsByProviderAsync(provider).ConfigureAwait(false);
            else
                results = await new TeamworkAPI(token).GetNewsOverviewAsync().ConfigureAwait(false);

            var output = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#E7B53B"))
                .WithFooter(results.Count - 5 >= 5
                    ? "Type 'next' within 10 seconds for the next five posts."
                    : "These are all the latest posts at this time.");

            foreach (var result in results.Where(x => x.Type != "tf2-notification").Take(5))
                output.AddField(result.CreatedAt.Date.ToString(), $"{result.Provider ?? result.Type}: [{result.Title}]({result.Link.AbsoluteUri})");
            return output.Build();
        }

        public static async Task<DiscordEmbed> GetContentCreatorAsync(string token, ulong query)
        {
            var response = await new TeamworkAPI(token).GetCommunityUserAsync(query.ToString()).ConfigureAwait(false);
            var results = response.FirstOrDefault();
            var output = new DiscordEmbedBuilder()
                .WithTitle(results?.Name)
                .WithDescription("Main Class: " + results?.Main?.ToString()?.ToUpper())
                .WithThumbnail(results?.ThumbnailUrl)
                .WithUrl(results?.Link)
                .WithColor(new DiscordColor("#E7B53B"))
                .WithFooter("Data retrieved from teamwork.tf");

            var links = new StringBuilder();
            if (results.DiscordGroup != null)
                links.Append($"[Discord]({Resources.URL_Discord}{results.DiscordGroup}) **|** ");
            if (!string.IsNullOrWhiteSpace(results.Steam))
                links.Append($"[Steam]({Resources.URL_Steam_User}{results.Steam}) **|** ");
            if (results.SteamGroup != null)
                links.Append($"[Steam Group]({Resources.URL_Steam_Group}{results.SteamGroup}) **|** ");
            if (results.Twitch != null)
                links.Append($"[Twitch]({Resources.URL_Twitch}{results.Twitch}) **|** ");
            if (!string.IsNullOrWhiteSpace(results.Twitter))
                links.Append($"[Twitter]({Resources.URL_Twitter}{results.Twitter}) **|** ");
            if (!string.IsNullOrWhiteSpace(results.Youtube))
                links.Append($"[YouTube]({string.Format(Resources.URL_YouTube_Channel, results.Youtube)})");
            output.AddField("Links", links.ToString(), true);
            return output.Build();
        }

        public static DiscordEmbed GetSchemaItem(string query)
        {
            if (ItemSchemaList.Count == 0) return null; // TODO: Redownload item schema if missing.
            var results = ItemSchemaList.Find(n => n.ItemName.Contains(query, StringComparison.InvariantCultureIgnoreCase));
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            var output = new DiscordEmbedBuilder()
                .WithTitle(results.ItemName)
                .WithDescription(results.ItemDescription ?? string.Empty)
                .AddField("Item Slot:", textInfo.ToTitleCase(results.ItemSlot) ?? "Unknown", true)
                .AddField("Giftable:", results.Capabilities.CanGiftWrap == true ? "Yes" : "No", true)
                .AddField("Nameable:", results.Capabilities.Nameable ? "Yes" : "No", true)
                .WithThumbnail(results.ImageUrlLarge ?? results.ImageUrl)
                .WithUrl(string.Format(Resources.URL_TF2Wiki, results.ItemName.Replace(' ', '_')))
                .WithFooter("ID: " + results.DefIndex)
                .WithColor(new DiscordColor("#E7B53B"));

            if (results.UsedByClasses != null)
            {
                var classes = new StringBuilder();
                foreach (var className in results.UsedByClasses)
                    classes.Append(className).Append(!className.Equals(results.UsedByClasses.Last()) ? ", " : string.Empty);
                output.AddField("Used by:", classes.ToString() ?? "Unknown");
            }
            else
                output.AddField("Used by:", "All-Classes");
            return output.Build();
        }

        public static async Task<bool> UpdateTF2SchemaAsync(string token)
        {
            try
            {
                _steamInterface = new SteamWebInterfaceFactory(token);
                var steam = _steamInterface.CreateSteamWebInterface<EconItems>(AppId.TeamFortress2, new HttpClient());
                var games = await steam.GetSchemaItemsForTF2Async().ConfigureAwait(false);
                ItemSchemaList.Clear();
                foreach (var game in games.Data.Result.Items)
                    if (!string.IsNullOrWhiteSpace(game.Name))
                        ItemSchemaList.Add(game);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.ERR_TF2_LIST, ex.Message);
                return false;
            }
        }

        public static async Task<DiscordEmbed> GetServersByGameModeAsync(string token, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;
            var response = await new TeamworkAPI(token).GetServerListByGameModeAsync(query).ConfigureAwait(false);
            var results = response[random.Next(response.Count)];

            var output = new DiscordEmbedBuilder()
                .WithTitle(results.Name)
                .WithDescription("steam://connect/" + results.Ip + ":" + results.Port)
                .AddField("Provider", results.Provider != null ? results.Provider.Name : "Unknown", true)
                .AddField("Player Count", (results.Players.ToString() ?? "Unknown") + "/" + (results.MaxPlayers.ToString() ?? "Unknown"), true)
                .AddField("Password Lock", results.HasPassword ? "Yes" : "No", true)
                .AddField("Random Crits", results.HasRandomCrits == true ? "Yes" : "No", true)
                .AddField("Instant Respawn", results.HasNoRespawnTime ? "Yes" : "No", true)
                .AddField("All Talk", results.HasAllTalk ? "Yes" : "No", true)
                .AddField("Current Map", results.MapName ?? "Unknown", true)
                .AddField("Next Map", results.MapNameNext ?? "Unknown", true)
                .WithColor(new DiscordColor("#E7B53B"));

            var thumbnailUrl = await GetMapThumbnailAsync(Program.Settings.Tokens.TeamworkToken, results.MapName).ConfigureAwait(false);
            output.WithImageUrl(thumbnailUrl.Name);
            return output.Build();
        }

        public static async Task<DiscordEmbed> GetCustomServerListsAsync(string token)
        {
            var results = await new TeamworkAPI(token).GetServerListsAsync().ConfigureAwait(false);
            results = results.OrderBy(_ => new Random().Next()).Take(5).ToList();

            var output = new DiscordEmbedBuilder()
                .WithFooter("Type 'next' within 10 seconds for the next set of server lists.")
                .WithColor(new DiscordColor("#E7B53B"));

            foreach (var list in results.Take(4))
            {
                var desc = Regex.Replace(list.DescriptionLong.Length <= 400
                    ? list.DescriptionLong
                    : list.DescriptionLong.Substring(0, 200) + "...", "<[^>]*>", "");
                output.AddField($"Created By: {list.Creator.Name ?? "Unknown"} \t Subscribers: {list.Subscribed}", $"[{list.Name}]({Resources.URL_TeamworkTF + list.Id}) - {desc}");
            }
            return output.Build();
        }

        public static async Task<DiscordEmbed> GetMapStatsAsync(string token, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;
            var map = new TeamworkAPI(token).GetMapsBySearchAsync(query).Result.FirstOrDefault()?.Name;
            var results = await new TeamworkAPI(token).GetMapStatsAsync(map).ConfigureAwait(false);

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
                foreach (var m in results.RelatedMaps.Take(4))
                    maps.Append(m + "\n");
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
                var mode = new TeamworkAPI(token).GetGameModeAsync(results.GameModes.FirstOrDefault()).Result;
                output.WithDescription(mode.Title + " - " + mode.Description);
                output.WithColor(new DiscordColor($"#{mode.Color}"));
            }
            return output.Build();
        }

        public static async Task<MapThumbnail> GetMapThumbnailAsync(string token, string query)
        {
            return await new TeamworkAPI(token).GetMapThumbnailAsync(query).ConfigureAwait(false);
        }

        public static string GetServerBanner(string token, string address)
        {
            return new TeamworkAPI(token).GetServerBanner(address);
        }
    }
}