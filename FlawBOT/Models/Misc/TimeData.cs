using Newtonsoft.Json;
using System;

namespace FlawBOT.Models
{
    public class TimeData
    {
        [JsonProperty("results")]
        public GeolocationModel[] results;

        [JsonProperty("timezone")]
        public TimeZoneResult timezone;

        [JsonProperty("time")]
        public DateTime time;

        [JsonProperty("status")]
        public string status { get; set; }

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
}