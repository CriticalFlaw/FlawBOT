using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class NintendoModule : ApplicationCommandModule
    {
        [SlashCommand("amiibo", "Retrieve information about an Amiibo figurine.")]
        public async Task GetAmiibo(InteractionContext ctx, [Option("search", "Name of the Amiibo figurine.")] string query)
        {
            var results = await NintendoService.GetAmiiboDataAsync(query).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            // TODO: Add pagination when supported for slash commands.
            var output = new DiscordEmbedBuilder()
                .WithTitle(results.Name)
                .AddField("Amiibo Series", results.AmiiboSeries, true)
                .AddField("Game Series", results.GameSeries, true)
                .AddField(":flag_us: Release:", results.ReleaseDate.American, true)
                .AddField(":flag_jp: Release:", results.ReleaseDate.Japanese, true)
                .AddField(":flag_eu: Release:", results.ReleaseDate.European, true)
                .AddField(":flag_au: Release:", results.ReleaseDate.Australian, true)
                .WithImageUrl(results.Image)
                .WithColor(new DiscordColor("#E70009"));
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        [SlashCommand("pokemon", "Retrieve a Pokémon card.")]
        public async Task Pokemon(InteractionContext ctx, [Option("search", "Name of the Pokémon.")] string query = "")
        {
            var results = await NintendoService.GetPokemonCardAsync(Program.Settings.Tokens.PokemonToken, query).ConfigureAwait(false);
            if (results is null)
            {
                await BotServices.SendResponseAsync(ctx, Resources.NOT_FOUND_COMMON, ResponseType.Missing).ConfigureAwait(false);
                return;
            }

            // TODO: Add pagination when supported for slash commands.
            var types = new StringBuilder();
            if (results.Types != null)
                foreach (var type in results.Types)
                    types.Append(type);

            var weaknesses = new StringBuilder();
            foreach (var weakness in results.Weaknesses)
                weaknesses.Append(weakness.Type);

            var output = new DiscordEmbedBuilder()
                .WithTitle(results.Name + $" (#{results.NationalPokedexNumbers})")
                .AddField("Rarity", results.Rarity ?? "Unknown", true)
                .AddField("HP", results.Hp.ToString() ?? "Unknown", true)
                .AddField("Types", types.ToString() ?? "Unknown", true)
                .AddField("Weaknesses", weaknesses.ToString() ?? "Unknown", true)
                .WithImageUrl(results.Images.Large ?? results.Images.Small)
                .WithFooter(results.Id)
                .WithColor(DiscordColor.Gold);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }
    }
}