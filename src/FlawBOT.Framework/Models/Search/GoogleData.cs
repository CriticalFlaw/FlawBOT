using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FlawBOT.Framework.Models
{
    #region TIME

    public class TimeData
    {
        [JsonProperty("results")]
        private GeolocationModel[] _results;

        [JsonProperty("timezone")]
        private TimeZoneResult _timeZone;

        [JsonProperty("status")]
        public string Status { get; set; }

        public GeolocationModel[] Results { get; }
        public TimeZoneResult Timezone { get; set; }
        public DateTime Time { get; set; }

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
            public double DstOffset { get; set; }

            [JsonProperty("rawOffset")]
            public double RawOffset { get; set; }

            [JsonProperty("timeZoneName")]
            public string TimeZoneName { get; set; }
        }
    }

    #endregion TIME

    #region NEWS

    public class NewsData
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("articles")]
        public List<Article> Articles { get; set; }
    }

    public class Article
    {
        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("urlToImage")]
        public string UrlImage { get; set; }

        [JsonProperty("publishedAt")]
        public DateTime PublishDate { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    #endregion NEWS

    #region WEATHER

    public class WeatherData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

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
        public int Cod { get; set; }
    }

    public class Sys
    {
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
        public int Id { get; set; }

        [JsonProperty("main")]
        public string Main { get; set; }
    }

    #endregion WEATHER
}