using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlawBOT.Models
{
    public class WeatherData
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("sys")]
        public Sys sys { get; set; }

        [JsonProperty("main")]
        public Main main { get; set; }

        [JsonProperty("wind")]
        public Wind wind { get; set; }

        [JsonProperty("weather")]
        public List<Weather> weather { get; set; }

        [JsonProperty("cod")]
        public int cod { get; set; }
    }

    public class Sys
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("country")]
        public string country { get; set; }
    }

    public class Main
    {
        [JsonProperty("temp")]
        public double temp { get; set; }

        [JsonProperty("humidity")]
        public float humidity { get; set; }

        [JsonProperty("tempMin")]
        public double tempMin { get; set; }

        [JsonProperty("tempMax")]
        public double tempMax { get; set; }
    }

    public class Wind
    {
        [JsonProperty("speed")]
        public double speed { get; set; }
    }

    public class Weather
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("main")]
        public string main { get; set; }
    }
}