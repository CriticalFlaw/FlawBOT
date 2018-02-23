using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Services;
using Newtonsoft.Json;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class SteamModule
    {
        [Command("steamgame")]
        [Aliases("sg")]
        [Description("Retrieve information on specific Steam game")]
        public async Task SearchSteamGame(CommandContext CTX, uint query)
        {
            await CTX.TriggerTypingAsync();
            var JSON = "";  // Load the configuration file
            using (var SRD = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)))
                JSON = await SRD.ReadToEndAsync();
            string Token = JsonConvert.DeserializeObject<APITokenService.APITokenList>(JSON).SteamToken;
            SteamStore steam = new SteamStore();
            ISteamWebResponse<SteamCommunityProfileModel> summary = null;
            var profile = await steam.GetStoreAppDetailsAsync(query).ConfigureAwait(false);
            var output = new DiscordEmbedBuilder()
                .WithTitle(profile.Name)
                .WithDescription(profile.DetailedDescription.Substring(0, 500))
                .WithThumbnailUrl(profile.HeaderImage)
                .AddField("About", profile.AboutTheGame.Substring(0, 500))
                .AddField("Developers", profile.Developers.ToString())
                .AddField("Genres", profile.Genres.ToString())
                .AddField("Metacritic", profile.Metacritic.ToString())
                .AddField("PC Requirements", profile.PcRequirements.ToString())
                .AddField("Publisher", profile.Publishers.ToString())
                .AddField("Release Date", profile.ReleaseDate.ToString())
                .WithFooter(profile.SteamAppId.ToString())
                .WithColor(DiscordColor.MidnightBlue);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("steamuser")]
        [Aliases("su")]
        [Description("Retrieve Steam user information")]
        public async Task SearchSteamUser(CommandContext CTX, [RemainingText] string query)
        {
            await CTX.TriggerTypingAsync();
            var JSON = "";  // Load the configuration file
            using (var SRD = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)))
                JSON = await SRD.ReadToEndAsync();
            string Token = JsonConvert.DeserializeObject<APITokenService.APITokenList>(JSON).SteamToken;
            SteamUser steam = new SteamUser(Token);
            SteamCommunityProfileModel profile = null;
            ISteamWebResponse<PlayerSummaryModel> summary = null;

            if (ulong.TryParse(query, out ulong steamid))
            {
                profile = await steam.GetCommunityProfileAsync(steamid).ConfigureAwait(false);
                summary = await steam.GetPlayerSummaryAsync(steamid).ConfigureAwait(false);
                var output = new DiscordEmbedBuilder()
                    .WithTitle(summary.Data.Nickname)
                    .WithDescription(Regex.Replace(profile.Summary, "<[^>]*>", ""))
                    .WithThumbnailUrl(profile.AvatarFull.ToString())
                    .WithColor(DiscordColor.Black)
                    .WithUrl($"http://steamcommunity.com/id/{profile.SteamID}/")
                    .WithFooter($"Steam ID: {profile.SteamID}");

                if (summary.Data.ProfileVisibility == ProfileVisibility.Public)
                {
                    output.AddField("Member since", summary.Data.AccountCreatedDate.ToUniversalTime().ToString(), true);
                    if (summary.Data.UserStatus != Steam.Models.SteamCommunity.UserStatus.Offline)
                        output.AddField("Status:", summary.Data.UserStatus.ToString(), true);
                    else
                        output.AddField("Last seen:", summary.Data.LastLoggedOffDate.ToUniversalTime().ToString(), true);
                    if (summary.Data.PlayingGameName != null)
                        output.AddField("Playing: ", $"{summary.Data.PlayingGameName}");
                    output.AddField("VAC Banned?:", profile.IsVacBanned ? "YES" : "NO", true);
                    output.AddField("Status:", summary.Data.UserStatus.ToString(), true);
                    output.AddField("Trade Banned?:", profile.TradeBanState, true);
                    await CTX.RespondAsync(embed: output.Build());
                }
                else
                {
                    output.Description = "This profile is private.";
                    await CTX.RespondAsync(embed: output.Build());
                }
            }
        }

        [Command("steamlink")]
        [Description("Format TF2 connection information into a clickable link")]
        public async Task FormatSteamLink(CommandContext CTX, [RemainingText] string link)
        {
            await CTX.TriggerTypingAsync();
            Regex Regexes = new Regex(@"connect\s*(?'ip'\S+)\s*;\s*password\s*(?'pw'\S+);?", RegexOptions.Compiled);
            Match match = Regexes.Match(link);
            if (match.Success)
                await CTX.RespondAsync(string.Format($"steam://connect/{match.Groups["ip"].Value}/{match.Groups["pw"].Value}"));
            else
                await CTX.RespondAsync($"Invalid connection info, follow the format: *connect 123.345.56.789:00000; password hello*");
        }
    }
}