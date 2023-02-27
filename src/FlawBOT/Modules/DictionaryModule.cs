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
        [SlashCommand("dictionary", "Retrieve an Urban Dictionary definition for a word or phrase.")]
        public async Task UrbanDictionary(InteractionContext ctx, [Option("search", "Word or phrase to find on Urban Dictionary.")] string query = "")
        {
            var results = await DictionaryService.GetDictionaryDefinitionAsync(query).ConfigureAwait(false);
            if (results == null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            // TODO: Add pagination when supported for slash commands.
            var output = new DiscordEmbedBuilder()
                .WithTitle(Formatter.Bold(results.Word))
                .WithDescription(Formatter.Italic(results.Example) ?? string.Empty)
                .AddField("Definition", results.Definition.Length < 500 ? results.Definition : results.Definition.Take(500) + "...")
                .AddField(":thumbsup:", results.ThumbsUp.ToString(), true)
                .AddField(":thumbsdown:", results.ThumbsDown.ToString(), true)
                .WithUrl(results.Permalink)
                .WithFooter(string.IsNullOrWhiteSpace(results.Author) ? string.Empty : "Submitted by: " + results.Author + " on " + results.WrittenOn.Split('T').First())
                .WithColor(new DiscordColor("#1F2439"));
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }
    }
}