using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class DictionaryModule : ApplicationCommandModule
    {
        /// <summary>
        /// Returns a definition to a word or phrase from the Urban Dictionary API.
        /// </summary>
        [SlashCommand("dictionary", "Retrieve an Urban Dictionary definition for a word or phrase.")]
        public async Task UrbanDictionary(InteractionContext ctx, [Option("search", "Word or phrase to find on Urban Dictionary.")] string search = "")
        {
            var result = await DictionaryService.GetDictionaryDefinitionAsync(search).ConfigureAwait(false);
            if (result == null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            var output = new DiscordEmbedBuilder()
                .WithTitle(Formatter.Bold(result.Word))
                .WithDescription("Submitted on" + result.WrittenOn)
                .AddField("Definition", result.Definition.Length < 500 ? result.Definition : result.Definition.Take(500) + "...")
                .AddField("Example", result.Example ?? "None")
                .AddField(":thumbsup:", result.ThumbsUp.ToString(), true)
                .AddField(":thumbsdown:", result.ThumbsDown.ToString(), true)
                .WithUrl(result.Permalink)
                .WithFooter(string.IsNullOrWhiteSpace(result.Author) ? string.Empty : "Submitted by: " + result.Author)
                .WithColor(new DiscordColor("#1F2439"));
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }
    }
}