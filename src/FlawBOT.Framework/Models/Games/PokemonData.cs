using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlawBOT.Framework.Models
{
    public class PokemonCards
    {
        [JsonProperty("cards")]
        public List<Card> Cards { get; set; }
    }

    public class Card
    {
        [JsonProperty("id")]
        public string ID { get; set; }
    }

    public class PokemonData
    {
        [JsonProperty("results")]
        public List<DataResult> Results { get; set; }
    }

    public class DataResult
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}