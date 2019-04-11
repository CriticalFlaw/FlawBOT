using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlawBOT.Models
{
    public class PokemonData
    {
        [JsonProperty("cards")]
        public List<PokemonCard> cards { get; set; }
    }

    public class PokemonCard
    {
        [JsonProperty("id")]
        public string id { get; set; }
    }
}