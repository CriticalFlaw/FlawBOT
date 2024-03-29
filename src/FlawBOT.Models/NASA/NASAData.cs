﻿using Newtonsoft.Json;

namespace FlawBOT.Models.NASA
{
    public class NasaData
    {
        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("explanation")]
        public string Description { get; set; }

        [JsonProperty("hdurl")]
        public string ImageHd { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string ImageSd { get; set; }
    }
}