using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class SimpsonsModule : BaseCommandModule
    {
        #region COMMAND_SIMPSONS

        [Command("simpsons")]
        [Aliases("caramba")]
        [Description("Retrieve a random Simpsons screenshot and episode")]
        public async Task Simpsons(CommandContext ctx)
        {
            var results = await SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.Frinkiac)
                .ConfigureAwait(false);
            await ctx.RespondAsync(results.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_SIMPSONS

        #region COMMAND_SIMPSONS_GIF

        [Command("simpsonsgif")]
        [Aliases("doh")]
        [Description("Retrieve a random Simpsons gif")]
        public async Task SimpsonsGif(CommandContext ctx,
            [Description("Inputting anything will add episode information")] [RemainingText]
            string input)
        {
            var output = await SimpsonsService.GetSimpsonsGifAsync(SimpsonsService.SiteRoot.Frinkiac)
                .ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(input))
            {
                await ctx.RespondAsync(output).ConfigureAwait(false);
                return;
            }

            // Include episode information if any kind of parameter is inputted
            var results = await SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.Frinkiac)
                .ConfigureAwait(false);
            results.WithFooter(Resources.INFO_GIF_LOADING);
            await ctx.RespondAsync(output, results.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_SIMPSONS_GIF

        #region COMMAND_FUTURAMA

        [Command("futurama")]
        [Aliases("bite")]
        [Description("Retrieve a random Futurama screenshot and episode")]
        public async Task Futurama(CommandContext ctx)
        {
            var results = await SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.Morbotron)
                .ConfigureAwait(false);
            results.WithColor(new DiscordColor("#69E398"));
            await ctx.RespondAsync(results.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_FUTURAMA

        #region COMMAND_FUTURAMA_GIF

        [Command("futuramagif")]
        [Aliases("neat")]
        [Description("Retrieve a random Futurama gif")]
        public async Task FuturamaGif(CommandContext ctx,
            [Description("Inputting anything will add episode information")] [RemainingText]
            string input)
        {
            var output = await SimpsonsService.GetSimpsonsGifAsync(SimpsonsService.SiteRoot.Morbotron)
                .ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(input))
            {
                await ctx.RespondAsync(output).ConfigureAwait(false);
                return;
            }

            // Include episode information if any kind of parameter is inputted
            var results = await SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.Morbotron)
                .ConfigureAwait(false);
            results.WithFooter(Resources.INFO_GIF_LOADING);
            results.WithColor(new DiscordColor("#69E398"));
            await ctx.RespondAsync(output, results.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_FUTURAMA_GIF

        #region COMMAND_RICKMORTY

        [Command("rick")]
        [Aliases("morty")]
        [Description("Retrieve a random Rick and Morty screenshot and episode")]
        public async Task Morty(CommandContext ctx)
        {
            var results = await SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.MasterOfAllScience)
                .ConfigureAwait(false);
            results.WithColor(new DiscordColor("#ABD5EC"));
            await ctx.RespondAsync(results.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_RICKMORTY

        #region COMMAND_RICKMORTY_GIF

        [Command("rickgif")]
        [Aliases("mortygif")]
        [Description("Retrieve a random Rick and Morty gif")]
        public async Task MortyGif(CommandContext ctx,
            [Description("Inputting anything will add episode information")] [RemainingText]
            string input)
        {
            var output = await SimpsonsService.GetSimpsonsGifAsync(SimpsonsService.SiteRoot.MasterOfAllScience)
                .ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(input))
            {
                await ctx.RespondAsync(output).ConfigureAwait(false);
                return;
            }

            // Include episode information if any kind of parameter is inputted
            var results = await SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.MasterOfAllScience)
                .ConfigureAwait(false);
            results.WithFooter(Resources.INFO_GIF_LOADING);
            results.WithColor(new DiscordColor("#ABD5EC"));
            await ctx.RespondAsync(output, results.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_RICKMORTY_GIF
    }
}