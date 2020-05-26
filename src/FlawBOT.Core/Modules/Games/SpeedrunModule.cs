using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class SpeedrunModule : BaseCommandModule
    {
        #region COMMAND_SPEEDRUN

        [Command("speedrun")]
        [Aliases("game", "run")]
        [Description("Retrieve a game from Speedrun.com")]
        public async Task Speedrun(CommandContext ctx,
            [Description("Game to search on Speedrun.com")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = SpeedrunService.GetSpeedrunGameAsync(query).Result.Data;
            if (results is null || results.Count == 0)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing)
                    .ConfigureAwait(false);
            else
                foreach (var game in results)
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(game.Names.International)
                        .AddField("Release Date", game.ReleaseDate ?? "Unknown", true)
                        .AddField("Developers", SpeedrunService.GetSpeedrunExtraAsync(game.Developers, SpeedrunExtras.Developers).Result ?? "Unknown", true)
                        .AddField("Publishers", SpeedrunService.GetSpeedrunExtraAsync(game.Publishers, SpeedrunExtras.Publishers).Result ?? "Unknown", true)
                        .AddField("Platforms", SpeedrunService.GetSpeedrunExtraAsync(game.Platforms, SpeedrunExtras.Platforms).Result ?? "Unknown")
                        .WithFooter($"ID: {game.Id} - Abbreviation: {game.Abbreviation}")
                        .WithThumbnailUrl(game.Assets.CoverLarge.Url ?? game.Assets.Icon.Url)
                        .WithUrl(game.WebLink)
                        .WithColor(new DiscordColor("#0F7A4D"));

                    var link = game.Links.First(x => x.Rel == "categories").Url;
                    var categories = SpeedrunService.GetSpeedrunCategoryAsync(link).Result.Data;
                    var category = new StringBuilder();
                    if (categories != null || categories.Count > 0)
                        foreach (var x in categories)
                            category.Append($"[{x.Name}]({x.Weblink}) **|** ");
                    output.AddField("Categories", category.ToString() ?? "Unknown", true);
                    await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                    if (results.Count == 1) continue;
                    var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                    if (interactivity.Result is null) break;
                    await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                    if (!game.Equals(results.Last()))
                        await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                }
        }

        #endregion COMMAND_SPEEDRUN
    }
}