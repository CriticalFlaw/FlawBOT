using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class TwitchModule : ApplicationCommandModule
    {
        [SlashCommand("twitch", "Retrieve Twitch stream information.")]
        public async Task Twitch(InteractionContext ctx, [Option("query", "Channel to find on Twitch.")] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await TwitchService.GetTwitchDataAsync(Program.Settings.Tokens.TwitchToken, Program.Settings.Tokens.TwitchAccess, query).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_TWITCH, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            // TODO: Add pagination when supported for slash commands.
            var output = new DiscordEmbedBuilder()
                .WithTitle(results.Title)
                .WithDescription("[LIVE] Now Playing: " + results.GameName)
                .AddField("Broadcaster", results.Type.ToUpperInvariant(), true)
                .AddField("Viewers", results.ViewerCount.ToString(), true)
                .AddField("Started at", results.StartedAt.ToString())
                .WithImageUrl(results.ThumbnailUrl)
                .WithUrl($"https://www.twitch.tv/{results.UserName}")
                .WithColor(new DiscordColor("#6441A5"));
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }
    }
}