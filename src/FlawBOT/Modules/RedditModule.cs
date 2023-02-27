using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class RedditModule : ApplicationCommandModule
    {
        [SlashCommand("hot", "Get hottest posts from a given subreddit.")]
        public Task HotPost(InteractionContext ctx, [Option("query", "Subreddit")] string query)
        {
            return RedditPost(ctx, query, RedditCategory.Hot);
        }

        [SlashCommand("new", "Get newest posts from a given subreddit.")]
        public Task NewPost(InteractionContext ctx, [Option("query", "Subreddit")] string query)
        {
            return RedditPost(ctx, query, RedditCategory.New);
        }

        [SlashCommand("top", "Get top posts from a given subreddit.")]
        public Task TopPost(InteractionContext ctx, [Option("query", "Subreddit")] string query)
        {
            return RedditPost(ctx, query, RedditCategory.Top);
        }

        private static async Task RedditPost(InteractionContext ctx, string query, RedditCategory category)
        {
            var results = RedditService.GetResults(query, category);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            // TODO: Add pagination when supported for slash commands.
            var output = new DiscordEmbedBuilder()
                .WithFooter($"Search results for {query} on Reddit")
                .WithColor(new DiscordColor("#FF4500"));

            foreach (var result in results)
                output.AddField(result.Authors.FirstOrDefault()?.Name, $"[{(result.Title.Text.Length < 500 ? result.Title.Text : result.Title.Text.Take(500) + "...")}]({result.Links.First().Uri})");

            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }
    }
}