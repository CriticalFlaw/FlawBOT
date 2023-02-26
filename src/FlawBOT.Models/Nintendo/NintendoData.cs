using Newtonsoft.Json;

namespace FlawBOT.Models.Nintendo
{
    public class NintendoData
    {
        [JsonProperty("amiibo")]
        public List<Amiibo> Amiibo { get; set; }
    }
}