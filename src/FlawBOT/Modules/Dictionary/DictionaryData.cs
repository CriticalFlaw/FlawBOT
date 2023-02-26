﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlawBOT.Modules
{
    public class UrbanDictionaryList
    {
        [JsonProperty("result_type")] public string ResultType { get; set; }

        [JsonProperty("list")] public List<UrbanDictionaryDto> List { get; set; }
    }

    public class UrbanDictionaryDto
    {
        [JsonProperty("definition")] public string Definition { get; set; }

        [JsonProperty("author")] public string Author { get; set; }

        [JsonProperty("permalink")] public string Permalink { get; set; }

        [JsonProperty("example")] public string Example { get; set; }

        [JsonProperty("thumbs_up")] public int ThumbsUp { get; set; }

        [JsonProperty("thumbs_down")] public int ThumbsDown { get; set; }
    }
}