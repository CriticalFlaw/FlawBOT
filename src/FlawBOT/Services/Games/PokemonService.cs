using FlawBOT.Common;
using FlawBOT.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Services.Games
{
    public class PokemonService
    {
        private static readonly string base_url = "http://api.pokemontcg.io/v1/cards?name=";
        private static readonly string poke_url = "https://pokeapi.co/api/v2/pokemon/?limit=100";
        private static readonly HttpClient http = new HttpClient();

        public static async Task<PokemonCards> GetPokemonCardsAsync(string query)
        {
            var results = await http.GetStringAsync(base_url + query.Trim());
            return JsonConvert.DeserializeObject<PokemonCards>(results);
        }

        public static string GetRandomPokemonAsync()
        {
            var random = new Random();
            return SharedData.PokemonList[random.Next(0, SharedData.PokemonList.Count)];
        }

        public static async Task UpdatePokemonList()
        {
            var results = await http.GetStringAsync(poke_url);
            SharedData.PokemonList.Clear();
            var list = JsonConvert.DeserializeObject<PokemonData>(results);
            foreach (var pokemon in list.results)
                SharedData.PokemonList.Add(pokemon.name);
        }
    }
}