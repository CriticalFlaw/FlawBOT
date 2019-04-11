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
    public class DictionaryModule : BaseCommandModule
    {
        #region COMMAND_DICTIONARY

        [Command("dictionary")]
        [Aliases("define", "def", "dic")]
        [Description("Retrieve an Urban Dictionary definition of a word or phrase")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task UrbanDictionary(CommandContext ctx, [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(ctx, query).Result) return;
            var data = await DictionaryService.GetDictionaryForTermAsync(query);
            if (data.result_type == "no_results")
                await BotServices.SendEmbedAsync(ctx, ":mag: No results found!", EmbedType.Warning);
            else
            {
                foreach (var value in data.list)
                {
                    var output = new DiscordEmbedBuilder()
                        .WithTitle($"Urban Dictionary definition for **{query}** by {value.author}")
                        .WithDescription(value.definition.Length < 500 ? value.definition : value.definition.Take(500) + "...")
                        .WithUrl(value.permalink)
                        .WithFooter("Type next for the next definition")
                        .WithColor(DiscordColor.Blurple);
                    if (!string.IsNullOrWhiteSpace(value.example))
                        output.AddField("Example", value.example);
                    output.AddField(":thumbsup:", value.thumbs_up.ToString(), true);
                    output.AddField(":thumbsdown:", value.thumbs_down.ToString(), true);
                    await ctx.RespondAsync(embed: output.Build());

                    var interactivity = await ctx.Client.GetInteractivity()
                        .WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
                    if (interactivity == null) break;
                }
            }
        }

        #endregion COMMAND_DICTIONARY
    }
}