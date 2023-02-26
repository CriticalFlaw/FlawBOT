﻿using Newtonsoft.Json;

namespace FlawBOT.Modules
{
    public class DogData
    {
        [JsonProperty("status")] public string Status { get; set; }

        [JsonProperty("message")] public string Message { get; set; }
    }

    public class IPLocation
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("region_code")]
        public string RegionCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("country_name")]
        public string CountryName { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("country_code_iso3")]
        public string CountryCodeIso3 { get; set; }

        [JsonProperty("country_capital")]
        public string CountryCapital { get; set; }

        [JsonProperty("country_tld")]
        public string CountryTld { get; set; }

        [JsonProperty("continent_code")]
        public string ContinentCode { get; set; }

        [JsonProperty("in_eu")]
        public bool InEu { get; set; }

        [JsonProperty("postal")]
        public string Postal { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("utc_offset")]
        public string UtcOffset { get; set; }

        [JsonProperty("country_calling_code")]
        public string CountryCallingCode { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("currency_name")]
        public string CurrencyName { get; set; }

        [JsonProperty("languages")]
        public string Languages { get; set; }

        [JsonProperty("country_area")]
        public double CountryArea { get; set; }

        [JsonProperty("country_population")]
        public int CountryPopulation { get; set; }

        [JsonProperty("asn")]
        public string Asn { get; set; }

        [JsonProperty("org")]
        public string Org { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}