using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using Steam.Models.SteamCommunity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserStatus = Steam.Models.SteamCommunity.UserStatus;

namespace FlawBOT.Modules
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
            try
            {
                var app = SteamService.GetSteamAppAsync(query).Result;
                var output = new DiscordEmbedBuilder()
                    .WithTitle(app.Name)
                    .WithDescription((Regex.Replace(app.DetailedDescription.Length <= 500 ? app.DetailedDescription : app.DetailedDescription.Substring(0, 250) + "...", "<[^>]*>", "")) ?? "Unknown")
                    .AddField("Release Date", app.ReleaseDate.Date ?? "Unknown", true)
                    .AddField("Developers", app.Developers[0] ?? "Unknown", true)
                    .AddField("Publisher", app.Publishers[0] ?? "Unknown", true)
                    .AddField("Price", (app.IsFree ? "Free" : (app.PriceOverview.FinalFormatted ?? "Unknown")), true)
                    .AddField("Metacritic", app.Metacritic.Score.ToString() ?? "Unknown", true)
                    .WithThumbnailUrl(app.HeaderImage)
                    .WithUrl("http://store.steampowered.com/app/" + app.SteamAppId.ToString())
                    .WithFooter("App ID: " + app.SteamAppId.ToString())
                    .WithColor(new DiscordColor("#1B2838"));

                var genres = new StringBuilder();
                foreach (var genre in app.Genres.Take(3))
                    genres.Append(genre.Description).Append(!genre.Equals(app.Genres.Last()) ? ", " : string.Empty);
                output.AddField("Genres", genres.ToString() ?? "Unknown", true);

                await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
            catch
            {
                await ctx.RespondAsync("Unable to retrieve game information from the Steam API.").ConfigureAwait(false);
            }
        }

        #endregion COMMAND_GAME

        #region COMMAND_USER

        [Command("user")]
        [Aliases("player")]
        [Description("Retrieve Steam user information")]
        public async Task SteamUser(CommandContext ctx,
            [Description("User to find on Steam")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var profile = SteamService.GetSteamProfileAsync(query).Result;
            var summary = SteamService.GetSteamSummaryAsync(query).Result;
            if (profile is null && summary is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                if (summary.Data.ProfileVisibility != ProfileVisibility.Public)
                    await BotServices.SendEmbedAsync(ctx, "This profile is private...", EmbedType.Warning).ConfigureAwait(false);
                else
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(summary.Data.Nickname)
                        .WithDescription(Regex.Replace(profile.Summary, "<[^>]*>", "") ?? string.Empty)
                        .AddField("Member since", summary.Data.AccountCreatedDate.ToUniversalTime().ToString(CultureInfo.CurrentCulture), true)
                        .WithThumbnailUrl(profile.AvatarFull.ToString() ?? profile.Avatar.ToString())
                        .WithColor(new DiscordColor("#1B2838"))
                        .WithUrl("http://steamcommunity.com/profiles/" + profile.SteamID)
                        .WithFooter("Steam ID: " + profile.SteamID);

                    if (summary.Data.UserStatus != UserStatus.Offline)
                        output.AddField("Status", summary.Data.UserStatus.ToString(), true);
                    else
                        output.AddField("Last seen", summary.Data.LastLoggedOffDate.ToUniversalTime().ToString(CultureInfo.CurrentCulture), true);

                    if (!string.IsNullOrWhiteSpace(profile.Location))
                        output.AddField("Location", profile.Location);

                    if (profile.InGameInfo != null)
                    {
                        output.AddField("In-Game", $"[{profile.InGameInfo.GameName}]({profile.InGameInfo.GameLink})", true);
                        output.AddField("Game Server IP", profile.InGameServerIP, true);
                        output.WithImageUrl(profile.InGameInfo.GameLogoSmall);
                    }
                    await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
                }
            }
        }

        #endregion COMMAND_USER

        #region COMMAND_CONNECT

        [Command("connect")]
        [Aliases("link")]
        [Description("Format a game connection string into a clickable link")]
        public async Task SteamServerLink(CommandContext ctx,
            [Description("Connection string")] [RemainingText] string link)
        {
            var regex = new Regex(@"\s*(?'ip'\S+)\s*", RegexOptions.Compiled).Match(link);
            if (regex.Success)
                await ctx.RespondAsync(string.Format($"steam://connect/{regex.Groups["ip"].Value}/{regex.Groups["pw"].Value}")).ConfigureAwait(false);
            else
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_STEAM_CONNECT_FORMAT, EmbedType.Warning).ConfigureAwait(false);
        }

        #endregion COMMAND_CONNECT
    }
}