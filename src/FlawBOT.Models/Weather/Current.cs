using Newtonsoft.Json;

namespace FlawBOT.Models.Weather
{
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
        public int WindSpeed { get; set; }

        [JsonProperty("wind_degree", NullValueHandling = NullValueHandling.Ignore)]
        public int WindDegree { get; set; }

        [JsonProperty("wind_dir", NullValueHandling = NullValueHandling.Ignore)]
        public string WindDirection { get; set; }

        [JsonProperty("pressure", NullValueHandling = NullValueHandling.Ignore)]
        public int Pressure { get; set; }

        [JsonProperty("precip", NullValueHandling = NullValueHandling.Ignore)]
        public double Precipitation { get; set; }

        [JsonProperty("humidity", NullValueHandling = NullValueHandling.Ignore)]
        public int Humidity { get; set; }

        [JsonProperty("cloudcover", NullValueHandling = NullValueHandling.Ignore)]
        public int CloudCover { get; set; }

        [JsonProperty("feelslike", NullValueHandling = NullValueHandling.Ignore)]
        public int FeelsLike { get; set; }

        [JsonProperty("uv_index", NullValueHandling = NullValueHandling.Ignore)]
        public int UvIndex { get; set; }

        [JsonProperty("visibility", NullValueHandling = NullValueHandling.Ignore)]
        public int Visibility { get; set; }

        [JsonProperty("is_day", NullValueHandling = NullValueHandling.Ignore)]
        public string IsDay { get; set; }
    }
}