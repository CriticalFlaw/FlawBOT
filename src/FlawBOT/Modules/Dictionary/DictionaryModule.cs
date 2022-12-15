using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class DictionaryModule : ApplicationCommandModule
    {
        #region COMMAND_DICTIONARY

        [SlashCommand("dictionary", "Retrieve an Urban Dictionary definition for a word or phrase.")]
        public async Task UrbanDictionary(InteractionContext ctx, [Option("search", "Word or phrase to find on Urban Dictionary.")] string search)
        {
            var result = await DictionaryService.GetDictionaryDefinitionAsync(search).ConfigureAwait(false);
            if (result == null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            foreach (var definition in result)
            {
                var author = string.IsNullOrWhiteSpace(definition.Author) ? string.Empty : "Submitted by: " + definition.Author;
                var description = definition.Definition.Length < 500 ? definition.Definition : definition.Definition.Take(500) + "...";
                var footer = definition.Equals(result.Last()) ? Resources.INFO_LIST_LAST_RESULT : Resources.INFO_LIST_NEXT_RESULT;
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Urban Dictionary definition for " + Formatter.Bold(search))
                    .WithDescription(author)
                    .AddField("Definition", description)
                    .AddField("Example", definition.Example ?? "None")
                    .AddField(":thumbsup:", definition.ThumbsUp.ToString(), true)
                    .AddField(":thumbsdown:", definition.ThumbsDown.ToString(), true)
                    .WithUrl(definition.Permalink)
                    .WithFooter(footer)
                    .WithColor(new DiscordColor("#1F2439"));
                await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);

                if (result.Count == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, Resources.APP_INTERACT, 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_DICTIONARY
    }
}