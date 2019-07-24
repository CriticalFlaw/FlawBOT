using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class RedditModule : BaseCommandModule
    {
        [Command("reddit")]
        [Description("Retrieve a subreddit from Reddit")]
        public async Task Reddit(CommandContext ctx,
            [Description("Subreddit to find on Reddit")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = RedditService.GetSubredditAsync(query);
            if (results.Id == null)
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(results.Title)
                    .WithDescription(results.Description.Length < 500 ? results.Description : results.Description.Take(500) + "...")
                    .AddField("Subscribers", string.Format("{0:n0}", results.Subscribers), true)
                    .AddField("Created", results.Created.ToString(), true)
                    .WithThumbnailUrl(results.BannerImg)
                    .WithUrl("https://www.reddit.com//" + results.URL)
                    .WithColor(new DiscordColor("#FF4500"));
                await ctx.RespondAsync(embed: output.Build());
            }
        }
    }
}