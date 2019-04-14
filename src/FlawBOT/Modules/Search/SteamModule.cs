using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models;
using FlawBOT.Services;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserStatus = Steam.Models.SteamCommunity.UserStatus;

namespace FlawBOT.Modules.Search
{
    [Group("steam")]
    [Description("Commands finding Steam games and users")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class SteamModule : BaseCommandModule
    {
        #region COMMAND_GAME

        [Command("game")]
        [Description("Retrieve Steam game information")]
        public async Task SteamGame(CommandContext ctx,
            [Description("Game to find on Steam")] [RemainingText] string query = "Team Fortress 2")
        {
            var game = SharedData.SteamAppList.FirstOrDefault(n => n.Value.ToUpperInvariant() == query.ToUpperInvariant()).Key;
            var check = false;
            while (check == false)
                try
                {
                    var random = new Random();
                    var store = new SteamStore();
                    var appId = (game >= 0) ? game : SharedData.SteamAppList.Keys.ToArray()[random.Next(0, SharedData.SteamAppList.Keys.Count - 1)];
                    var app = await store.GetStoreAppDetailsAsync(appId);
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(app.Name)
                        .WithThumbnailUrl(app.HeaderImage)
                        .WithUrl("http://store.steampowered.com/app/" + app.SteamAppId.ToString())
                        .WithFooter("App ID: " + app.SteamAppId.ToString())
                        .WithColor(new DiscordColor("#1B2838"));
                    if (!string.IsNullOrWhiteSpace(app.DetailedDescription))
                        output.WithDescription(Regex.Replace(app.DetailedDescription.Length <= 500 ? app.DetailedDescription : app.DetailedDescription.Substring(0, 500) + "...", "<[^>]*>", ""));
                    if (!string.IsNullOrWhiteSpace(app.Developers[0]))
                        output.AddField("Developers", app.Developers[0], true);
                    if (!string.IsNullOrWhiteSpace(app.Publishers[0]))
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

        #endregion COMMAND_GAME

        #region COMMAND_USER

        [Command("user")]
        [Description("Retrieve Steam user information")]
        public async Task SteamUser(CommandContext ctx,
            [Description("User to find on Steam")] [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                await BotServices.SendEmbedAsync(ctx, "SteamID or Community URL are required! Try **.steam user criticalflaw**", EmbedType.Warning);
            else
            {
                var steam = new SteamUser(SharedData.Tokens.SteamToken);
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
                        var output = new DiscordEmbedBuilder().WithTitle(summary.Data.Nickname);
                        if (summary.Data.ProfileVisibility == ProfileVisibility.Public)
                        {
                            output.WithThumbnailUrl(profile.AvatarFull.ToString());
                            output.WithColor(new DiscordColor("#1B2838"));
                            output.WithUrl("http://steamcommunity.com/id/" + profile.SteamID);
                            output.WithFooter("Steam ID: " + profile.SteamID);
                            output.AddField("Member since", summary.Data.AccountCreatedDate.ToUniversalTime().ToString(CultureInfo.CurrentCulture), true);
                            if (!string.IsNullOrWhiteSpace(profile.Summary))
                                output.WithDescription(Regex.Replace(profile.Summary, "<[^>]*>", ""));
                            if (summary.Data.UserStatus != UserStatus.Offline)
                                output.AddField("Status", summary.Data.UserStatus.ToString(), true);
                            else
                                output.AddField("Last seen", summary.Data.LastLoggedOffDate.ToUniversalTime().ToString(CultureInfo.CurrentCulture), true);
                            output.AddField("VAC Banned?", profile.IsVacBanned ? "YES" : "NO", true);
                            output.AddField("Trade Bans?", profile.TradeBanState, true);
                            if (profile.InGameInfo != null)
                            {
                                output.AddField("In-Game", $"[{profile.InGameInfo.GameName}]({profile.InGameInfo.GameLink})", true);
                                output.AddField("Game Server IP", profile.InGameServerIP, true);
                                output.WithImageUrl(profile.InGameInfo.GameLogoSmall);
                            }
                        }
                        else
                            output.Description = "This profile is private...";
                        await ctx.RespondAsync(embed: output.Build());
                    }
                    else
                        await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
                }
            }
        }

        #endregion COMMAND_USER
    }
}