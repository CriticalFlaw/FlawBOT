using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Models
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

    #region NEWS

    public class NewsData
    {
        [JsonProperty("@status")]
        public string Status { get; set; }

        [JsonProperty("totalResults")]
        public int ArticleCount { get; set; }

        [JsonProperty("articles")]
        public List<Article> Articles { get; set; }
    }

    public class Article
    {
        [JsonProperty("source")]
        public Source Source { get; set; }

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

    public class Source
    {
        [JsonProperty("@id")]
        public string ID { get; set; }

        [JsonProperty("@name")]
        public string Name { get; set; }
    }

    #endregion NEWS

    #region IP_LOCATION

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

        [JsonProperty("org")]
        public string Organization { get; set; }

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

    #endregion IP_LOCATION
}