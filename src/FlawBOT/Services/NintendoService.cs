using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Nintendo;
using FlawBOT.Properties;
using Newtonsoft.Json;
using PokemonTcgSdk.Standard.Features.FilterBuilder.Pokemon;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class NintendoService : HttpHandler
    {
        /// <summary>
        ///     Call the Nintendo Amiibo API for an Amiibo figurine.
        /// </summary>
        public static async Task<DiscordEmbed> GetAmiiboInfoAsync(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query)) return null;
                var url = string.Format(Resources.URL_Amiibo, Uri.EscapeDataString(query.Trim().ToLowerInvariant()));
                using var response = await Http.GetAsync(url).ConfigureAwait(false);
                // The API answers 404 when nothing matches.
                if (response.StatusCode == HttpStatusCode.NotFound) return null;
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return BuildAmiiboEmbed(JsonConvert.DeserializeObject<NintendoData>(json));
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed BuildAmiiboEmbed(NintendoData result)
        {
            if (result?.Amiibo is null || result.Amiibo.Count == 0) return null;
            var results = result.Amiibo[Random.Next(result.Amiibo.Count)];
            var release = results.ReleaseDate ?? new Release();
            var output = new DiscordEmbedBuilder()
                .WithTitle(results.Name)
                .AddSafeField("Amiibo Series", results.AmiiboSeries, true)
                .AddSafeField("Game Series", results.GameSeries, true)
                .AddSafeField(":flag_us: Release:", release.American, true)
                .AddSafeField(":flag_jp: Release:", release.Japanese, true)
                .AddSafeField(":flag_eu: Release:", release.European, true)
                .AddSafeField(":flag_au: Release:", release.Australian, true)
                .WithColor(new DiscordColor("#E70009"));
            if (!string.IsNullOrWhiteSpace(results.Image)) output.WithImageUrl(results.Image);
            return output.Build();
        }

        /// <summary>
        ///     Call the Pokémon TCG API for a set of cards.
        /// </summary>
        public static async Task<DiscordEmbed> GetPokemonCardAsync(string token, string query = "")
        {
            try
            {
                var client = string.IsNullOrWhiteSpace(token) ? new PokemonApiClient() : new PokemonApiClient(token);
                var filter = PokemonFilterBuilder.CreatePokemonFilter();
                if (!string.IsNullOrWhiteSpace(query)) filter.AddName(query.Trim());
                var result = await client.GetApiResourceAsync<PokemonCard>(25, 1, filter).ConfigureAwait(false);
                if (result?.Results is null || result.Results.Count == 0) return null;

                var card = result.Results[Random.Next(result.Results.Count)];
                var pokedex = card.NationalPokedexNumbers?.Count > 0 ? $" (#{string.Join(", #", card.NationalPokedexNumbers)})" : string.Empty;
                var output = new DiscordEmbedBuilder()
                    .WithTitle(card.Name + pokedex)
                    .AddSafeField("Rarity", card.Rarity, true)
                    .AddSafeField("HP", card.Hp > 0 ? card.Hp.ToString() : null, true)
                    .AddSafeField("Types", card.Types.JoinOrDefault(), true)
                    .AddSafeField("Weaknesses", card.Weaknesses?.Select(x => x.Type).JoinOrDefault(", ", "None"), true)
                    .WithFooter(card.Id)
                    .WithColor(DiscordColor.Gold);
                var image = card.Images?.Large ?? card.Images?.Small;
                if (image is not null) output.WithImageUrl(image);
                return output.Build();
            }
            catch
            {
                return null;
            }
        }
    }
}
