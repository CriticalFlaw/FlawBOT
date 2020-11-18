using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    public class AmiiboData
    {
        [JsonProperty("amiibo")] public List<Amiibo> Amiibo { get; set; }
    }

    public class Amiibo
    {
        [JsonProperty("amiiboSeries")] public string AmiiboSeries { get; set; }

        [JsonProperty("gameSeries")] public string GameSeries { get; set; }

        [JsonProperty("image")] public string Image { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("release")] public Release ReleaseDate { get; set; }
    }

    public class Release
    {
        [JsonProperty("au")] public string Australian { get; set; }

        [JsonProperty("eu")] public string European { get; set; }

        [JsonProperty("jp")] public string Japanese { get; set; }

        [JsonProperty("na")] public string American { get; set; }
    }
}