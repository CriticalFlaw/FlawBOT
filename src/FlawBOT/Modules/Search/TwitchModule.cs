using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using FlawBOT.Services.Lookup;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class TwitchModule : BaseCommandModule
    {
        #region COMMAND_TWITCH

        [Command("twitch")]
        [Aliases("stream")]
        [Description("Retrieve Twitch stream information.")]
        public async Task Twitch(CommandContext ctx, [Description("Channel to find on Twitch.")][RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            await ctx.TriggerTypingAsync();
            var results = await TwitchService.GetTwitchDataAsync(Program.Settings.Tokens.TwitchToken, Program.Settings.Tokens.TwitchAccess, query).ConfigureAwait(false);
            if (results.Streams.Length == 0)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_TWITCH, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            foreach (var streamer in results.Streams)
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(streamer.Title)
                    .WithDescription("[LIVE] Now Playing: " + streamer.GameName)
                    .AddField("Broadcaster", streamer.Type.ToUpperInvariant(), true)
                    .AddField("Viewers", streamer.ViewerCount.ToString(), true)
                    .AddField("Started at", streamer.StartedAt.ToString())
                    .WithImageUrl(streamer.ThumbnailUrl)
                    .WithUrl($"https://www.twitch.tv/{streamer.UserName}")
                    .WithColor(new DiscordColor("#6441A5"));
                var message = await ctx.RespondAsync(output.Build()).ConfigureAwait(false);

                if (results.Streams.Length == 1) continue;
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