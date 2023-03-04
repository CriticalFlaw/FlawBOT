using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class RedditModule : ApplicationCommandModule
    {
        [SlashCommand("reddit_hot", "Get hottest posts from a given subreddit.")]
        public Task RedditHot(InteractionContext ctx, [Option("query", "Subreddit")] string query)
        {
            return RedditPost(ctx, query, RedditCategory.Hot);
        }

        [SlashCommand("reddit_new", "Get newest posts from a given subreddit.")]
        public Task RedditNew(InteractionContext ctx, [Option("query", "Subreddit")] string query)
        {
            return RedditPost(ctx, query, RedditCategory.New);
        }

        [SlashCommand("reddit_top", "Get top posts from a given subreddit.")]
        public Task RedditTop(InteractionContext ctx, [Option("query", "Subreddit")] string query)
        {
            return RedditPost(ctx, query, RedditCategory.Top);
        }

        private static async Task RedditPost(InteractionContext ctx, string query, RedditCategory category)
        {
            var output = RedditService.GetRedditResults(query, category);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}