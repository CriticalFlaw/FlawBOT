using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FlawBOT.Models
{
    #region TIME

    public class TimeData
    {
        [JsonProperty("results")]
        private GeolocationModel[] results;

        [JsonProperty("timezone")]
        private TimeZoneResult timezone;

        [JsonProperty("time")]
        private DateTime time;

        [JsonProperty("status")]
        public string status { get; set; }

        public GeolocationModel[] Results { get => results; set => results = value; }
        public TimeZoneResult Timezone { get => timezone; set => timezone = value; }
        public DateTime Time { get => time; set => time = value; }

        public class GeolocationModel
        {
            [JsonProperty("formatted_address")]
            public string formatted_address { get; set; }

            [JsonProperty("geometry")]
            public GeometryModel geometry { get; set; }

            public class GeometryModel
            {
                [JsonProperty("location")]
                public LocationModel location { get; set; }

                public class LocationModel
                {
                    [JsonProperty("lat")]
                    public float lat { get; set; }

                    [JsonProperty("lng")]
                    public float lng { get; set; }
                }
            }
        }

        public class TimeZoneResult
        {
            [JsonProperty("dstOffset")]
            public double dstOffset { get; set; }

            [JsonProperty("rawOffset")]
            public double rawOffset { get; set; }

            [JsonProperty("timeZoneName")]
            public string timeZoneName { get; set; }
        }
    }

    #endregion TIME

    #region WEATHER

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

    #endregion WEATHER
}