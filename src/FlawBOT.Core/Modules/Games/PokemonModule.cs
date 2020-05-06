using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
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
        [Description("Retrieve a Pokémon card")]
        public async Task Pokemon(CommandContext ctx,
            [Description("Name of the Pokémon")] [RemainingText] string query)
        {
            var results = await PokemonService.GetPokemonCardsAsync(query).ConfigureAwait(false);
            if (results.Cards.Count == 0)
                await BotServices.SendEmbedAsync(ctx, Resources.NOT_FOUND_GENERIC, EmbedType.Missing).ConfigureAwait(false);
            else
            {
                foreach (var dex in results.Cards)
                {
                    var card = PokemonService.GetExactPokemon(dex.ID);
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(card.Name + $" (#{card.NationalPokedexNumber})")
                        .AddField("Series", card.Series ?? "Unknown", true)
                        .AddField("Rarity", card.Rarity ?? "Unknown", true)
                        .AddField("HP", card.Hp ?? "Unknown", true)
                        .AddField("Ability", (card.Ability != null) ? card.Ability.Name : "Unknown", true)
                        .WithImageUrl(card.ImageUrlHiRes ?? card.ImageUrl)
                        .WithFooter(!card.Equals(results.Cards.Last()) ? "Type 'next' within 10 seconds for the next Pokémon" : "This is the last found Pokémon on the list.")
                        .WithColor(DiscordColor.Gold);

                    var types = new StringBuilder();
                    foreach (var type in card.Types)
                        types.Append(type);
                    output.AddField("Types", types.ToString() ?? "Unknown", true);

                    var weaknesses = new StringBuilder();
                    foreach (var weakness in card.Weaknesses)
                        weaknesses.Append(weakness.Type);
                    output.AddField("Weaknesses", weaknesses.ToString() ?? "Unknown", true);
                    await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);

                    if (results.Cards.Count == 1) continue;
                    var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                    if (interactivity.Result is null) break;
                    if (!card.Equals(results.Cards.Last()))
                        await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                }
            }
        }

        #endregion COMMAND_POKEMON
    }
}