using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System.Linq;
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
        [Description("Get hottest posts for a subreddit.")]
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
            var results = RedditService.GetResults(query, category);
            if (results is null || results.Count == 0)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);

            while (results.Count > 0)
            {
                var output = new DiscordEmbedBuilder()
                    .WithFooter("Type 'next' within 10 seconds for the next five posts.")
                    .WithColor(new DiscordColor("#FF4500"));

                foreach (var result in results.Take(5))
                {
                    output.AddField(result.Authors[0].Name, $"[{(result.Title.Text.Length < 500 ? result.Title.Text : result.Title.Text.Take(500) + "...")}]({result.Links.First().Uri})");
                    results.Remove(result);
                }
                var message = await ctx.RespondAsync("Search results for r/" + query, embed: output).ConfigureAwait(false);

                if (results.Count == 5) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                await BotServices.RemoveMessage(message).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_POST
    }
}