using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class NintendoModule : BaseCommandModule
    {
        #region COMMAND_AMIIBO

        [Command("amiibo")]
        [Aliases("amib")]
        [Description("Retrieve Amiibo figurine information")]
        public async Task GetAmiibo(CommandContext ctx,
            [Description("Name of the Amiibo figurine")] [RemainingText]
            string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            await ctx.TriggerTypingAsync();
            var results = await AmiiboService.GetAmiiboDataAsync(query).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            foreach (var amiibo in results.Amiibo)
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(amiibo.Name)
                    .AddField("Amiibo Series", amiibo.AmiiboSeries, true)
                    .AddField("Game Series", amiibo.GameSeries, true)
                    .AddField(":flag_us: Release:", amiibo.ReleaseDate.American, true)
                    .AddField(":flag_jp: Release:", amiibo.ReleaseDate.Japanese, true)
                    .AddField(":flag_eu: Release:", amiibo.ReleaseDate.European, true)
                    .AddField(":flag_au: Release:", amiibo.ReleaseDate.Australian, true)
                    .WithImageUrl(amiibo.Image)
                    .WithFooter(!amiibo.Equals(results.Amiibo.Last())
                        ? "Type 'next' within 10 seconds for the next amiibo."
                        : "This is the last found amiibo on the list.")
                    .WithColor(new DiscordColor("#E70009"));
                var message = await ctx.RespondAsync(output.Build()).ConfigureAwait(false);

                if (results.Amiibo.Count == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
                if (!amiibo.Equals(results.Amiibo.Last()))
                    await BotServices.RemoveMessage(message).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_AMIIBO

        #region COMMAND_POKEMON

        [Command("pokemon")]
        [Aliases("poke", "pk")]
        [Description("Retrieve a Pokémon card")]
        public async Task Pokemon(CommandContext ctx,
            [Description("Name of the Pokémon")] [RemainingText]
            string query = "")
        {
            await ctx.TriggerTypingAsync();
            var results = await PokemonService.GetPokemonCardsAsync(query).ConfigureAwait(false);
            if (results.Cards.Count == 0)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing)
                    .ConfigureAwait(false);
                return;
            }

            foreach (var dex in results.Cards)
            {
                var card = PokemonService.GetExactPokemon(dex.Id);
                var output = new DiscordEmbedBuilder()
                    .WithTitle(card.Name + $" (#{card.NationalPokedexNumber})")
                    .AddField("Series", card.Series ?? "Unknown", true)
                    .AddField("Rarity", card.Rarity ?? "Unknown", true)
                    .AddField("HP", card.Hp ?? "Unknown", true)
                    .AddField("Ability", card.Ability != null ? card.Ability.Name : "Unknown", true)
                    .WithImageUrl(card.ImageUrlHiRes ?? card.ImageUrl)
                    .WithFooter(!string.Equals(card.Id, results.Cards.Last().Id)
                        ? "Type 'next' within 10 seconds for the next Pokémon."
                        : "This is the last found Pokémon on the list.")
                    .WithColor(DiscordColor.Gold);

                var types = new StringBuilder();
                if (card.Types != null)
                    foreach (var type in card.Types)
                        types.Append(type);
                output.AddField("Types", types.ToString() ?? "Unknown", true);

                var weaknesses = new StringBuilder();
                foreach (var weakness in card.Weaknesses)
                    weaknesses.Append(weakness.Type);
                output.AddField("Weaknesses", weaknesses.ToString() ?? "Unknown", true);
                await ctx.RespondAsync(output.Build()).ConfigureAwait(false);

                if (results.Cards.Count == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                if (!string.Equals(card.Id, results.Cards.Last().Id))
                    await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_POKEMON
    }
}