using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    public class DogData
    {
        [JsonProperty("status")] public string Status { get; set; }

        [JsonProperty("message")] public string Message { get; set; }
    }

    public class IpStack
    {
        [JsonProperty("ip")] public string Ip { get; set; }

        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("continent_name")] public string Continent { get; set; }

        [JsonProperty("country_name")] public string Country { get; set; }

        [JsonProperty("region_name")] public string Region { get; set; }

        [JsonProperty("city")] public string City { get; set; }

        [JsonProperty("zip")] public string ZipCode { get; set; }

        [JsonProperty("latitude")] public double Latitude { get; set; }

        [JsonProperty("longitude")] public double Longitude { get; set; }

        [JsonProperty("continent_code")]
        [JsonIgnore]
        private string ContinentCode { get; set; }

        [JsonProperty("country_code")]
        [JsonIgnore]
        private string CountryCode { get; set; }

        [JsonProperty("region_code")]
        [JsonIgnore]
        private string RegionCode { get; set; }

        [JsonProperty("location")]
        [JsonIgnore]
        private string Location { get; set; }
    }
}