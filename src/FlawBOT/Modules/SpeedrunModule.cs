using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Models.Speedrun;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class SpeedrunModule : ApplicationCommandModule
    {
        [SlashCommand("speedrun", "Retrieve a game from Speedrun.com")]
        public async Task Speedrun(InteractionContext ctx, [Option("query", "Game to find on Speedrun.com")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = SpeedrunService.GetSpeedrunGameAsync(query).Result;
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            // TODO: Add pagination when supported for slash commands.
            var link = results.Links.First(x => x.Rel == "categories").Url;
            var categories = SpeedrunService.GetSpeedrunCategoryAsync(link).Result;
            var category = new StringBuilder();
            if (categories != null || categories.Data.Count > 0)
                foreach (var x in categories.Data)
                    category.Append($"[{x.Name}]({x.Weblink}) **|** ");

            var output = new DiscordEmbedBuilder()
                .WithTitle(results.Names.International)
                .AddField("Developers", SpeedrunService.GetSpeedrunExtraAsync(results.Developers, SpeedrunExtras.Developers).Result ?? "Unknown", true)
                .AddField("Publishers", SpeedrunService.GetSpeedrunExtraAsync(results.Publishers, SpeedrunExtras.Publishers).Result ?? "Unknown", true)
                .AddField("Release Date", results.ReleaseDate ?? "Unknown")
                .AddField("Platforms", SpeedrunService.GetSpeedrunExtraAsync(results.Platforms, SpeedrunExtras.Platforms).Result ?? "Unknown")
                .WithFooter($"ID: {results.Id} - Abbreviation: {results.Abbreviation}")
                .WithThumbnail(results.Assets.CoverLarge.Url ?? results.Assets.Icon.Url)
                .AddField("Categories", category.ToString())
                .WithUrl(results.WebLink)
                .WithColor(new DiscordColor("#0F7A4D"));
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }
    }
}