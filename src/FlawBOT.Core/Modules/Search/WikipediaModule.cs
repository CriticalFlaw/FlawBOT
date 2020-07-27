using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class WikipediaModule : BaseCommandModule
    {
        #region COMMAND_WIKIPEDIA

        [Command("wiki")]
        [Aliases("wikipedia")]
        [Description("Search Wikipedia for a given query")]
        public async Task Wikipedia(CommandContext ctx,
            [Description("Query to search on Wikipedia")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = WikipediaService.GetWikipediaDataAsync(query).Result;
            if (results.Missing)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_WIKIPEDIA, EmbedType.Missing)
                    .ConfigureAwait(false);
            else
                await ctx.Channel.SendMessageAsync(results.FullUrl).ConfigureAwait(false);
        }

        #endregion COMMAND_WIKIPEDIA
    }
}