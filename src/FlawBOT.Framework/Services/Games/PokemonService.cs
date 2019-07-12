using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class PokemonService : HttpHandler
    {
        public static List<string> PokemonList { get; set; } = new List<string>();

        public static async Task<PokemonCards> GetPokemonCardsAsync(string query)
        {
            if (PokemonList.Count <= 0) await UpdatePokemonListAsync().ConfigureAwait(false);
            query = (string.IsNullOrWhiteSpace(query)) ? GetRandomPokemonAsync() : query;
            var results = await _http.GetStringAsync("https://api.pokemontcg.io/v1/cards?name=" + query.Trim());
            return JsonConvert.DeserializeObject<PokemonCards>(results);
        }

        public static string GetRandomPokemonAsync()
        {
            var random = new Random();
            return PokemonList[random.Next(0, PokemonList.Count)];
        }

        public static async Task<bool> UpdatePokemonListAsync()
        {
            try
            {
                var client = new HttpClient();
                var list = await client.GetStringAsync("https://pokeapi.co/api/v2/pokemon/?limit=100");
                var results = JsonConvert.DeserializeObject<PokemonData>(list).Results;
                PokemonList.Clear();
                foreach (var pokemon in results)
                    if (!string.IsNullOrWhiteSpace(pokemon.Name))
                        PokemonList.Add(pokemon.Name);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error update Pokemon list. " + ex.Message);
                return false;
            }
        }
    }
}