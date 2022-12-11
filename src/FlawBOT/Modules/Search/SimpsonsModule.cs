using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Properties;
using FlawBOT.Services.Lookup;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
{
    public class SimpsonsModule : ApplicationCommandModule
    {
        #region COMMAND_SIMPSONS

        [SlashCommand("simpsons", "Retrieve a random Simpsons screenshot and episode.")]
        public async Task Simpsons(InteractionContext ctx)
        {
            var results = await SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.Frinkiac).ConfigureAwait(false);
            await ctx.CreateResponseAsync(results.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_SIMPSONS

        #region COMMAND_SIMPSONS_GIF

        [SlashCommand("frinkiac", "Retrieve a random Simpsons gif.")]
        public async Task SimpsonsGif(InteractionContext ctx, [Option("input", "Inputting anything will add episode information.")] string input)
        {
            var output = await SimpsonsService.GetSimpsonsGifAsync(SimpsonsService.SiteRoot.Frinkiac).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(input))
            {
                await ctx.CreateResponseAsync(output).ConfigureAwait(false);
                return;
            }

            // Include episode information if any kind of parameter is inputted
            var results = await SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.Frinkiac).ConfigureAwait(false);
            results.WithFooter(Resources.INFO_GIF_LOADING);
            await ctx.CreateResponseAsync(output, results.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_SIMPSONS_GIF

        #region COMMAND_FUTURAMA

        [SlashCommand("futurama", "Retrieve a random Futurama screenshot and episode.")]
        public async Task Futurama(InteractionContext ctx)
        {
            var results = await SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.Morbotron).ConfigureAwait(false);
            results.WithColor(new DiscordColor("#69E398"));
            await ctx.CreateResponseAsync(results.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_FUTURAMA

        #region COMMAND_FUTURAMA_GIF

        [SlashCommand("morbotron", "Retrieve a random Futurama gif.")]
        public async Task FuturamaGif(InteractionContext ctx, [Option("input", "Inputting anything will add episode information.")] string input)
        {
            var output = await SimpsonsService.GetSimpsonsGifAsync(SimpsonsService.SiteRoot.Morbotron).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(input))
            {
                await ctx.CreateResponseAsync(output).ConfigureAwait(false);
                return;
            }

            // Include episode information if any kind of parameter is inputted
            var results = await SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.Morbotron).ConfigureAwait(false);
            results.WithFooter(Resources.INFO_GIF_LOADING);
            results.WithColor(new DiscordColor("#69E398"));
            await ctx.CreateResponseAsync(output, results.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_FUTURAMA_GIF

        #region COMMAND_RICK&MORTY

        [SlashCommand("rick", "Retrieve a random Rick and Morty screenshot and episode.")]
        public async Task Morty(InteractionContext ctx)
        {
            var results = await SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.MasterOfAllScience).ConfigureAwait(false);
            results.WithColor(new DiscordColor("#ABD5EC"));
            await ctx.CreateResponseAsync(results.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_RICK&MORTY

        #region COMMAND_RICK&MORTY_GIF

        [SlashCommand("morty", "Retrieve a random Rick and Morty gif.")]
        public async Task MortyGif(InteractionContext ctx, [Option("input", "Inputting anything will add episode information.")] string input)
        {
            var output = await SimpsonsService.GetSimpsonsGifAsync(SimpsonsService.SiteRoot.MasterOfAllScience).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(input))
            {
                await ctx.CreateResponseAsync(output).ConfigureAwait(false);
                return;
            }

            // Include episode information if any kind of parameter is inputted
            var results = await SimpsonsService.GetSimpsonsDataAsync(SimpsonsService.SiteRoot.MasterOfAllScience).ConfigureAwait(false);
            results.WithFooter(Resources.INFO_GIF_LOADING);
            results.WithColor(new DiscordColor("#ABD5EC"));
            await ctx.CreateResponseAsync(output, results.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_RICK&MORTY_GIF
    }
}