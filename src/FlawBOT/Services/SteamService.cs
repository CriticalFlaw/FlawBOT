using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using Steam.Models;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace FlawBOT.Services
{
    public class SteamService : HttpHandler
    {
        private static SteamWebInterfaceFactory _steamInterface;

        private static ISteamWebResponse<IReadOnlyCollection<SteamAppModel>> SteamAppList { get; set; } = new SteamWebResponse<IReadOnlyCollection<SteamAppModel>>();

        public static async Task<DiscordEmbed> GetSteamGame(string query)
        {
            try
            {
                var appId = SteamAppList.Data.First(n => string.Equals(n.Name, query, StringComparison.InvariantCultureIgnoreCase)).AppId;
                var results = await new SteamWebInterfaceFactory(Program.Settings.Tokens.SteamToken).CreateSteamStoreInterface().GetStoreAppDetailsAsync(appId).ConfigureAwait(false);

                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.Name)
                    .WithDescription(Regex.Replace(results.DetailedDescription.Length <= 500
                        ? results.DetailedDescription
                        : results.DetailedDescription.Substring(0, 250) + "...", "<[^>]*>", "") ?? "Unknown")
                    .AddField("Release Date", results.ReleaseDate.Date ?? "Unknown", true)
                    .AddField("Developers", results.Developers.FirstOrDefault() ?? "Unknown", true)
                    .AddField("Publisher", results.Publishers.FirstOrDefault() ?? "Unknown", true)
                    .AddField("Price", results.IsFree ? "Free" : results.PriceOverview.FinalFormatted ?? "Unknown", true)
                    .AddField("Metacritic", results.Metacritic != null ? results.Metacritic.Score.ToString() : "Unknown", true)
                    .WithThumbnail(results.HeaderImage)
                    .WithUrl(string.Format(Resources.URL_Steam_App, results.SteamAppId))
                    .WithFooter("App ID: " + results.SteamAppId)
                    .WithColor(new DiscordColor("#1B2838"));

                var genres = new StringBuilder();
                foreach (var genre in results.Genres.Take(3))
                    genres.Append(genre.Description).Append(!genre.Equals(results.Genres.Last()) ? ", " : string.Empty);
                output.AddField("Genres", genres.ToString() ?? "Unknown", true);
                return output.Build();
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
                if (string.IsNullOrWhiteSpace(query)) return null;
                _steamInterface = new SteamWebInterfaceFactory(token);
                var steam = _steamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
                var userId = ulong.TryParse(query, out var steamId) ? steamId : 0;

                var data = GetSteamUserId(query).Result;
                if (data is null) return null;
                var results = await steam.GetPlayerSummaryAsync(data.Data).ConfigureAwait(false);

                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.Data.Nickname)
                    .AddField("Status", results.Data.UserStatus.ToString(), true)
                    .AddField("Country", $":flag_{results.Data.CountryCode.ToLowerInvariant()}: {results.Data.CountryCode}", true)
                    .WithUrl(results.Data.ProfileUrl)
                    .WithThumbnail(results.Data.AvatarFullUrl)
                    .WithColor(new DiscordColor("#1B2838"))
                    .WithFooter("Steam ID: " + results.Data.SteamId);

                if (results.Data.UserStatus == Steam.Models.SteamCommunity.UserStatus.Offline)
                    output.AddField("Last seen", results.Data.LastLoggedOffDate.ToUniversalTime().ToString(CultureInfo.CurrentCulture));

                if (results.Data.PlayingGameId != null)
                {
                    output.AddField("Now Playing", $"`{results.Data.PlayingGameName}`", true);
                    output.WithColor(new DiscordColor("#79A14D"));
                }
                
                return output.Build();
            }
            catch
            {
                return null;
            }
        }

        public static async Task<ISteamWebResponse<ulong>> GetSteamUserId(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                var steam = _steamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
                return await steam.ResolveVanityUrlAsync(query.Replace(" ", "")).ConfigureAwait(false) ?? null;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<bool> UpdateSteamAppListAsync(string token)
        {
            try
            {
                _steamInterface = new SteamWebInterfaceFactory(token);
                SteamAppList = await _steamInterface.CreateSteamWebInterface<SteamApps>(new HttpClient()).GetAppListAsync();
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