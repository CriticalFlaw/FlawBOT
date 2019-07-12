using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlawBOT.Framework.Models
{
    public class SteamData
    {
        [JsonProperty("applist")]
        public Applist AppList { get; set; }
    }

    public class Applist
    {
        [JsonProperty("apps")]
        public List<App> Apps { get; set; }
    }

    public class App
    {
        [JsonProperty("appid")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}