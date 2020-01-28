using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class DictionaryModule : BaseCommandModule
    {
        #region COMMAND_DICTIONARY

        [Command("dictionary")]
        [Aliases("define", "def", "dic")]
        [Description("Retrieve an Urban Dictionary definition of a word or phrase")]
        public async Task UrbanDictionary(CommandContext ctx,
            [Description("Query to pass to Urban Dictionary")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = await DictionaryService.GetDictionaryForTermAsync(query).ConfigureAwait(false);
            if (results.ResultType == "no_results")
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                foreach (var definition in results.List)
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle("Urban Dictionary definition for " + Formatter.Bold(query) + (!string.IsNullOrWhiteSpace(definition.Author) ? " by " + definition.Author : string.Empty))
                        .WithDescription(definition.Definition.Length < 500 ? definition.Definition : definition.Definition.Take(500) + "...")
                        .AddField("Example", definition.Example ?? "None")
                        .AddField(":thumbsup:", definition.ThumbsUp.ToString(), true)
                        .AddField(":thumbsdown:", definition.ThumbsDown.ToString(), true)
                        .WithUrl(definition.Permalink)
                        .WithFooter(!definition.Equals(results.List.Last()) ? "Type 'next' within 10 seconds for the next definition" : "This is the last found definition on the list.")
                        .WithColor(new DiscordColor("#1F2439"));
                    var message = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && string.Equals(m.Content, "next", StringComparison.InvariantCultureIgnoreCase), TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                    if (interactivity.Result is null) break;
                    await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                    if (!definition.Equals(results.List.Last()))
                        await BotServices.RemoveMessage(message).ConfigureAwait(false);
                }
            }
        }

        #endregion COMMAND_DICTIONARY
    }
}