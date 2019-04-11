using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlawBOT.Models
{
    public class DictionaryData
    {
        [JsonProperty("result_type")]
        public string result_type { get; set; }

        [JsonProperty("list")]
        public List<DictionaryDataList> list { get; set; }
    }

    public class DictionaryDataList
    {
        [JsonProperty("definition")]
        public string definition { get; set; }

        [JsonProperty("author")]
        public string author { get; set; }

        [JsonProperty("permalink")]
        public string permalink { get; set; }

        [JsonProperty("example")]
        public string example { get; set; }

        [JsonProperty("thumbs_up")]
        public int thumbs_up { get; set; }

        [JsonProperty("thumbs_down")]
        public int thumbs_down { get; set; }
    }
}