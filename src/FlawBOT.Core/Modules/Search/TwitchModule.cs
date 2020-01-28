using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class TwitchModule : BaseCommandModule
    {
        #region COMMAND_TWITCH

        [Command("twitch")]
        [Aliases("stream")]
        [Description("Retrieve Twitch stream information")]
        public async Task Twitch(CommandContext ctx,
            [Description("Channel to find on Twitch")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = await TwitchService.GetTwitchDataAsync(query).ConfigureAwait(false);
            if (results.Stream.Count == 0)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_TWITCH, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                var stream = results.Stream[0];
                var output = new DiscordEmbedBuilder()
                    .WithTitle(stream.UserName + " is " + (stream.Type != "live" ? "offline" : "live on Twitch!"))
                    .WithDescription(stream.Title)
                    .AddField("Start Time:", stream.StartTime, true)
                    .AddField("View Count:", stream.ViewCount.ToString(), true)
                    .WithImageUrl(stream.ThumbnailUrl.Replace("{width}", "500").Replace("{height}", "300"))
                    .WithUrl("https://www.twitch.tv/" + stream.UserName)
                    .WithColor(new DiscordColor("#6441A5"));
                await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_TWITCH
    }
}