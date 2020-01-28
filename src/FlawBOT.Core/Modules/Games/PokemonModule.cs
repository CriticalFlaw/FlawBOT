using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class PokemonModule : BaseCommandModule
    {
        #region COMMAND_POKEMON

        [Command("pokemon")]
        [Aliases("poke", "pk")]
        [Description("Retrieve a Pokemon card")]
        public async Task Pokemon(CommandContext ctx,
            [Description("Name of the pokemon")] [RemainingText] string query)
        {
            var results = await PokemonService.GetPokemonCardsAsync(query).ConfigureAwait(false);
            if (results.Cards.Count == 0)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                foreach (var dex in results.Cards)
                {
                    var card = PokemonService.GetExactPokemonAsync(dex.ID);
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(card.Name)
                        .WithDescription("Pokédex ID: " + card.NationalPokedexNumber.ToString() ?? "Unknown")
                        .AddField("Series", card.Series ?? "Unknown", true)
                        .AddField("Rarity", card.Rarity ?? "Unknown", true)
                        .AddField("HP", card.Hp ?? "Unknown", true)
                        .WithImageUrl(card.ImageUrlHiRes ?? card.ImageUrl)
                        .WithFooter(!card.Equals(results.Cards.Last()) ? "Type 'next' within 10 seconds for the next pokemon" : "This is the last found pokemon on the list.")
                        .WithColor(DiscordColor.Gold);

                    var values = new StringBuilder();
                    foreach (var type in card.Types)
                        values.Append(type);
                    output.AddField("Types", values.ToString() ?? "Unknown", true);
                    foreach (var weakness in card.Weaknesses)
                        values.Append(weakness.Value);
                    output.AddField("Weaknesses", values.ToString() ?? "Unknown", true);
                    await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && string.Equals(m.Content, "next", StringComparison.InvariantCultureIgnoreCase), TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                    if (interactivity.Result is null) break;
                    if (!card.Equals(results.Cards.Last()))
                        await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                }
            }
        }

        #endregion COMMAND_POKEMON
    }
}