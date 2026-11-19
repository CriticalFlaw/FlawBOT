using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using Newtonsoft.Json.Linq;
using Steam.Models;
using Steam.Models.SteamCommunity;
using Steam.Models.SteamStore;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public partial class SteamService : HttpHandler
    {
        private static IReadOnlyCollection<SteamAppModel> SteamAppList { get; set; } = [];

        /// <summary>
        ///     Separate client for SteamWebAPI2, which configures the client it is given.
        /// </summary>
        internal static readonly HttpClient SteamHttp = new() { Timeout = TimeSpan.FromSeconds(60) };

        public static async Task<DiscordEmbed> GetSteamGameAsync(string token, string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                var appId = await FindAppIdAsync(query.Trim()).ConfigureAwait(false);
                if (appId is null) return null;

                var factory = new SteamWebInterfaceFactory(string.IsNullOrWhiteSpace(token) ? "-" : token);
                var results = await factory.CreateSteamStoreInterface(SteamHttp).GetStoreAppDetailsAsync(appId.Value).ConfigureAwait(false);
                return BuildGameEmbed(results);
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed BuildGameEmbed(StoreAppDetailsDataModel results)
        {
            if (results is null || string.IsNullOrWhiteSpace(results.Name)) return null;
            var description = (string.IsNullOrWhiteSpace(results.ShortDescription) ? results.DetailedDescription : results.ShortDescription).StripHtml();
            var output = new DiscordEmbedBuilder()
                .WithTitle(results.Name)
                .WithDescription(string.IsNullOrWhiteSpace(description) ? EmbedExtensions.Unknown : description.Truncate(500))
                .AddSafeField("Release Date", results.ReleaseDate?.Date, true)
                .AddSafeField("Developers", results.Developers?.FirstOrDefault(), true)
                .AddSafeField("Publisher", results.Publishers?.FirstOrDefault(), true)
                .AddSafeField("Price", results.IsFree ? "Free" : results.PriceOverview?.FinalFormatted, true)
                .AddSafeField("Metacritic", results.Metacritic?.Score.ToString(), true)
                .AddSafeField("Genres", results.Genres?.Take(3).Select(x => x.Description).JoinOrDefault(), true)
                .WithUrl(string.Format(Resources.URL_Steam_App, results.SteamAppId))
                .WithFooter("App ID: " + results.SteamAppId)
                .WithColor(new DiscordColor("#1B2838"));
            if (!string.IsNullOrWhiteSpace(results.HeaderImage)) output.WithThumbnail(results.HeaderImage);
            return output.Build();
        }

        /// <summary>
        ///     Finds a game's app ID by exact name in the cached app list, falling back to the Steam store search.
        /// </summary>
        public static async Task<uint?> FindAppIdAsync(string query)
        {
            if (uint.TryParse(query, out var id)) return id;
            var match = SteamAppList.FirstOrDefault(n => string.Equals(n.Name, query, StringComparison.InvariantCultureIgnoreCase));
            if (match is not null) return match.AppId;

            try
            {
                var response = await Http.GetStringAsync(string.Format(Resources.URL_Steam_Search, Uri.EscapeDataString(query))).ConfigureAwait(false);
                var items = JObject.Parse(response)["items"] as JArray;
                return items?.FirstOrDefault()?["id"]?.Value<uint>();
            }
            catch
            {
                return null;
            }
        }

        public static async Task<DiscordEmbed> GetSteamProfileAsync(string token, string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(query)) return null;
                var steamId = await GetSteamUserIdAsync(token, query).ConfigureAwait(false);
                if (steamId is null) return null;

                var steam = new SteamWebInterfaceFactory(token).CreateSteamWebInterface<SteamUser>(SteamHttp);
                var results = await steam.GetPlayerSummaryAsync(steamId.Value).ConfigureAwait(false);
                return BuildProfileEmbed(results?.Data);
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed BuildProfileEmbed(PlayerSummaryModel profile)
        {
            if (profile is null || string.IsNullOrWhiteSpace(profile.Nickname)) return null;
            var output = new DiscordEmbedBuilder()
                .WithTitle(profile.Nickname)
                .AddSafeField("Status", profile.UserStatus.ToString(), true)
                .AddSafeField("Country", string.IsNullOrWhiteSpace(profile.CountryCode) ? null : $":flag_{profile.CountryCode.ToLowerInvariant()}: {profile.CountryCode}", true)
                .WithColor(new DiscordColor("#1B2838"))
                .WithFooter("Steam ID: " + profile.SteamId);
            if (!string.IsNullOrWhiteSpace(profile.ProfileUrl)) output.WithUrl(profile.ProfileUrl);
            if (!string.IsNullOrWhiteSpace(profile.AvatarFullUrl)) output.WithThumbnail(profile.AvatarFullUrl);
            if (profile.UserStatus == Steam.Models.SteamCommunity.UserStatus.Offline && profile.LastLoggedOffDate > DateTime.UnixEpoch)
                output.AddSafeField("Last seen", profile.LastLoggedOffDate.ToUniversalTime().ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture));
            if (!string.IsNullOrWhiteSpace(profile.PlayingGameName))
            {
                output.AddSafeField("Now Playing", $"`{profile.PlayingGameName}`", true);
                output.WithColor(new DiscordColor("#79A14D"));
            }

            return output.Build();
        }

        /// <summary>
        ///     Resolves a 64-bit Steam ID, a profile URL or a vanity name to a Steam ID.
        /// </summary>
        public static async Task<ulong?> GetSteamUserIdAsync(string token, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;
            var vanity = ParseSteamUserQuery(query, out var steamId);
            if (steamId is not null) return steamId;
            if (string.IsNullOrWhiteSpace(token) || vanity is null) return null;

            try
            {
                var steam = new SteamWebInterfaceFactory(token).CreateSteamWebInterface<SteamUser>(SteamHttp);
                var response = await steam.ResolveVanityUrlAsync(vanity).ConfigureAwait(false);
                return response?.Data is > 0 ? response.Data : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Splits a Steam user query into either a 64-bit Steam ID or a vanity name.
        /// </summary>
        public static string ParseSteamUserQuery(string query, out ulong? steamId)
        {
            steamId = null;
            var value = query.Trim().TrimEnd('/');
            var slash = value.LastIndexOf('/');
            if (slash >= 0) value = value[(slash + 1)..];
            value = value.Replace(" ", string.Empty);

            // 64-bit Steam IDs for individual accounts start at 76561197960265728.
            if (value.Length == 17 && ulong.TryParse(value, out var id) && id >= 76561197960265728UL)
            {
                steamId = id;
                return null;
            }

            return string.IsNullOrEmpty(value) ? null : value;
        }

        /// <summary>
        ///     Turns "IP:PORT" or "IP:PORT; password PASS" into a steam://connect link. Returns null for invalid input.
        /// </summary>
        public static string FormatConnectLink(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            var match = ConnectRegex().Match(input);
            if (!match.Success) return null;
            var link = $"steam://connect/{match.Groups["ip"].Value}";
            return match.Groups["pw"].Success ? $"{link}/{match.Groups["pw"].Value}" : link;
        }

        [GeneratedRegex(@"^\s*(?:connect\s+)?(?<ip>[A-Za-z0-9.\-]+(?::\d{1,5})?)\s*(?:;\s*password\s+""?(?<pw>[^\s""]+)""?)?\s*;?\s*$", RegexOptions.IgnoreCase)]
        private static partial Regex ConnectRegex();

        public static async Task<bool> UpdateSteamAppListAsync(string token)
        {
            try
            {
                var response = await new SteamWebInterfaceFactory(token).CreateSteamWebInterface<SteamApps>(SteamHttp).GetAppListAsync().ConfigureAwait(false);
                if (response?.Data is null || response.Data.Count == 0) return false;
                SteamAppList = response.Data;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.ERR_STEAM_LIST, ex.Message);
                return false;
            }
        }
    }
}
