using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlawBOT.Models;
using FlawBOT.Properties;
using Newtonsoft.Json;
using PokemonTcgSdk;
using PokemonTcgSdk.Models;

namespace FlawBOT.Services.Games
{
    public class NintendoService : HttpHandler
    {
        private static List<string> PokemonList { get; set; } = new();

        public static async Task<AmiiboData> GetAmiiboDataAsync(string query)
        {
            try
            {
                var results = await Http.GetStringAsync(string.Format(Resources.URL_Amiibo, query.ToLowerInvariant()))
                    .ConfigureAwait(false);
                return JsonConvert.DeserializeObject<AmiiboData>(results);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Call the Pokémon TCG API for a set of cards.
        /// </summary>
        public static async Task<Pokemon> GetPokemonCardsAsync(string query = "")
        {
            // If the user did not provide a search query, pick at random.
            query ??= GetRandomPokemon();
            return await Card.GetAsync<Pokemon>(new Dictionary<string, string> {{"name", query}});
        }

        /// <summary>
        ///     Return a Pokémon card based on the Id provided.
        /// </summary>
        public static PokemonCard GetExactPokemon(string cardId)
        {
            return Card.Find<Pokemon>(cardId).Card;
        }

        /// <summary>
        ///     Pick a random Pokémon from the list.
        /// </summary>
        private static string GetRandomPokemon()
        {
            return PokemonList[new Random().Next(0, PokemonList.Count)];
        }

        /// <summary>
        ///     Update the list of Pokémon cards.
        /// </summary>
        public static async Task<bool> UpdatePokemonListAsync()
        {
            try
            {
                PokemonList.Clear();
                var cards = await Card.AllAsync();
                foreach (var pokemon in cards)
                    PokemonList.Add(pokemon.Name);
                PokemonList = PokemonList.Distinct().ToList();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.ERR_POKEMON_LIST, ex.Message);
                return false;
            }
        }
    }
}