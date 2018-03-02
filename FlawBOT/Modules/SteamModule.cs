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
        [Command("tf2")]
        [Description("Retrieve a Team Fortress item from schema")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task TF2Item(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            APITokenService service = new APITokenService();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string Token = service.GetAPIToken("steam");
            EconItems schema = new EconItems(Token, EconItemsAppId.TeamFortress2);
            string classes = null;
            var items = await schema.GetSchemaForTF2Async();
            Random RND = new Random();
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

        [Hidden]
        [Command("tf2search")]
        [Description("Retrieve a Team Fortress item by search")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task TF2ItemSearch(CommandContext CTX, [RemainingText] string query)
        {
            var items = from pv in GlobalVariables.itemSchema where pv.Value.Contains(query) select pv.Key;
            var index = Convert.ToInt32(items);
            if (index == 0)
                await CTX.RespondAsync(":warning: No results found!");
            else
            {
                await CTX.TriggerTypingAsync();
                APITokenService service = new APITokenService();
                string Token = service.GetAPIToken("steam");
                EconItems client = new EconItems(Token, EconItemsAppId.TeamFortress2);
                var schema = await client.GetSchemaForTF2Async();
                string wikiName = schema.Data.Items[index].ItemName.Replace(' ', '_');
                var output = new DiscordEmbedBuilder()
                    .WithTitle(schema.Data.Items[index].ItemName)
                    .WithThumbnailUrl(schema.Data.Items[index].ImageUrl)
                    .WithUrl($"https://wiki.teamfortress.com/wiki/{wikiName}")
                    .WithColor(DiscordColor.Orange);
                if (schema.Data.Items[index].ItemDescription != null)
                    output.WithDescription(schema.Data.Items[index].ItemDescription);
                if (schema.Data.Items[index].ItemSlot != null)
                    output.AddField("Item Slot:", schema.Data.Items[index].ItemSlot);
                if (schema.Data.Items[index].ModelPlayer != null)
                    output.AddField("Model Path:", schema.Data.Items[index].ModelPlayer);
                await CTX.RespondAsync(embed: output.Build());
            }
        }

        [Command("tf2wiki")]
        [Description("Get a page from the Team Fortress 2 wiki")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task TF2Wiki(CommandContext CTX, [RemainingText] string query)
        {
            using (var http = new HttpClient())
            {
                await CTX.TriggerTypingAsync();
                string search = null;
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
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
                    await CTX.RespondAsync(":warning: TF2Wiki page not found").ConfigureAwait(false);
                else
                    await CTX.Channel.SendMessageAsync(data.Query.Pages[0].FullUrl).ConfigureAwait(false);
            }
        }

        [Command("steamgame")]
        [Aliases("sg")]
        [Description("Retrieve information on specific Steam game")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SteamGame(CommandContext CTX, [RemainingText] string query)
        {
            try
            {
                int index = 0;
                foreach (var app in GlobalVariables.gameList)
                {
                    if (app.Value.ToUpperInvariant() == query.ToUpperInvariant())
                    {
                        index = app.Key;
                        break;
                    }
                }
                if (index != 0)
                {
                    //http://api.steampowered.com/ISteamApps/GetAppList/v0002/
                    //https://steamdb.info/api/GetAppList/
                    await CTX.TriggerTypingAsync();
                    SteamStore steam = new SteamStore();
                    var profile = await steam.GetStoreAppDetailsAsync(Convert.ToUInt32(index));
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(profile.Name)
                        .WithDescription((Regex.Replace(profile.DetailedDescription.Substring(0, 500), "<[^>]*>", "")))
                        .WithThumbnailUrl(profile.HeaderImage)
                        .AddField("About", Regex.Replace(profile.AboutTheGame.Substring(0, 500), "<[^>]*>", ""))
                        .AddField("Developers", profile.Developers[0], true)
                        .AddField("Publisher", profile.Publishers[0], true)
                        .AddField("Release Date", profile.ReleaseDate.Date, true)
                        .AddField("Genres", profile.Genres[0].Description, true)
                        .AddField("Metacritic", profile.Metacritic.Score.ToString(), true)
                        .WithFooter($"App ID: {profile.SteamAppId.ToString()}")
                        .WithColor(DiscordColor.MidnightBlue);
                    if (profile.MacRequirements.Minimum != null)
                        output.AddField("Metacritic", profile.MacRequirements.Minimum, true);
                    await CTX.RespondAsync(embed: output.Build());
                }
            }
            catch (Exception ex)
            {
                await CTX.RespondAsync($":warning: Unable to retrieve Steam game, please enter a valid App ID like **.steamgame 440** {ex.Message}");
            }
        }

        [Command("steamuser")]
        [Aliases("su")]
        [Description("Retrieve Steam user information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SteamUser(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await CTX.RespondAsync(":warning: Please provide a Steam user to search for, try **.steamuser criticalflaw**");
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
                            if (profile.Summary != null)
                                output.WithDescription(Regex.Replace(profile.Summary, "<[^>]*>", ""));
                            if (summary.Data.UserStatus != Steam.Models.SteamCommunity.UserStatus.Offline)
                                output.AddField("Status:", summary.Data.UserStatus.ToString(), true);
                            else
                                output.AddField("Last seen:", summary.Data.LastLoggedOffDate.ToUniversalTime().ToString(), true);
                            if (summary.Data.PlayingGameName != null)
                                output.AddField("Playing: ", $"{summary.Data.PlayingGameName}");
                            output.AddField("VAC Banned?:", profile.IsVacBanned ? "YES" : "NO", true);
                            output.AddField("Trade Banned?:", profile.TradeBanState, true);
                            output.AddField("Favorite Group:", summary.Data.PrimaryGroupId);
                            if (profile.InGameInfo != null)
                            {
                                output.AddField("In-Game:", $"[{profile.InGameInfo.GameName}]({profile.InGameInfo.GameLink})", true);
                                output.AddField("Game Server IP:", profile.InGameServerIP, true);
                                output.WithImageUrl(profile.InGameInfo.GameLogoSmall);
                            }
                            await CTX.RespondAsync(embed: output.Build());
                        }
                        else
                        {
                            output.Description = "This profile is private...";
                            await CTX.RespondAsync(embed: output.Build());
                        }
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
    }
}