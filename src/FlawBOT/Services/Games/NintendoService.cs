using System.Threading.Tasks;
using FlawBOT.Models;
using FlawBOT.Properties;
using Newtonsoft.Json;
using PokemonTcgSdk.Standard.Features.FilterBuilder.Pokemon;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Base;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;

namespace FlawBOT.Services.Games
{
    public class NintendoService : HttpHandler
    {
        /// <summary>
        /// Call the Nintendo Amiibo API for an Amiibo figurine.
        /// </summary>
        public static async Task<AmiiboData> GetAmiiboDataAsync(string query)
        {
            try
            {
                var results = await Http.GetStringAsync(string.Format(Resources.URL_Amiibo, query.ToLowerInvariant())).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<AmiiboData>(results);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Call the Pokémon TCG API for a set of cards.
        /// </summary>
        public static async Task<ApiResourceList<PokemonCard>> GetPokemonCardsAsync(string token, string query = "")
        {
            return await new PokemonApiClient(token).GetApiResourceAsync<PokemonCard>(PokemonFilterBuilder.CreatePokemonFilter().AddName(query));
        }
    }
}