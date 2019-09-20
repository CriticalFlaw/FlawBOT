using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

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
            var results = RedditService.GetSubredditPost(query, category);
            if (results is null)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_REDDIT, EmbedType.Missing);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#FF4500"));
                foreach (var result in results)
                    output.AddField(result.Title.Text.Substring(0, 255), result.Links.First().Uri.ToString());
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        #endregion COMMAND_POST
    }
}