using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Globalization;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class NewsModule : ApplicationCommandModule
    {
        [SlashCommand("news", "Retrieve the news articles on a topic from NewsAPI.org.")]
        public async Task News(InteractionContext ctx, [Option("search", "Article topic to find on NewsAPI.org.")] string query = "")
        {
            var results = await NewsService.GetNewsDataAsync(Program.Settings.Tokens.NewsToken, query).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            // TODO: Add pagination when supported for slash commands.
            var output = new DiscordEmbedBuilder()
                .WithTitle("Latest Google News articles from News API.")
                .WithColor(new DiscordColor("#253B80"));

            foreach (var result in results)
                output.AddField(result.PublishDate.ToString(CultureInfo.InvariantCulture), $"[{result.Title}]({result.Url})");

            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }
    }
}