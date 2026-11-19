using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class TwitchModule : ApplicationCommandModule
    {
        [SlashCommand("twitch", "Returns Twitch stream information, if online.")]
        public async Task Twitch(InteractionContext ctx, [Option("query", "Channel to find on Twitch.")] string query)
        {
            await ctx.DeferAsync().ConfigureAwait(false);
            var output = await TwitchService.GetTwitchDataAsync(Program.Settings.Tokens.TwitchToken, Program.Settings.Tokens.TwitchAccess, query).ConfigureAwait(false);
            await BotServices.SendDeferredAsync(ctx, output, Resources.NOT_FOUND_TWITCH, ResponseType.Missing).ConfigureAwait(false);
        }
    }
}
