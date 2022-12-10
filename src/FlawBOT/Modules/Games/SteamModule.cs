using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using FlawBOT.Services.Games;
using Steam.Models.SteamCommunity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserStatus = Steam.Models.SteamCommunity.UserStatus;

namespace FlawBOT.Modules.Games
{
    [Group("steam")]
    [Description("Commands finding Steam games and users.")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class SteamModule : BaseCommandModule
    {
        #region COMMAND_CONNECT

        [Command("connect")]
        [Aliases("link")]
        [Description("Format a game connection string into a link.")]
        public async Task SteamLink(CommandContext ctx,
            [Description("Connection string (ex. IP:PORT).")] [RemainingText]
            string link)
        {
            await ctx.TriggerTypingAsync();
            var regex = new Regex(@"\s*(?'ip'\S+)\s*", RegexOptions.Compiled).Match(link);
            if (regex.Success)
                await ctx.RespondAsync(
                        string.Format($"steam://connect/{regex.Groups["ip"].Value}/{regex.Groups["pw"].Value}"))
                    .ConfigureAwait(false);
            else
                await BotServices.SendResponseAsync(ctx, Resources.ERR_INVALID_IP_GAME, ResponseType.Warning)
                    .ConfigureAwait(false);
        }

        #endregion COMMAND_CONNECT

        #region COMMAND_GAME

        [Command("game")]
        [Description("Retrieve information on a Steam game.")]
        public async Task SteamGame(CommandContext ctx,
            [Description("Game to find on Steam.")] [RemainingText]
            string query = "Team Fortress 2")
        {
            try
            {
                await ctx.TriggerTypingAsync();
                var app = SteamService.GetSteamAppAsync(query).Result;
                if (app is null)
                {
                    await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                        .ConfigureAwait(false);
                    return;
                }

                var output = new DiscordEmbedBuilder()
                    .WithTitle(app.Name)
                    .WithDescription(
                        Regex.Replace(
                            app.DetailedDescription.Length <= 500
                                ? app.DetailedDescription
                                : app.DetailedDescription.Substring(0, 250) + "...", "<[^>]*>", "") ?? "Unknown")
                    .AddField("Release Date", app.ReleaseDate.Date ?? "Unknown", true)
                    .AddField("Developers", app.Developers.FirstOrDefault() ?? "Unknown", true)
                    .AddField("Publisher", app.Publishers.FirstOrDefault() ?? "Unknown", true)
                    .AddField("Price", app.IsFree ? "Free" : app.PriceOverview.FinalFormatted ?? "Unknown", true)
                    .AddField("Metacritic", app.Metacritic != null ? app.Metacritic.Score.ToString() : "Unknown", true)
                    .WithThumbnail(app.HeaderImage)
                    .WithUrl(string.Format(Resources.URL_Steam_App, app.SteamAppId))
                    .WithFooter("App ID: " + app.SteamAppId)
                    .WithColor(new DiscordColor("#1B2838"));

                var genres = new StringBuilder();
                foreach (var genre in app.Genres.Take(3))
                    genres.Append(genre.Description).Append(!genre.Equals(app.Genres.Last()) ? ", " : string.Empty);
                output.AddField("Genres", genres.ToString() ?? "Unknown", true);

                await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
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
        [Description("Retrieve information on a Steam user.")]
        public async Task SteamUser(CommandContext ctx,
            [Description("User to find on Steam.")] [RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            await ctx.TriggerTypingAsync();
            var profile = SteamService.GetSteamProfileAsync(Program.Settings.Tokens.SteamToken, query).Result;
            if (profile is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
            }
            else if (profile.Data.ProfileVisibility == ProfileVisibility.Private)
            {
                await BotServices.SendResponseAsync(ctx, "This profile is private...", ResponseType.Warning)
                    .ConfigureAwait(false);
            }
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(profile.Data.Nickname)
                    .AddField("Status", profile.Data.UserStatus.ToString(), true)
                    .AddField("Country",
                        $":flag_{profile.Data.CountryCode.ToLowerInvariant()}: {profile.Data.CountryCode}", true)
                    .WithUrl(profile.Data.ProfileUrl)
                    .WithThumbnail(profile.Data.AvatarFullUrl)
                    .WithColor(new DiscordColor("#1B2838"))
                    .WithFooter("Steam ID: " + profile.Data.SteamId);

                if (profile.Data.UserStatus == UserStatus.Offline)
                    output.AddField("Last seen",
                        profile.Data.LastLoggedOffDate.ToUniversalTime().ToString(CultureInfo.CurrentCulture));

                if (profile.Data.PlayingGameId != null)
                {
                    output.AddField("Now Playing", $"`{profile.Data.PlayingGameName}`", true);
                    output.WithColor(new DiscordColor("#79A14D"));
                }

                await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_USER
    }
}