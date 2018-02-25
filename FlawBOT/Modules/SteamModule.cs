using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Services;
using Newtonsoft.Json;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class SteamModule
    {
        [Command("tf2")]
        [Aliases("tf2wiki")]
        [Description("Get a page from the Team Fortress 2 wiki")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchTFWiki(CommandContext CTX, [RemainingText] string query)
        {
            await CTX.TriggerTypingAsync();
            using (var http = new HttpClient())
            {
                var result = await http.GetStringAsync("https://wiki.teamfortress.com/w/api.php?action=query&format=json&prop=info&redirects=1&formatversion=2&inprop=url&titles=" + Uri.EscapeDataString(query));
                var data = JsonConvert.DeserializeObject<WikipediaService>(result);
                if (data.Query.Pages[0].Missing)
                    await CTX.RespondAsync("TF2 Wiki page not found").ConfigureAwait(false);
                else
                    await CTX.Channel.SendMessageAsync(data.Query.Pages[0].FullUrl).ConfigureAwait(false);
            }
        }

        [Hidden]
        [Command("tf2wep")]
        [Description("Retrieve a Team Fortress item from schema")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchTFItem(CommandContext CTX, [RemainingText] string query)
        {
            // OPTION 1: Manual
            await CTX.TriggerTypingAsync();
            using (var http = new HttpClient())
            {
                APITokenService service = new APITokenService();
                string Token = service.GetAPIToken("steam");
                var result = await http.GetStringAsync($"http://api.steampowered.com/IEconItems_440/GetSchema/v0001/?key={Token}");
                var data = JsonConvert.DeserializeObject<SteamService.TF2Service>(result);
                await CTX.Channel.SendMessageAsync($"{data.Query.result.items[0].name}").ConfigureAwait(false);
            }

            // OPTION 2: Automatic
            //await CTX.TriggerTypingAsync();
            //APITokenService service = new APITokenService();
            //string Token = service.GetAPIToken("steam");
            //IEconItems schema = null;
            //var items = await schema.GetSchemaForTF2Async();
            ////TODO: Get only the searched item
            //var output = new DiscordEmbedBuilder()
            //    .WithTitle(items.Data.Items[0].ItemName)
            //    .AddField("CraftClass", items.Data.Items[0].CraftClass)
            //    .AddField("CraftMaterialType", items.Data.Items[0].CraftMaterialType)
            //    .AddField("DropType", items.Data.Items[0].DropType)
            //    .AddField("DefIndex", items.Data.Items[0].DefIndex.ToString())
            //    .AddField("HolidayRestriction", items.Data.Items[0].HolidayRestriction)
            //    .AddField("ItemClass", items.Data.Items[0].ItemClass)
            //    .AddField("ItemDescription", items.Data.Items[0].ItemDescription)
            //    .AddField("ItemLogName", items.Data.Items[0].ItemLogName)
            //    .AddField("ItemName", items.Data.Items[0].ItemName)
            //    .AddField("ItemQuality", items.Data.Items[0].ItemQuality.ToString())
            //    .AddField("ItemSet", items.Data.Items[0].ItemSet)
            //    .AddField("ItemSlot", items.Data.Items[0].ItemSlot)
            //    .AddField("ItemTypeName", items.Data.Items[0].ItemTypeName)
            //    .AddField("MaxIlevel", items.Data.Items[0].MaxIlevel.ToString())
            //    .AddField("MinIlevel", items.Data.Items[0].MinIlevel.ToString())
            //    .AddField("ModelPlayer", items.Data.Items[0].ModelPlayer)
            //    .AddField("Name", items.Data.Items[0].Name)
            //    //.AddField("PerClassLoadoutSlots", items.Data.Items[0].PerClassLoadoutSlots)
            //    .AddField("ProperName", items.Data.Items[0].ProperName.ToString())
            //    //.AddField("Styles", items.Data.Items[0].Styles)
            //    //.AddField("Tool", items.Data.Items[0].Tool)
            //    //.AddField("UsedByClasses", items.Data.Items[0].UsedByClasses)
            //    .WithThumbnailUrl(items.Data.Items[0].ImageUrl)
            //    .WithColor(DiscordColor.Orange);
            //await CTX.RespondAsync(embed: output.Build());
        }

        [Command("steamgame")]
        [Aliases("sg")]
        [Description("Retrieve information on specific Steam game")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchSteamGame(CommandContext CTX, uint query)
        {
            await CTX.TriggerTypingAsync();
            SteamStore steam = new SteamStore();
            var profile = await steam.GetStoreAppDetailsAsync(query);
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
                .WithFooter($"Steam App ID: {profile.SteamAppId.ToString()}")
                .WithColor(DiscordColor.MidnightBlue);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("steamuser")]
        [Aliases("su")]
        [Description("Retrieve Steam user information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SearchSteamUser(CommandContext CTX, [RemainingText] string query)
        {
            await CTX.TriggerTypingAsync();
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
                    var output = new DiscordEmbedBuilder()
                    .WithTitle(summary.Data.Nickname)
                    .WithDescription(Regex.Replace(profile.Summary, "<[^>]*>", ""))
                    .WithThumbnailUrl(profile.AvatarFull.ToString())
                    .WithColor(DiscordColor.MidnightBlue)
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
        }

        [Command("steamlink")]
        [Aliases("sl")]
        [Description("Format TF2 connection information into a clickable link")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
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