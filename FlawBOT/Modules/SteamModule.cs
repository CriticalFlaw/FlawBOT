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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserStatus = Steam.Models.SteamCommunity.UserStatus;

namespace FlawBOT.Modules
{
    public class SteamModule : BaseCommandModule
    {
        [Command("steamgame")]
        [Aliases("sg")]
        [Description("Retrieve information on specific Steam game")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SteamGame(CommandContext ctx)
        {
            var check = false;
            while (check == false)
                try
                {
                    var rnd = new Random();
                    var store = new SteamStore();
                    var app = await store.GetStoreAppDetailsAsync(GlobalVariables.SteamAppList.Keys.ToArray()[rnd.Next(0, GlobalVariables.SteamAppList.Keys.Count - 1)]);
                    await ctx.TriggerTypingAsync();
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(app.Name)
                        .WithThumbnailUrl(app.HeaderImage)
                        .WithUrl($"http://store.steampowered.com/app/{app.SteamAppId}")
                        .WithFooter($"App ID: {app.SteamAppId}")
                        .WithColor(DiscordColor.MidnightBlue);
                    if (!string.IsNullOrWhiteSpace(app.DetailedDescription))
                        output.WithDescription(Regex.Replace(app.DetailedDescription.Length <= 500 ? app.DetailedDescription : $"{app.DetailedDescription.Substring(0, 500)}...", "<[^>]*>", ""));
                    if (app.Developers.Length > 0 && !string.IsNullOrWhiteSpace(app.Developers[0]))
                        output.AddField("Developers", app.Developers[0], true);
                    if (app.Publishers.Length > 0 && !string.IsNullOrWhiteSpace(app.Publishers[0]))
                        output.AddField("Publisher", app.Publishers[0], true);
                    if (!string.IsNullOrWhiteSpace(app.ReleaseDate.Date))
                        output.AddField("Release Date", app.ReleaseDate.Date, true);
                    if (app.Metacritic != null)
                        output.AddField("Metacritic", app.Metacritic.Score.ToString(), true);
                    await ctx.RespondAsync(embed: output.Build());
                    check = true;
                }
                catch
                {
                    check = false;
                }
        }

        [Command("steamuser")]
        [Aliases("su")]
        [Description("Retrieve Steam user information")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SteamUser(CommandContext ctx, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: SteamID or Community URL are required! Try **.su criticalflaw**");
            else
            {
                var service = new BotServices();
                var steam = new SteamUser(service.GetAPIToken("steam"));
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
                        await ctx.TriggerTypingAsync();
                        var output = new DiscordEmbedBuilder()
                            .WithTitle(summary.Data.Nickname);
                        if (summary.Data.ProfileVisibility == ProfileVisibility.Public)
                        {
                            output.WithThumbnailUrl(profile.AvatarFull.ToString());
                            output.WithColor(DiscordColor.MidnightBlue);
                            output.WithUrl($"http://steamcommunity.com/id/{profile.SteamID}/");
                            output.WithFooter($"Steam ID: {profile.SteamID}");
                            output.AddField("Member since",
                                summary.Data.AccountCreatedDate.ToUniversalTime().ToString(CultureInfo.CurrentCulture), true);
                            if (!string.IsNullOrWhiteSpace(profile.Summary))
                                output.WithDescription(Regex.Replace(profile.Summary, "<[^>]*>", ""));
                            if (summary.Data.UserStatus != UserStatus.Offline)
                                output.AddField("Status:", summary.Data.UserStatus.ToString(), true);
                            else
                                output.AddField("Last seen:", summary.Data.LastLoggedOffDate.ToUniversalTime().ToString(CultureInfo.CurrentCulture), true);
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
                        await ctx.RespondAsync(embed: output.Build());
                    }
                    else
                        await BotServices.SendErrorEmbedAsync(ctx, ":mag: No results found!");
                }
            }
        }

        [Command("steamlink")]
        [Aliases("sl")]
        [Description("Format TF2 connection information into a clickable link")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SteamServerLink(CommandContext ctx, [RemainingText] string link)
        {
            await ctx.TriggerTypingAsync();
            var regexes = new Regex(@"connect\s*(?'ip'\S+)\s*;\s*password\s*(?'pw'\S+);?", RegexOptions.Compiled);
            var match = regexes.Match(link);
            if (match.Success)
                await ctx.RespondAsync(string.Format($"steam://connect/{match.Groups["ip"].Value}/{match.Groups["pw"].Value}"));
            else
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: Invalid connection info, follow the format: **connect 123.345.56.789:00000; password hello**");
        }

        [Command("tf2")]
        [Description("Retrieve a Team Fortress item from schema")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task TF2Item(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var rnd = new Random();
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            var service = new BotServices();
            var schema = new EconItems(service.GetAPIToken("steam"), EconItemsAppId.TeamFortress2);
            var items = await schema.GetSchemaForTF2Async();
            var index = rnd.Next(0, items.Data.Items.Count);
            var wikiName = items.Data.Items[index].ItemName.Replace(' ', '_');
            var output = new DiscordEmbedBuilder()
                .WithTitle(items.Data.Items[index].ItemName)
                .WithThumbnailUrl(items.Data.Items[index].ImageUrl)
                .WithUrl($"https://wiki.teamfortress.com/wiki/{wikiName}")
                .WithColor(DiscordColor.Orange);
            if (items.Data.Items[index].ItemDescription != null)
                output.WithDescription(items.Data.Items[index].ItemDescription);
            if (items.Data.Items[index].ItemSlot != null)
                output.AddField("Item Slot:", textInfo.ToTitleCase(items.Data.Items[index].ItemSlot), true);
            var classes = items.Data.Items[index].UsedByClasses.Aggregate<string, string>(null, (current, userClass) => current + $" {userClass} ");
            if (classes != null)
                output.AddField("Used by:", textInfo.ToTitleCase(classes), true);
            if (items.Data.Items[index].ModelPlayer != null)
                await ctx.RespondAsync(embed: output.Build());
        }

        [Command("tf2wiki")]
        [Description("Get a page from the Team Fortress 2 wiki")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task TF2Wiki(CommandContext ctx, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: TF2Wiki search query is a required!");
            else
            {
                await ctx.TriggerTypingAsync();
                var http = new HttpClient();
                var textInfo = new CultureInfo("en-US", false).TextInfo;
                var search = new StringBuilder();
                foreach (var term in query.Split(' '))
                    if (term.Length <= 2)
                        search.Append(term).Append(" ");
                    else
                        search.Append(textInfo.ToTitleCase(term)).Append(" ");
                if (search != null)
                {
                    var title = Uri.EscapeDataString(search.Replace(' ', '_').ToString().Trim());
                    var result = await http.GetStringAsync($"https://wiki.teamfortress.com/w/api.php?action=query&format=json&prop=info&redirects=1&formatversion=2&inprop=url&titles={title}");
                    var data = JsonConvert.DeserializeObject<WikipediaService>(result);
                    if (data.Query.Pages[0].Missing)
                        await BotServices.SendErrorEmbedAsync(ctx, ":mag: TF2Wiki page not found!");
                    else
                        await ctx.Channel.SendMessageAsync(data.Query.Pages[0].FullUrl, embed: null).ConfigureAwait(false);
                }
            }
        }
    }
}