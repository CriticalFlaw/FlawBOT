using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;
using PokemonTcgSdk;
using PokemonTcgSdk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class PokemonService : HttpHandler
    {
        public static List<string> PokemonList { get; set; } = new List<string>();

        public static async Task<PokemonCards> GetPokemonCardsAsync(string query = "")
        {
            query ??= GetRandomPokemon();
            var results = await _http.GetStringAsync(Resources.API_PokemonTCG + "?name=" + query.ToLowerInvariant().Trim()).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<PokemonCards>(results);
        }

        public static PokemonCard GetExactPokemon(string cardID)
        {
            return PokemonTcgSdk.Card.Find<Pokemon>(cardID).Card;
        }

        public static string GetRandomPokemon()
        {
            var random = new Random();
            return PokemonList[random.Next(0, PokemonList.Count)];
        }

        public static async Task<bool> UpdatePokemonListAsync()
        {
            try
            {
                var list = await _http.GetStringAsync(Resources.API_Pokemon).ConfigureAwait(false);
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
                Console.WriteLine("Error updating the Pokémon list. " + ex.Message);
                return false;
            }
        }
    }
}