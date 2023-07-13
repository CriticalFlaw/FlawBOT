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
            var output = await TwitchService.GetTwitchDataAsync(Program.Settings.Tokens.TwitchToken, Program.Settings.Tokens.TwitchAccess, query).ConfigureAwait(false);
            if (output is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_TWITCH, ResponseType.Missing).ConfigureAwait(false);
                return;
            }
            await ctx.CreateResponseAsync(output).ConfigureAwait(false);
        }
    }
}