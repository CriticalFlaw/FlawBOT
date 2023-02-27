using FlawBOT.Common;
using FlawBOT.Models.Nintendo;
using FlawBOT.Properties;
using Newtonsoft.Json;
using PokemonTcgSdk.Standard.Features.FilterBuilder.Pokemon;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class NintendoService : HttpHandler
    {
        /// <summary>
        /// Call the Nintendo Amiibo API for an Amiibo figurine.
        /// </summary>
        public static async Task<Amiibo> GetAmiiboDataAsync(string query)
        {
            try
            {
                query = string.Format(Resources.URL_Amiibo, query.ToLowerInvariant());
                var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<NintendoData>(response);
                return result.Amiibo[random.Next(result.Amiibo.Count)];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Call the Pokémon TCG API for a set of cards.
        /// </summary>
        public static async Task<PokemonCard> GetPokemonCardAsync(string token, string query = "")
        {
            try
            {
                var result = await new PokemonApiClient(token).GetApiResourceAsync<PokemonCard>(PokemonFilterBuilder.CreatePokemonFilter().AddName(query));
                return result.Results[random.Next(result.Results.Count)];
            }
            catch
            {
                return null;
            }
        }
    }
}