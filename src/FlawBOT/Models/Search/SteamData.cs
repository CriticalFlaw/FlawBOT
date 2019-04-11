using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlawBOT.Models
{
    public class SteamData
    {
        [JsonProperty("applist")]
        public Applist applist { get; set; }
    }

    public class Applist
    {
        [JsonProperty("apps")]
        public List<App> apps { get; set; }
    }

    public class App
    {
        [JsonProperty("appid")]
        public int appid { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }
    }
}