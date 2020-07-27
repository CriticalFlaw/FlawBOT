using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;
using PokemonTcgSdk;
using PokemonTcgSdk.Models;
using Card = PokemonTcgSdk.Card;

namespace FlawBOT.Framework.Services
{
    public class PokemonService : HttpHandler
    {
        private static List<string> PokemonList { get; set; } = new List<string>();

        public static async Task<PokemonCards> GetPokemonCardsAsync(string query = "")
        {
            query ??= GetRandomPokemon();
            var results = await Http
                .GetStringAsync(Resources.API_PokemonTCG + "?name=" + query.ToLowerInvariant().Trim())
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<PokemonCards>(results);
        }

        public static PokemonCard GetExactPokemon(string cardId)
        {
            return Card.Find<Pokemon>(cardId).Card;
        }

        private static string GetRandomPokemon()
        {
            var random = new Random();
            return PokemonList[random.Next(0, PokemonList.Count)];
        }

        public static async Task<bool> UpdatePokemonListAsync()
        {
            try
            {
                var list = await Http.GetStringAsync(Resources.API_Pokemon).ConfigureAwait(false);
                var results = JsonConvert.DeserializeObject<PokemonData>(list).Results;
                PokemonList.Clear();
                foreach (var pokemon in results.Where(pokemon => !string.IsNullOrWhiteSpace(pokemon.Name)))
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