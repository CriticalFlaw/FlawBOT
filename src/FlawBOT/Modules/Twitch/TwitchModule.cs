using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class TwitchModule : ApplicationCommandModule
    {
        #region COMMAND_TWITCH

        [SlashCommand("twitch", "Retrieve Twitch stream information.")]
        public async Task Twitch(InteractionContext ctx, [Option("query", "Channel to find on Twitch.")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
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
                await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);

                if (results.Streams.Length == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_TWITCH
    }
}