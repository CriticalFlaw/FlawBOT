using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Nintendo;
using FlawBOT.Properties;
using Newtonsoft.Json;
using PokemonTcgSdk.Standard.Features.FilterBuilder.Pokemon;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class NintendoService : HttpHandler
    {
        /// <summary>
        /// Call the Nintendo Amiibo API for an Amiibo figurine.
        /// </summary>
        public static async Task<DiscordEmbed> GetAmiiboInfoAsync(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                query = string.Format(Resources.URL_Amiibo, query.ToLowerInvariant());
                var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<NintendoData>(response);
                var results = result.Amiibo[random.Next(result.Amiibo.Count)];

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
                return output.Build();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Call the Pokémon TCG API for a set of cards.
        /// </summary>
        public static async Task<DiscordEmbed> GetPokemonCardAsync(string token, string query = "")
        {
            try
            {
                var result = await new PokemonApiClient(token).GetApiResourceAsync<PokemonCard>(PokemonFilterBuilder.CreatePokemonFilter().AddName(query));
                var results = result.Results[random.Next(result.Results.Count)];

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
                return output.Build();
            }
            catch
            {
                return null;
            }
        }
    }
}