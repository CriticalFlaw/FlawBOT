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
        private readonly string rickmorty_site = "masterofallscience";

        #region COMMAND_SIMPSONS

        [Command("simpsons")]
        [Aliases("caramba")]
        [Description("Retrieve a random Simpsons screenshot and episode")]
        public async Task Simpsons(CommandContext ctx)
        {
            var results = await SimpsonsService.GetSimpsonsDataAsync(simpsons_site);
            await ctx.RespondAsync(embed: results.Build());
        }

        #endregion COMMAND_SIMPSONS

        #region COMMAND_SIMPSONS_GIF

        [Command("simpsonsgif")]
        [Aliases("doh")]
        [Description("Retrieve a random Simpsons gif")]
        public async Task SimpsonsGIF(CommandContext ctx,
            [Description("Inputting anything will add episode information")] [RemainingText] string input)
        {
            var output = await SimpsonsService.GetSimpsonsGifAsync(simpsons_site);
            if (string.IsNullOrWhiteSpace(input))
                await ctx.RespondAsync(output);
            else // Include episode information if any kind of parameter is inputted
            {
                var results = await SimpsonsService.GetSimpsonsDataAsync(simpsons_site);
                results.WithFooter("Note: First time gifs take a few minutes to properly generate");
                await ctx.RespondAsync(output, embed: results.Build());
            }
        }

        #endregion COMMAND_SIMPSONS_GIF

        #region COMMAND_FUTURAMA

        [Command("futurama")]
        [Aliases("bite")]
        [Description("Retrieve a random Futurama screenshot and episode")]
        public async Task Futurama(CommandContext ctx)
        {
            var results = await SimpsonsService.GetSimpsonsDataAsync(futurama_site);
            results.WithColor(new DiscordColor("#69E398"));
            await ctx.RespondAsync(embed: results.Build());
        }

        #endregion COMMAND_FUTURAMA

        #region COMMAND_FUTURAMA_GIF

        [Command("futuramagif")]
        [Aliases("neat")]
        [Description("Retrieve a random Futurama gif")]
        public async Task FuturamaGIF(CommandContext ctx,
            [Description("Inputting anything will add episode information")] [RemainingText] string input)
        {
            var output = await SimpsonsService.GetSimpsonsGifAsync(futurama_site);
            if (string.IsNullOrWhiteSpace(input))
                await ctx.RespondAsync(output);
            else // Include episode information if any kind of parameter is inputted
            {
                var results = await SimpsonsService.GetSimpsonsDataAsync(futurama_site);
                results.WithFooter("Note: First time gifs take a few minutes to properly generate");
                results.WithColor(new DiscordColor("#69E398"));
                await ctx.RespondAsync(output, embed: results.Build());
            }
        }

        #endregion COMMAND_FUTURAMA_GIF

        #region COMMAND_RICKMORTY

        [Command("rick")]
        [Aliases("morty")]
        [Description("Retrieve a random Rick and Morty screenshot and episode")]
        public async Task RickMorty(CommandContext ctx)
        {
            var results = await SimpsonsService.GetSimpsonsDataAsync(rickmorty_site);
            results.WithColor(new DiscordColor("#ABD5EC"));
            await ctx.RespondAsync(embed: results.Build());
        }

        #endregion COMMAND_RICKMORTY

        #region COMMAND_RICKMORTY_GIF

        [Command("rickgif")]
        [Aliases("mortygif")]
        [Description("Retrieve a random Rick and Morty gif")]
        public async Task RickMortyGif(CommandContext ctx,
            [Description("Inputting anything will add episode information")] [RemainingText] string input)
        {
            var output = await SimpsonsService.GetSimpsonsGifAsync(futurama_site);
            if (string.IsNullOrWhiteSpace(input))
                await ctx.RespondAsync(output);
            else // Include episode information if any kind of parameter is inputted
            {
                var results = await SimpsonsService.GetSimpsonsDataAsync(rickmorty_site);
                results.WithFooter("Note: First time gifs take a few minutes to properly generate");
                results.WithColor(new DiscordColor("#ABD5EC"));
                await ctx.RespondAsync(output, embed: results.Build());
            }
        }

        #endregion COMMAND_RICKMORTY_GIF
    }
}