using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;
using PokemonTcgSdk;
using PokemonTcgSdk.Models;

namespace FlawBOT.Framework.Services
{
    public class PokemonService : HttpHandler
    {
        public static List<string> PokemonList { get; set; } = new List<string>();

        public static async Task<PokemonCards> GetPokemonCardsAsync(string query)
        {
            query = (string.IsNullOrWhiteSpace(query)) ? GetRandomPokemonAsync() : query;
            var results = await _http.GetStringAsync(Resources.API_PokemonTCG + "?name=" + query.ToLowerInvariant().Trim());
            return JsonConvert.DeserializeObject<PokemonCards>(results);
        }

        public static PokemonCard GetExactPokemonAsync(string cardID)
        {
            return PokemonTcgSdk.Card.Find<Pokemon>(cardID).Card;
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
                var list = await _http.GetStringAsync(Resources.API_Pokemon + "?limit=800");
                var results = JsonConvert.DeserializeObject<PokemonData>(list).Results;
                PokemonList.Clear();
                foreach (var pokemon in results)
                    if (!string.IsNullOrWhiteSpace(pokemon.Name))
                        PokemonList.Add(pokemon.Name);
                PokemonList = PokemonList.Distinct().ToList();
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