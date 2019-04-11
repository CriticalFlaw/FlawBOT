using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Models;
using FlawBOT.Services;
using FlawBOT.Services.Games;
using PokemonTcgSdk;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Games
{
    public class PokemonModule : BaseCommandModule
    {
        #region COMMAND_POKEMON

        [Command("pokemon")]
        [Aliases("poke")]
        [Description("Get a Pokemon card")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Pokemon(CommandContext ctx, [RemainingText] string query)
        {
            var pokemon = (string.IsNullOrWhiteSpace(query)) ? PokemonService.GetRandomPokemonAsync() : query;
            var data = await PokemonService.GetPokemonDataAsync(pokemon);
            if (data.cards.Count == 0)
                await BotServices.SendEmbedAsync(ctx, ":mag: Pokemon not found!", EmbedType.Warning);
            else
            {
                foreach (var value in data.cards)
                {
                    var card = Card.Find<Pokemon>(value.id).Card;
                    var output = new DiscordEmbedBuilder()
                        .WithTitle($"{card.Name} (ID: {card.NationalPokedexNumber})")
                        .AddField("Health Points", card.Hp, true)
                        .AddField("Artist", card.Artist, true)
                        .AddField("Rarity", card.Rarity, true)
                        .AddField("Series", card.Series, true)
                        .WithImageUrl(card.ImageUrl)
                        .WithColor(DiscordColor.Lilac)
                        .WithFooter("Type next for the next definition");
                    if (card.ImageUrlHiRes != null) output.WithImageUrl(card.ImageUrlHiRes);
                    await ctx.RespondAsync(embed: output.Build());

                    var interactivity = await ctx.Client.GetInteractivity()
                        .WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "next", TimeSpan.FromSeconds(10));
                    if (interactivity == null) break;
                }
            }
        }

        #endregion COMMAND_POKEMON
    }
}