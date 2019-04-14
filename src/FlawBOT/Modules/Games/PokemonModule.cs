using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Models;
using FlawBOT.Services;
using FlawBOT.Services.Games;
using PokemonTcgSdk;
using System;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Games
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class PokemonModule : BaseCommandModule
    {
        #region COMMAND_POKEMON

        [Command("pokemon")]
        [Aliases("poke")]
        [Description("Retrieve a Pokemon card")]
        public async Task Pokemon(CommandContext ctx,
            [Description("Name of the pokemon")] [RemainingText] string query)
        {
            var pokemon = (string.IsNullOrWhiteSpace(query)) ? PokemonService.GetRandomPokemonAsync() : query;
            var results = await PokemonService.GetPokemonCardsAsync(pokemon);
            if (results.cards.Count == 0)
                await BotServices.SendEmbedAsync(ctx, "Pokemon not found!", EmbedType.Missing);
            else
            {
                foreach (var value in results.cards)
                {
                    var card = PokemonTcgSdk.Card.Find<Pokemon>(value.id).Card;
                    var output = new DiscordEmbedBuilder()
                        .WithTitle(card.Name + $" (PokeDex ID: {card.NationalPokedexNumber})")
                        .AddField("Evolves From", card.EvolvesFrom ?? "No-one", true)
                        .AddField("Health Points", card.Hp, true)
                        .AddField("Artist", card.Artist, true)
                        .AddField("Rarity", card.Rarity, true)
                        .AddField("Series", card.Series, true)
                        .WithImageUrl(card.ImageUrl)
                        .WithColor(DiscordColor.Gold)
                        .WithFooter("Type next in the next 10 seconds for the next card");
                    if (card.ImageUrlHiRes != null)
                        output.WithImageUrl(card.ImageUrlHiRes);

                    var types = new StringBuilder();
                    foreach (var type in card.Types)
                        types.Append(type);
                    if (types.Length != 0)
                        output.AddField("Type(s)", types.ToString(), true);
                    await ctx.RespondAsync(embed: output.Build());

                    var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
                    if (interactivity == null) break;
                    await BotServices.RemoveMessage(interactivity.Message);
                }
            }
        }

        #endregion COMMAND_POKEMON
    }
}