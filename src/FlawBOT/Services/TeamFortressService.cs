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
using System.Text;
using System.Threading.Tasks;
using TeamworkTF.Sharp;

namespace FlawBOT.Services
{
    public class TeamFortressService : HttpHandler
    {
        private static IReadOnlyList<SchemaItem> ItemSchemaList { get; set; } = [];
        private static readonly TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;
        private static readonly DiscordColor Tf2Color = new("#E7B53B");

        public static async Task<DiscordEmbed> GetNewsArticlesAsync(string token, int page = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token)) return null;
                var api = new TeamworkAPI(token);
                var results = page > 0
                    ? await api.GetNewsByPageAsync(page).ConfigureAwait(false)
                    : await api.GetNewsOverviewAsync().ConfigureAwait(false);
                var posts = results?.Where(x => x.Type != "tf2-notification" && x.Link is not null).Take(5).ToList();
                if (posts is null || posts.Count == 0) return null;

                var output = new DiscordEmbedBuilder()
                    .WithColor(Tf2Color)
                    .WithFooter("Use the page option to see older posts.");
                foreach (var result in posts)
                    output.AddSafeField(result.CreatedAt?.Date.ToString(), $"{result.Provider ?? result.Type}: [{result.Title}]({result.Link.AbsoluteUri})");
                return output.Build();
            }
            catch
            {
                return null;
            }
        }

        public static async Task<DiscordEmbed> GetContentCreatorAsync(string token, ulong query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token)) return null;
                var response = await new TeamworkAPI(token).GetCommunityUserAsync(query.ToString()).ConfigureAwait(false);
                var results = response?.FirstOrDefault();
                if (results is null) return null;

                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.Name)
                    .WithDescription("Main Class: " + (results.Main?.ToString()?.ToUpperInvariant() ?? EmbedExtensions.Unknown))
                    .WithColor(Tf2Color)
                    .WithFooter("Data retrieved from teamwork.tf");
                if (results.ThumbnailUrl is not null) output.WithThumbnail(results.ThumbnailUrl);
                if (results.Link is not null) output.WithUrl(results.Link);

                var links = new List<string>();
                if (results.DiscordGroup != null) links.Add($"[Discord]({Resources.URL_Discord}{results.DiscordGroup})");
                if (!string.IsNullOrWhiteSpace(results.Steam)) links.Add($"[Steam]({Resources.URL_Steam_User}{results.Steam})");
                if (results.SteamGroup != null) links.Add($"[Steam Group]({Resources.URL_Steam_Group}{results.SteamGroup})");
                if (results.Twitch != null) links.Add($"[Twitch]({Resources.URL_Twitch}{results.Twitch})");
                if (!string.IsNullOrWhiteSpace(results.Twitter)) links.Add($"[Twitter]({Resources.URL_Twitter}{results.Twitter})");
                if (!string.IsNullOrWhiteSpace(results.Youtube)) links.Add($"[YouTube]({string.Format(Resources.URL_YouTube_Channel, results.Youtube)})");
                output.AddSafeField("Links", links.JoinOrDefault(" **|** ", "None"), true);
                return output.Build();
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed GetSchemaItem(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || ItemSchemaList.Count == 0) return null;
            query = query.Trim();
            // Prefer an exact name match over the first partial match.
            var results = ItemSchemaList.FirstOrDefault(n => string.Equals(n.ItemName, query, StringComparison.InvariantCultureIgnoreCase))
                          ?? ItemSchemaList.FirstOrDefault(n => n.ItemName?.Contains(query, StringComparison.InvariantCultureIgnoreCase) == true);
            return results is null ? null : BuildSchemaItemEmbed(results);
        }

        public static DiscordEmbed BuildSchemaItemEmbed(SchemaItem results)
        {
            var output = new DiscordEmbedBuilder()
                .WithTitle(results.ItemName)
                .WithDescription(results.ItemDescription ?? string.Empty)
                .AddSafeField("Item Slot:", string.IsNullOrWhiteSpace(results.ItemSlot) ? null : TextInfo.ToTitleCase(results.ItemSlot), true)
                .AddSafeField("Giftable:", results.Capabilities?.CanGiftWrap == true ? ":heavy_check_mark:" : ":x:", true)
                .AddSafeField("Nameable:", results.Capabilities?.Nameable == true ? ":heavy_check_mark:" : ":x:", true)
                .AddSafeField("Used by:", results.UsedByClasses is { Count: > 0 } ? string.Join(", ", results.UsedByClasses) : "All-Classes")
                .WithUrl(string.Format(Resources.URL_TF2Wiki, Uri.EscapeDataString(results.ItemName.Replace(' ', '_'))))
                .WithFooter("ID: " + results.DefIndex)
                .WithColor(Tf2Color);
            var image = results.ImageUrlLarge ?? results.ImageUrl;
            if (!string.IsNullOrWhiteSpace(image)) output.WithThumbnail(image);
            return output.Build();
        }

        public static async Task<bool> UpdateTF2SchemaAsync(string token)
        {
            try
            {
                var steam = new SteamWebInterfaceFactory(token).CreateSteamWebInterface<EconItems>(AppId.TeamFortress2, SteamService.SteamHttp);
                var items = new List<SchemaItem>();
                uint? next = null;
                // The schema is paginated; keep requesting until there is no next page.
                do
                {
                    var page = await steam.GetSchemaItemsForTF2Async(start: next).ConfigureAwait(false);
                    var result = page?.Data?.Result;
                    if (result?.Items is null) break;
                    items.AddRange(result.Items.Where(x => !string.IsNullOrWhiteSpace(x.ItemName)));
                    next = result.Next;
                } while (next is not null);

                if (items.Count == 0) return false;
                ItemSchemaList = items;
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
            try
            {
                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(query)) return null;
                var api = new TeamworkAPI(token);
                var response = await api.GetServerListByGameModeAsync(query).ConfigureAwait(false);
                if (response is null || response.Count == 0) return null;

                var results = response[Random.Next(response.Count)];
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.Name)
                    .WithDescription($"steam://connect/{results.Ip}:{results.Port}")
                    .AddSafeField("Provider", results.Provider?.Name, true)
                    .AddSafeField("Player Count", $"{results.Players}/{results.MaxPlayers}", true)
                    .AddSafeField("Password Lock", results.HasPassword ? ":heavy_check_mark:" : ":x:", true)
                    .AddSafeField("Random Crits", results.HasRandomCrits == true ? ":heavy_check_mark:" : ":x:", true)
                    .AddSafeField("Instant Respawn", results.HasNoRespawnTime ? ":heavy_check_mark:" : ":x:", true)
                    .AddSafeField("All Talk", results.HasAllTalk ? ":heavy_check_mark:" : ":x:", true)
                    .AddSafeField("Current Map", results.MapName, true)
                    .AddSafeField("Next Map", results.MapNameNext, true)
                    .WithColor(Tf2Color);

                if (!string.IsNullOrWhiteSpace(results.MapName))
                {
                    try
                    {
                        var thumbnail = await api.GetMapThumbnailAsync(results.MapName).ConfigureAwait(false);
                        if (!string.IsNullOrWhiteSpace(thumbnail?.Name)) output.WithImageUrl(thumbnail.Name);
                    }
                    catch
                    {
                        // The thumbnail is optional.
                    }
                }

                return output.Build();
            }
            catch
            {
                return null;
            }
        }

        public static async Task<DiscordEmbed> GetCustomServerListsAsync(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token)) return null;
                var results = await new TeamworkAPI(token).GetServerListsAsync().ConfigureAwait(false);
                if (results is null || results.Count == 0) return null;

                var output = new DiscordEmbedBuilder()
                    .WithFooter("Run the command again for a different set of server lists.")
                    .WithColor(Tf2Color);
                foreach (var list in results.OrderBy(_ => Random.Next()).Take(4))
                {
                    var desc = (list.DescriptionLong ?? string.Empty).StripHtml().Truncate(200);
                    output.AddSafeField($"Created By: {list.Creator?.Name ?? EmbedExtensions.Unknown} \t Subscribers: {list.Subscribed}", $"[{list.Name}]({Resources.URL_TeamworkTF + list.Id}) - {desc}");
                }

                return output.Build();
            }
            catch
            {
                return null;
            }
        }

        public static async Task<DiscordEmbed> GetMapStatsAsync(string token, string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(query)) return null;
                var api = new TeamworkAPI(token);
                var map = (await api.GetMapsBySearchAsync(query.Trim()).ConfigureAwait(false))?.FirstOrDefault()?.Name;
                if (string.IsNullOrWhiteSpace(map)) return null;

                var results = await api.GetMapStatsAsync(map).ConfigureAwait(false);
                if (results is null || string.IsNullOrWhiteSpace(results.MapName)) return null;

                _ = double.TryParse(results.AllTimeAvgPlayers, NumberStyles.Float, CultureInfo.InvariantCulture, out var avgPlayers);
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.MapName)
                    .AddSafeField("Highest Server Count", results.HighestServers.ToString(), true)
                    .AddSafeField("Highest Player Count", results.HighestPlayers.ToString(), true)
                    .AddSafeField("Avg. Players", Math.Round(avgPlayers, 2).ToString(CultureInfo.InvariantCulture), true)
                    .WithFooter("Statistics retrieved from teamwork.tf - refreshed every 5 minutes")
                    .WithUrl(string.Format(Resources.URL_TF2Wiki, results.MapName))
                    .WithColor(Tf2Color);
                if (results.Thumbnail is not null) output.WithImageUrl(results.Thumbnail);

                if (results.RelatedMaps is { Count: > 0 })
                    output.AddSafeField("Related Map(s)", string.Join("\n", results.RelatedMaps.Take(4)), true);

                if (results.ExtraInfo != null)
                {
                    var links = new List<string>();
                    if (results.ExtraInfo.SteamWorkshopUrl != null) links.Add($"[Steam Workshop]({results.ExtraInfo.SteamWorkshopUrl})");
                    if (results.ExtraInfo.Tf2MapsUrl != null) links.Add($"[TF2Maps]({results.ExtraInfo.Tf2MapsUrl})");
                    if (results.ExtraInfo.GameBananaUrl != null) links.Add($"[GameBanana]({results.ExtraInfo.GameBananaUrl})");
                    if (links.Count > 0) output.AddSafeField("Links", string.Join(" **|** ", links), true);
                }

                if (results.GameModes is { Count: > 0 })
                {
                    try
                    {
                        var mode = await api.GetGameModeAsync(results.GameModes[0]).ConfigureAwait(false);
                        if (mode is not null)
                        {
                            output.WithDescription($"{mode.Title} - {mode.Description}");
                            if (!string.IsNullOrWhiteSpace(mode.Color)) output.WithColor(new DiscordColor($"#{mode.Color.TrimStart('#')}"));
                        }
                    }
                    catch
                    {
                        // The game mode description is optional.
                    }
                }

                return output.Build();
            }
            catch
            {
                return null;
            }
        }

        public static string GetServerBanner(string token, string address)
        {
            return new TeamworkAPI(token).GetServerBanner(address);
        }
    }
}
