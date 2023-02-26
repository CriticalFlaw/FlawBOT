using Newtonsoft.Json;

namespace FlawBOT.Models.Dictionary
{
    public class UrbanDictionaryList
    {
        [JsonProperty("result_type")]
        public string ResultType { get; set; }

        [JsonProperty("list")]
        public List<UrbanDictionaryDto> List { get; set; }
    }
}