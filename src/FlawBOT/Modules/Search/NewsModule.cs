using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class NewsModule : BaseCommandModule
    {
        #region COMMAND_NEWS

        [Command("news")]
        [Description("Retrieve the latest news articles from NewsAPI.org")]
        public async Task News(CommandContext ctx,
            [Description("Article topic to find on Google News"), RemainingText]
            string query)
        {
            var results = await NewsService.GetNewsDataAsync(query).ConfigureAwait(false);
            if (results.Status != "ok")
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            while (results.Articles.Count > 0)
            {
                var output = new DiscordEmbedBuilder()
                    .WithFooter("Type 'next' within 10 seconds for the next five articles.")
                    .WithColor(new DiscordColor("#253B80"));

                foreach (var result in results.Articles.Take(5))
                {
                    output.AddField(result.PublishDate.ToString(CultureInfo.InvariantCulture),
                        $"[{result.Title}]({result.Url})");
                    results.Articles.Remove(result);
                }

                var message = await ctx
                    .RespondAsync("Latest Google News articles from News API", embed: output.Build())
                    .ConfigureAwait(false);

                if (results.Articles.Count == 5) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                await BotServices.RemoveMessage(message).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_NEWS
    }
}