using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class TwitchModule : BaseCommandModule
    {
        #region COMMAND_TWITCH

        [Command("twitch"), Aliases("stream")]
        [Description("Retrieve Twitch stream information")]
        public async Task Twitch(CommandContext ctx,
            [Description("Channel to find on Twitch"), RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await TwitchService.GetTwitchDataAsync(query).ConfigureAwait(false);
            if (results.Total == 0)
            {
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_TWITCH, EmbedType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            foreach (var streamer in results.Streams)
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(streamer.Channel.DisplayName)
                    .WithDescription("[LIVE] Now Playing: " + streamer.Channel.Game)
                    .AddField("Broadcaster", streamer.Channel.BroadcasterType.ToUpperInvariant(), true)
                    .AddField("Viewers", streamer.Viewers.ToString(), true)
                    .AddField("Followers", streamer.Channel.Followers.ToString(), true)
                    .AddField("Status", streamer.Channel.Status)
                    .WithThumbnail(streamer.Channel.Logo)
                    .WithImageUrl(streamer.Preview.Large)
                    .WithUrl(streamer.Channel.Url)
                    .WithFooter(!streamer.Id.Equals(results.Streams.Last().Id)
                        ? "Type 'next' within 10 seconds for the next streamer"
                        : "This is the last found streamer on the list.")
                    .WithColor(new DiscordColor("#6441A5"));
                var message = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                if (results.Total == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                if (!streamer.Id.Equals(results.Streams.Last().Id))
                    await BotServices.RemoveMessage(message).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_TWITCH
    }
}