using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    public class DogData
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class IPLocationData
    {
        [JsonProperty("@as")]
        public string Title { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("isp")]
        public string ISP { get; set; }

        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lon")]
        public double Longitude { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("regionName")]
        public string Region { get; set; }

        [JsonProperty("region")]
        public string RegionCode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("timezone")]
        public string TimeZone { get; set; }

        [JsonProperty("zip")]
        public string ZipCode { get; set; }
    }
}