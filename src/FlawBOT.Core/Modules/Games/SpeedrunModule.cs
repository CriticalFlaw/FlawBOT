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
            var results = SpeedrunService.GetSpeedrunGameAsync(query).Result.Data.FirstOrDefault();
            if (results is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.Names.International)
                    .AddField("Release Date", results.ReleaseDate.ToString() ?? "Unknown", true)
                    .AddField("Emulators Allowed?", (results.RuleSet.EmulatorsAllowed) ? "Yes" : "No", true)
                    .AddField("Developers", (results.Developers.Count > 0) ? SpeedrunService.GetSpeedrunExtraAsync(results.Developers.Take(3).ToList(), SpeedrunExtras.Developers).Result : "Unknown", true)
                    .AddField("Publishers", (results.Publishers.Count > 0) ? SpeedrunService.GetSpeedrunExtraAsync(results.Publishers.Take(3).ToList(), SpeedrunExtras.Publishers).Result : "Unknown", true)
                    .AddField("Genres", (results.Genres.Count > 0) ? SpeedrunService.GetSpeedrunExtraAsync(results.Genres.Take(3).ToList(), SpeedrunExtras.Genres).Result : "Unknown", true)
                    .AddField("Platforms", (results.Platforms.Count > 0) ? SpeedrunService.GetSpeedrunExtraAsync(results.Platforms.Take(3).ToList(), SpeedrunExtras.Platforms).Result : "Unknown", true)
                    .WithFooter($"ID: {results.ID} - Abbreviation: {results.Abbreviation}")
                    .WithImageUrl(results.Assets.CoverLarge.URL ?? results.Assets.Icon.URL)
                    .WithUrl(results.WebLink)
                    .WithColor(new DiscordColor("#0F7A4D"));
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_SPEEDRUN
    }
}