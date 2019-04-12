using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlawBOT.Models
{
    public class PokemonCards
    {
        [JsonProperty("cards")]
        public List<Card> cards { get; set; }
    }

    public class Card
    {
        [JsonProperty("id")]
        public string id { get; set; }
    }

    public class PokemonData
    {
        [JsonProperty("count")]
        public int count { get; set; }

        [JsonProperty("next")]
        public string next { get; set; }

        [JsonProperty("previous")]
        public object previous { get; set; }

        [JsonProperty("results")]
        public List<Result> results { get; set; }
    }

    public class Result
    {

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }
    }
}