using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class NewsModule : ApplicationCommandModule
    {
        #region COMMAND_NEWS

        [SlashCommand("news", "Retrieve the news articles on a topic from NewsAPI.org.")]
        public async Task News(InteractionContext ctx, [Option("search", "Article topic to find on NewsAPI.org.")] string search)
        {
            var results = await NewsService.GetNewsDataAsync(Program.Settings.Tokens.NewsToken, search).ConfigureAwait(false);
            if (results.Status != "ok")
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            while (results.Articles.Count > 0)
            {
                var output = new DiscordEmbedBuilder()
                    .WithFooter("Type 'next' within 10 seconds for the next five articles.")
                    .WithColor(new DiscordColor("#253B80"));

                foreach (var result in results.Articles.Take(5))
                {
                    output.AddField(result.PublishDate.ToString(CultureInfo.InvariantCulture), $"[{result.Title}]({result.Url})");
                    results.Articles.Remove(result);
                }
                await ctx.CreateResponseAsync("Latest Google News articles from News API", output.Build()).ConfigureAwait(false);

                if (results.Articles.Count == 5) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_NEWS
    }
}