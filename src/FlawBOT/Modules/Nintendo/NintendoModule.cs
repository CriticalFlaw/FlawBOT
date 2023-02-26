using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Modules.Bot;
using FlawBOT.Properties;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Nintendo
{
    public class NintendoModule : ApplicationCommandModule
    {
        #region COMMAND_AMIIBO

        [SlashCommand("amiibo", "Retrieve information about an Amiibo figurine.")]
        public async Task GetAmiibo(InteractionContext ctx, [Option("search", "Name of the Amiibo figurine.")] string search)
        {
            if (string.IsNullOrWhiteSpace(search)) return;
            var results = await NintendoService.GetAmiiboDataAsync(search).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
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
                await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);

                if (results.Amiibo.Count == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_AMIIBO

        #region COMMAND_POKEMON

        [SlashCommand("pokemon", "Retrieve a Pokémon card.")]
        public async Task Pokemon(InteractionContext ctx, [Option("search", "Name of the Pokémon.")] string search = "")
        {
            var results = await NintendoService.GetPokemonCardsAsync(Program.Settings.Tokens.PokemonToken, search).ConfigureAwait(false);
            if (results.Results.Count == 0)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            foreach (var card in results.Results)
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(card.Name + $" (#{card.NationalPokedexNumbers})")
                    .AddField("Rarity", card.Rarity ?? "Unknown", true)
                    .AddField("HP", card.Hp.ToString() ?? "Unknown", true)
                    .WithImageUrl(card.Images.Large ?? card.Images.Small)
                    .WithFooter(!string.Equals(card.Id, results.Results.Last().Id)
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
                await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);

                if (results.Results.Count == 1) continue;
                var interactivity = await BotServices.GetUserInteractivity(ctx, "next", 10).ConfigureAwait(false);
                if (interactivity.Result is null) break;
                if (!string.Equals(card.Id, results.Results.Last().Id))
                    await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_POKEMON
    }
}