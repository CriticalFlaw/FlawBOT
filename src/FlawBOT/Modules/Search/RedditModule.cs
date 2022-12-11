using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using FlawBOT.Services.Lookup;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
{
    public class RedditModule : ApplicationCommandModule
    {
        #region COMMAND_HOT

        [SlashCommand("hot", "Get hottest posts from a given subreddit.")]
        public Task HotPost(InteractionContext ctx, [Option("query", "Subreddit")] string query)
        {
            return RedditPost(ctx, query, RedditCategory.Hot);
        }

        #endregion COMMAND_HOT

        #region COMMAND_NEW

        [SlashCommand("new", "Get newest posts from a given subreddit.")]
        public Task NewPost(InteractionContext ctx, [Option("query", "Subreddit")] string query)
        {
            return RedditPost(ctx, query, RedditCategory.New);
        }

        #endregion COMMAND_NEW

        #region COMMAND_TOP

        [SlashCommand("top", "Get top posts from a given subreddit.")]
        public Task TopPost(InteractionContext ctx, [Option("query", "Subreddit")] string query)
        {
            return RedditPost(ctx, query, RedditCategory.Top);
        }

        #endregion COMMAND_TOP

        #region COMMAND_POST

        private static async Task RedditPost(InteractionContext ctx, [Option("query", "Subreddit")] string query, [Option("category", "category")] RedditCategory category)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = RedditService.GetResults(query, category);
            if (results is null || results.Count == 0)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            while (results.Count > 0)
            {
                var output = new DiscordEmbedBuilder()
                    .WithFooter("Type 'next' within 10 seconds for the next five posts.")
                    .WithColor(new DiscordColor("#FF4500"));

                foreach (var result in results.Take(5))
                {
                    output.AddField(result.Authors.FirstOrDefault()?.Name, $"[{(result.Title.Text.Length < 500 ? result.Title.Text : result.Title.Text.Take(500) + "...")}]({result.Links.First().Uri})");
                    results.Remove(result);
                }
                await ctx.CreateResponseAsync("Search results for r/" + query + " on Reddit", output).ConfigureAwait(false);

                if (results.Count == 5) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_POST
    }
}