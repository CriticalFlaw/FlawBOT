using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Services.Search;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class SimpsonsModule : BaseCommandModule
    {
        private readonly string simpsons_site = "frinkiac";
        private readonly string futurama_site = "morbotron";

        #region COMMAND_SIMPSONS

        [Command("simpsons")]
        [Aliases("doh")]
        [Description("Get a random Simpsons screenshot and episode")]
        public async Task Simpsons(CommandContext ctx)
        {
            var data = await SimpsonsService.GetSimpsonsDataAsync(simpsons_site);
            await ctx.RespondAsync(embed: data.Build());
        }

        #endregion COMMAND_SIMPSONS

        #region COMMAND_SIMPSONS_GIF

        [Command("simpsonsgif")]
        [Description("Get a random Simpsons gif")]
        public async Task SimpsonsGIF(CommandContext ctx, [RemainingText] string input)
        {
            var gif = await SimpsonsService.GetSimpsonsGifAsync(simpsons_site);
            if (string.IsNullOrWhiteSpace(input))
                await ctx.RespondAsync(gif);
            else // Include episode information if any kind of parameter is inputted
            {
                var data = await SimpsonsService.GetSimpsonsDataAsync(simpsons_site);
                data.WithFooter("Note: First time gifs take a few minutes to properly generate");
                await ctx.RespondAsync(gif, embed: data.Build());
            }
        }

        #endregion COMMAND_SIMPSONS_GIF

        #region COMMAND_FUTURAMA

        [Command("futurama")]
        [Aliases("bite")]
        [Description("Get a random Futurama screenshot and episode")]
        public async Task Futurama(CommandContext ctx)
        {
            var data = await SimpsonsService.GetSimpsonsDataAsync(futurama_site);
            data.WithColor(DiscordColor.DarkBlue);
            await ctx.RespondAsync(embed: data.Build());
        }

        #endregion COMMAND_FUTURAMA

        #region COMMAND_FUTURAMA_GIF

        [Command("futuramagif")]
        [Description("Get a random Futurama gif")]
        public async Task FuturamaGIF(CommandContext ctx, [RemainingText] string input)
        {
            var gif = await SimpsonsService.GetSimpsonsGifAsync(futurama_site);
            if (string.IsNullOrWhiteSpace(input))
                await ctx.RespondAsync(gif);
            else // Include episode information if any kind of parameter is inputted
            {
                var data = await SimpsonsService.GetSimpsonsDataAsync(futurama_site);
                data.WithFooter("Note: First time gifs take a few minutes to properly generate");
                data.WithColor(DiscordColor.DarkBlue);
                await ctx.RespondAsync(gif, embed: data.Build());
            }
        }

        #endregion COMMAND_FUTURAMA_GIF
    }
}