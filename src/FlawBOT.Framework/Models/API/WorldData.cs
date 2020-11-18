using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
{
    #region IP

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

    #endregion IP

    #region WEATHER

    public class WeatherData
    {
        [JsonProperty("request", NullValueHandling = NullValueHandling.Ignore)]
        public Request Request { get; set; }

        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public Location Location { get; set; }

        [JsonProperty("current", NullValueHandling = NullValueHandling.Ignore)]
        public Current Current { get; set; }
    }

    public class Request
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
        public string Query { get; set; }

        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }

        [JsonProperty("unit", NullValueHandling = NullValueHandling.Ignore)]
        public string Unit { get; set; }
    }

    public class Location
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; }

        [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
        public string Region { get; set; }

        [JsonProperty("lat", NullValueHandling = NullValueHandling.Ignore)]
        public string Latitude { get; set; }

        [JsonProperty("lon", NullValueHandling = NullValueHandling.Ignore)]
        public string Longitude { get; set; }

        [JsonProperty("timezone_id", NullValueHandling = NullValueHandling.Ignore)]
        public string Timezone { get; set; }

        [JsonProperty("localtime", NullValueHandling = NullValueHandling.Ignore)]
        public string LocalTime { get; set; }

        [JsonProperty("localtime_epoch", NullValueHandling = NullValueHandling.Ignore)]
        public int LocalTime_epoch { get; set; }

        [JsonProperty("utc_offset", NullValueHandling = NullValueHandling.Ignore)]
        public string UTC_Offset { get; set; }
    }

    public class Current
    {
        [JsonProperty("observation_time", NullValueHandling = NullValueHandling.Ignore)]
        public string ObservationTime { get; set; }

        [JsonProperty("temperature", NullValueHandling = NullValueHandling.Ignore)]
        public int Temperature { get; set; }

        [JsonProperty("weather_code", NullValueHandling = NullValueHandling.Ignore)]
        public int Code { get; set; }

        [JsonProperty("weather_icons", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Icons { get; set; }

        [JsonProperty("weather_descriptions", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Descriptions { get; set; }

        [JsonProperty("wind_speed", NullValueHandling = NullValueHandling.Ignore)]
        public int Wind_Speed { get; set; }

        [JsonProperty("wind_degree", NullValueHandling = NullValueHandling.Ignore)]
        public int Wind_Degree { get; set; }

        [JsonProperty("wind_dir", NullValueHandling = NullValueHandling.Ignore)]
        public string Wind_Direction { get; set; }

        [JsonProperty("pressure", NullValueHandling = NullValueHandling.Ignore)]
        public int Pressure { get; set; }

        [JsonProperty("precip", NullValueHandling = NullValueHandling.Ignore)]
        public int Precipitation { get; set; }

        [JsonProperty("humidity", NullValueHandling = NullValueHandling.Ignore)]
        public int Humidity { get; set; }

        [JsonProperty("cloudcover", NullValueHandling = NullValueHandling.Ignore)]
        public int CloudCover { get; set; }

        [JsonProperty("feelslike", NullValueHandling = NullValueHandling.Ignore)]
        public int FeelsLike { get; set; }

        [JsonProperty("uv_index", NullValueHandling = NullValueHandling.Ignore)]
        public int UV_Index { get; set; }

        [JsonProperty("visibility", NullValueHandling = NullValueHandling.Ignore)]
        public int Visibility { get; set; }

        [JsonProperty("is_day", NullValueHandling = NullValueHandling.Ignore)]
        public string IsDay { get; set; }
    }

    #endregion WEATHER
}