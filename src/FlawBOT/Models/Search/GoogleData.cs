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
        private TimeZoneResult timeZone;

        [JsonProperty("time")]
        private DateTime time;

        [JsonProperty("status")]
        public string status { get; set; }

        public GeolocationModel[] Results { get => results; set => results = value; }
        public TimeZoneResult Timezone { get => timeZone; set => timeZone = value; }
        public DateTime Time { get => time; set => time = value; }

        public class GeolocationModel
        {
            [JsonProperty("formatted_address")]
            public string FormattedAddress { get; set; }

            [JsonProperty("geometry")]
            public GeometryModel Geometry { get; set; }

            public class GeometryModel
            {
                [JsonProperty("location")]
                public LocationModel Location { get; set; }

                public class LocationModel
                {
                    [JsonProperty("lat")]
                    public float Latitude { get; set; }

                    [JsonProperty("lng")]
                    public float Longitude { get; set; }
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
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sys")]
        public Sys Sys { get; set; }

        [JsonProperty("main")]
        public Main Main { get; set; }

        [JsonProperty("wind")]
        public Wind Wind { get; set; }

        [JsonProperty("weather")]
        public List<Weather> Weather { get; set; }

        [JsonProperty("cod")]
        public int COD { get; set; }
    }

    public class Sys
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }

    public class Main
    {
        [JsonProperty("temp")]
        public double Temperature { get; set; }

        [JsonProperty("humidity")]
        public float Humidity { get; set; }

        [JsonProperty("tempMin")]
        public double MinTemp { get; set; }

        [JsonProperty("tempMax")]
        public double MaxTemp { get; set; }
    }

    public class Wind
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }
    }

    public class Weather
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("main")]
        public string Main { get; set; }
    }

    #endregion WEATHER
}