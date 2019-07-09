using FlawBOT.Common;
using FlawBOT.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services.Games
{
    public class PokemonService : HttpHandler
    {
        public static async Task<PokemonCards> GetPokemonCardsAsync(string query)
        {
            if (SharedData.PokemonList.Count <= 0) await UpdatePokemonListAsync();
            query = (string.IsNullOrWhiteSpace(query)) ? GetRandomPokemonAsync() : query;
            var results = await _http.GetStringAsync("https://api.pokemontcg.io/v1/cards?name=" + query.Trim());
            return JsonConvert.DeserializeObject<PokemonCards>(results);
        }

        public static string GetRandomPokemonAsync()
        {
            var random = new Random();
            return SharedData.PokemonList[random.Next(0, SharedData.PokemonList.Count)];
        }

        public static async Task<bool> UpdatePokemonListAsync()
        {
            try
            {
                var client = new HttpClient();
                var list = await client.GetStringAsync("https://pokeapi.co/api/v2/pokemon/?limit=100");
                var results = JsonConvert.DeserializeObject<PokemonData>(list).results;
                SharedData.PokemonList.Clear();
                foreach (var pokemon in results)
                    if (!string.IsNullOrWhiteSpace(pokemon.name))
                        SharedData.PokemonList.Add(pokemon.name);
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