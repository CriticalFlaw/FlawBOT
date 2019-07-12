using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Games
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
            if (results == null)
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.Names.International + $" ({results.Abbreviation})")
                    .AddField("Release Year", (results.ReleaseDate != null) ? results.ReleaseDate.ToString() : "UNKNOWN", true)
                    .AddField("Show Milliseconds", (results.RuleSet.ShowMilliseconds) ? "YES" : "NO", true)
                    .AddField("Requires Verification", (results.RuleSet.RequiresVerification) ? "YES" : "NO", true)
                    .AddField("Requires Video", (results.RuleSet.RequiresVideo) ? "YES" : "NO", true)
                    .AddField("Emulators Allowed", (results.RuleSet.EmulatorsAllowed) ? "YES" : "NO", true)
                    .WithFooter($"ID: {results.ID}")
                    .WithUrl(results.WebLink)
                    .WithColor(DiscordColor.Magenta);

                if (results.Assets.CoverLarge.URL != null)
                    output.WithThumbnailUrl(results.Assets.CoverLarge.URL);

                var platforms = new StringBuilder();
                foreach (var platform in results.Platforms.Take(3))
                    platforms.Append(SpeedrunService.GetGamePlatformAsync(platform).Result.Data.Name + "\n");
                if (platforms.Length > 0)
                    output.AddField("Platforms", platforms.ToString(), true);   // TODO: Convert to text

                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_SPEEDRUN
    }
}