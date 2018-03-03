using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Services;
using Newtonsoft.Json;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class SteamModule
    {
        [Command("steamgame")]
        [Aliases("sg")]
        [Description("Retrieve information on specific Steam game")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SteamGame(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            Random RND = new Random();
            SteamStore steam = new SteamStore();
            var index = GlobalVariables.SteamAppList.Keys.ToArray()[RND.Next(0, GlobalVariables.SteamAppList.Keys.Count - 1)];
            var app = await steam.GetStoreAppDetailsAsync(index);
            var output = new DiscordEmbedBuilder()
                .WithTitle(app.Name)
                .WithThumbnailUrl(app.HeaderImage)
                .WithFooter($"App ID: {app.SteamAppId.ToString()}")
                .WithColor(DiscordColor.MidnightBlue);
            if (!string.IsNullOrWhiteSpace(app.DetailedDescription))
                output.WithDescription((Regex.Replace(app.DetailedDescription.Substring(0, 500), "<[^>]*>", "")));
            if (!string.IsNullOrWhiteSpace(app.AboutTheGame))
                output.AddField("About", Regex.Replace(app.AboutTheGame.Substring(0, 500), "<[^>]*>", ""));
            if (!string.IsNullOrWhiteSpace(app.Developers[0]))
                output.AddField("Developers", app.Developers[0], true);
            if (!string.IsNullOrWhiteSpace(app.Publishers[0]))
                output.AddField("Publisher", app.Publishers[0], true);
            if (!string.IsNullOrWhiteSpace(app.ReleaseDate.Date))
                output.AddField("Release Date", app.ReleaseDate.Date, true);
            if (!string.IsNullOrWhiteSpace(app.Genres[0].Description))
                output.AddField("Genres", app.Genres[0].Description, true);
            if (!string.IsNullOrWhiteSpace(app.Metacritic.Score.ToString()))
                output.AddField("Metacritic", app.Metacritic.Score.ToString(), true);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("steamuser")]
        [Aliases("su")]
        [Description("Retrieve Steam user information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SteamUser(CommandContext CTX, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync(":warning: SteamID or Community URL are required! Try **.su criticalflaw** :warning:");
            else
            {
                APITokenService service = new APITokenService();
                string Token = service.GetAPIToken("steam");
                SteamUser steam = new SteamUser(Token);
                SteamCommunityProfileModel profile = null;
                ISteamWebResponse<PlayerSummaryModel> summary = null;
                try
                {
                    var decode = await steam.ResolveVanityUrlAsync(query);
                    profile = await steam.GetCommunityProfileAsync(decode.Data).ConfigureAwait(false);
                    summary = await steam.GetPlayerSummaryAsync(decode.Data).ConfigureAwait(false);
                }
                catch
                {
                    profile = await steam.GetCommunityProfileAsync(ulong.Parse(query)).ConfigureAwait(false);
                    summary = await steam.GetPlayerSummaryAsync(ulong.Parse(query)).ConfigureAwait(false);
                }
                finally
                {
                    if (profile != null && summary != null)
                    {
                        await CTX.TriggerTypingAsync();
                        var output = new DiscordEmbedBuilder()
                            .WithTitle(summary.Data.Nickname);
                        if (summary.Data.ProfileVisibility == ProfileVisibility.Public)
                        {
                            output.WithThumbnailUrl(profile.AvatarFull.ToString());
                            output.WithColor(DiscordColor.MidnightBlue);
                            output.WithUrl($"http://steamcommunity.com/id/{profile.SteamID}/");
                            output.WithFooter($"Steam ID: {profile.SteamID}");
                            output.AddField("Member since", summary.Data.AccountCreatedDate.ToUniversalTime().ToString(), true);
                            if (!string.IsNullOrWhiteSpace(profile.Summary))
                                output.WithDescription(Regex.Replace(profile.Summary, "<[^>]*>", ""));
                            if (summary.Data.UserStatus != Steam.Models.SteamCommunity.UserStatus.Offline)
                                output.AddField("Status:", summary.Data.UserStatus.ToString(), true);
                            else
                                output.AddField("Last seen:", summary.Data.LastLoggedOffDate.ToUniversalTime().ToString(), true);
                            output.AddField("VAC Banned?:", profile.IsVacBanned ? "YES" : "NO", true);
                            output.AddField("Trade Banned?:", profile.TradeBanState, true);
                            if (profile.InGameInfo != null)
                            {
                                output.AddField("In-Game:", $"[{profile.InGameInfo.GameName}]({profile.InGameInfo.GameLink})", true);
                                output.AddField("Game Server IP:", profile.InGameServerIP, true);
                                output.WithImageUrl(profile.InGameInfo.GameLogoSmall);
                            }
                        }
                        else
                            output.Description = "This profile is private...";
                        await CTX.RespondAsync(embed: output.Build());
                    }
                    else
                        await CTX.RespondAsync(":warning: No results found! :warning:");
                }
            }
        }

        [Command("steamlink")]
        [Aliases("sl")]
        [Description("Format TF2 connection information into a clickable link")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SteamServerLink(CommandContext CTX, [RemainingText] string link)
        {
            await CTX.TriggerTypingAsync();
            Regex Regexes = new Regex(@"connect\s*(?'ip'\S+)\s*;\s*password\s*(?'pw'\S+);?", RegexOptions.Compiled);
            Match match = Regexes.Match(link);
            if (match.Success)
                await CTX.RespondAsync(string.Format($"steam://connect/{match.Groups["ip"].Value}/{match.Groups["pw"].Value}"));
            else
                await CTX.RespondAsync($"Invalid connection info, follow the format: **connect 123.345.56.789:00000; password hello**");
        }

        [Command("tf2")]
        [Description("Retrieve a Team Fortress item from schema")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task TF2Item(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            Random RND = new Random();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            APITokenService service = new APITokenService();
            string Token = service.GetAPIToken("steam");
            EconItems schema = new EconItems(Token, EconItemsAppId.TeamFortress2);
            string classes = null;
            var items = await schema.GetSchemaForTF2Async();
            int index = RND.Next(0, items.Data.Items.Count);
            string wikiName = items.Data.Items[index].ItemName.Replace(' ', '_');
            var output = new DiscordEmbedBuilder()
                .WithTitle(items.Data.Items[index].ItemName)
                .WithThumbnailUrl(items.Data.Items[index].ImageUrl)
                .WithUrl($"https://wiki.teamfortress.com/wiki/{wikiName}")
                .WithColor(DiscordColor.Orange);
            if (items.Data.Items[index].ItemDescription != null)
                output.WithDescription(items.Data.Items[index].ItemDescription);
            if (items.Data.Items[index].ItemSlot != null)
                output.AddField("Item Slot:", textInfo.ToTitleCase(items.Data.Items[index].ItemSlot));
            foreach (var userClass in items.Data.Items[index].UsedByClasses)
                classes += $" {userClass} ";
            if (classes != null)
                output.AddField("Used by:", textInfo.ToTitleCase(classes));
            if (items.Data.Items[index].ModelPlayer != null)
                output.AddField("Model Path:", items.Data.Items[index].ModelPlayer);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("tf2wiki")]
        [Description("Get a page from the Team Fortress 2 wiki")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task TF2Wiki(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync(":warning: TF2Wiki search query is a required!");
            else
            {
                await CTX.TriggerTypingAsync();
                HttpClient http = new HttpClient();
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                string search = null;
                foreach (string term in query.Split(' '))
                {
                    if (term.Length <= 2)
                        search += $"{term} ";
                    else
                        search += $"{textInfo.ToTitleCase(term)} ";
                }
                var result = await http.GetStringAsync($"https://wiki.teamfortress.com/w/api.php?action=query&format=json&prop=info&redirects=1&formatversion=2&inprop=url&titles={Uri.EscapeDataString(search.Replace(' ', '_').Trim())}");
                var data = JsonConvert.DeserializeObject<WikipediaService>(result);
                if (data.Query.Pages[0].Missing)
                    await CTX.RespondAsync(":warning: TF2Wiki page not found! :warning:").ConfigureAwait(false);
                else
                    await CTX.Channel.SendMessageAsync(data.Query.Pages[0].FullUrl).ConfigureAwait(false);
            }
        }
    }
}