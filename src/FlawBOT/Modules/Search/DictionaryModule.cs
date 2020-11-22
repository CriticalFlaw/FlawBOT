using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class DictionaryModule : BaseCommandModule
    {
        #region COMMAND_DICTIONARY

        [Command("dictionary"), Aliases("define", "def", "dic")]
        [Description("Retrieve an Urban Dictionary definition of a word or phrase")]
        public async Task UrbanDictionary(CommandContext ctx,
            [Description("Query to pass to Urban Dictionary"), RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var results = await DictionaryService.GetDictionaryDefinitionAsync(query).ConfigureAwait(false);
            if (results.ResultType == "no_results" || results.List.Count == 0)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            foreach (var definition in results.List)
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Urban Dictionary definition for " + Formatter.Bold(query))
                    .WithDescription(!string.IsNullOrWhiteSpace(definition.Author)
                        ? "Submitted by: " + definition.Author
                        : string.Empty)
                    .AddField("Definition", definition.Definition.Length < 500
                        ? definition.Definition
                        : definition.Definition.Take(500) + "...")
                    .AddField("Example", definition.Example ?? "None")
                    .AddField(":thumbsup:", definition.ThumbsUp.ToString(), true)
                    .AddField(":thumbsdown:", definition.ThumbsDown.ToString(), true)
                    .WithUrl(definition.Permalink)
                    .WithFooter(!definition.Equals(results.List.Last())
                        ? "Type 'next' within 10 seconds for the next definition"
                        : "This is the last found definition on the list.")
                    .WithColor(new DiscordColor("#1F2439"));
                var message = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                if (results.List.Count == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                if (!definition.Equals(results.List.Last()))
                    await BotServices.RemoveMessage(message).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_DICTIONARY
    }
}