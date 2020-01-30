using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using FlawBOT.Framework.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Group("reddit")]
    [Description("Commands for findings posts on a subreddit.")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class RedditModule : BaseCommandModule
    {
        #region COMMAND_POST

        [Command("hot")]
        [Description("Get newest hot posts for a subreddit.")]
        public Task HowPost(CommandContext ctx, [Description("Subreddit.")] string query)
            => RedditPost(ctx, query, RedditCategory.Hot);

        [Command("new")]
        [Description("Get newest posts for a subreddit.")]
        [Aliases("newest", "latest")]
        public Task NewPost(CommandContext ctx, [Description("Subreddit.")] string query)
            => RedditPost(ctx, query, RedditCategory.New);

        [Command("top")]
        [Description("Get top posts for a subreddit.")]
        public Task TopPost(CommandContext ctx, [Description("Subreddit.")] string query)
            => RedditPost(ctx, query, RedditCategory.Top);

        private async Task RedditPost(CommandContext ctx, string query, RedditCategory category)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = RedditService.GetEmbeddedResults(query, category);
            await ctx.RespondAsync("Search results for r/" + query, embed: results).ConfigureAwait(false);
        }

        #endregion COMMAND_POST
    }
}