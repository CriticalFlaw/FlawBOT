using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Models;
using FlawBOT.Services;
using FlawBOT.Services.Search;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
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
            var results = await DictionaryService.GetDictionaryForTermAsync(query);
            if (results.result_type == "no_results")
                await BotServices.SendEmbedAsync(ctx, "No results found!", EmbedType.Missing);
            else
            {
                foreach (var definition in results.list)
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle("Urban Dictionary definition for " + Formatter.Bold(query) + (!string.IsNullOrWhiteSpace(definition.author) ? $" by {definition.author}" : ""))
                        .WithDescription(definition.definition.Length < 500 ? definition.definition : definition.definition.Take(500) + "...")
                        .WithUrl(definition.permalink)
                        .WithFooter("Type next in the next 10 seconds the next definition")
                        .WithColor(new DiscordColor("#1F2439"));
                    if (!string.IsNullOrWhiteSpace(definition.example))
                        output.AddField("Example", definition.example);
                    output.AddField(":thumbsup:", definition.thumbs_up.ToString(), true);
                    output.AddField(":thumbsdown:", definition.thumbs_down.ToString(), true);
                    var message = await ctx.RespondAsync(embed: output.Build());

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
                    if (interactivity.Result == null) break;
                    await BotServices.RemoveMessage(interactivity.Result);
                    await BotServices.RemoveMessage(message);
                }
            }
        }

        #endregion COMMAND_DICTIONARY
    }
}