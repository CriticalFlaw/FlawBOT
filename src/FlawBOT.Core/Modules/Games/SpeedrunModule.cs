using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class SpeedrunModule : BaseCommandModule
    {
        #region COMMAND_SPEEDRUN

        [Command("speedrun")]
        [Description("Search Speedrun.com for a given game")]
        public async Task Speedrun(CommandContext ctx,
            [Description("Game to search on Speedrun.com")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var game = SpeedrunService.GetSpeedrunGameAsync(query).Result.Data.FirstOrDefault();
            if (game is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(game.Names.International)
                    .AddField("Release Date", game.ReleaseDate ?? "Unknown", true)
                    .AddField("Developers", (game.Developers.Count > 0) ? SpeedrunService.GetSpeedrunExtraAsync(game.Developers, SpeedrunExtras.Developers).Result : "Unknown", true)
                    .AddField("Publishers", (game.Publishers.Count > 0) ? SpeedrunService.GetSpeedrunExtraAsync(game.Publishers, SpeedrunExtras.Publishers).Result : "Unknown", true)
                    .AddField("Genres", (game.Genres.Count > 0) ? SpeedrunService.GetSpeedrunExtraAsync(game.Genres, SpeedrunExtras.Genres).Result : "Unknown", true)
                    .AddField("Platforms", (game.Platforms.Count > 0) ? SpeedrunService.GetSpeedrunExtraAsync(game.Platforms, SpeedrunExtras.Platforms).Result : "Unknown", true)
                    .WithFooter($"ID: {game.ID} - Abbreviation: {game.Abbreviation}")
                    .WithThumbnailUrl(game.Assets.CoverLarge.URL ?? game.Assets.Icon.URL)
                    .WithUrl(game.WebLink)
                    .WithColor(new DiscordColor("#0F7A4D"));
                await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_SPEEDRUN
    }
}