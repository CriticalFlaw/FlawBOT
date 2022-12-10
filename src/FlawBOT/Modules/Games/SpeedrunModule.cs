using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models;
using FlawBOT.Properties;
using FlawBOT.Services;
using FlawBOT.Services.Games;
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
        [Aliases("game", "run")]
        [Description("Retrieve a game from Speedrun.com")]
        public async Task Speedrun(CommandContext ctx,
            [Description("Game to find on Speedrun.com")] [RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            await ctx.TriggerTypingAsync();
            var results = SpeedrunService.GetSpeedrunGameAsync(query).Result;
            if (results is null || results.Data.Count == 0)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            foreach (var game in results.Data)
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(game.Names.International)
                    .AddField("Developers",
                        SpeedrunService.GetSpeedrunExtraAsync(game.Developers, SpeedrunExtras.Developers).Result ??
                        "Unknown", true)
                    .AddField("Publishers",
                        SpeedrunService.GetSpeedrunExtraAsync(game.Publishers, SpeedrunExtras.Publishers).Result ??
                        "Unknown", true)
                    .AddField("Release Date", game.ReleaseDate ?? "Unknown")
                    .AddField("Platforms",
                        SpeedrunService.GetSpeedrunExtraAsync(game.Platforms, SpeedrunExtras.Platforms).Result ??
                        "Unknown")
                    .WithFooter($"ID: {game.Id} - Abbreviation: {game.Abbreviation}")
                    .WithThumbnail(game.Assets.CoverLarge.Url ?? game.Assets.Icon.Url)
                    .WithUrl(game.WebLink)
                    .WithColor(new DiscordColor("#0F7A4D"));

                var link = game.Links.First(x => x.Rel == "categories").Url;
                var categories = SpeedrunService.GetSpeedrunCategoryAsync(link).Result;
                var category = new StringBuilder();
                if (categories != null || categories.Data.Count > 0)
                    foreach (var x in categories.Data)
                        category.Append($"[{x.Name}]({x.Weblink}) **|** ");
                output.AddField("Categories", category.ToString());
                await ctx.RespondAsync(output.Build()).ConfigureAwait(false);

                if (results.Data.Count == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                if (!game.Equals(results.Data.Last()))
                    await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_SPEEDRUN
    }
}