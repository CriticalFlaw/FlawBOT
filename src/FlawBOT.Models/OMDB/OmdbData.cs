using Newtonsoft.Json;

namespace FlawBOT.Models.OMDB
{
    public class OmdbData
    {
        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Year")]
        public string Year { get; set; }

        [JsonProperty("Rated")]
        public string Rated { get; set; }

        [JsonProperty("Released")]
        public string Released { get; set; }

        [JsonProperty("Runtime")]
        public string Runtime { get; set; }

        [JsonProperty("Genre")]
        public string Genre { get; set; }

        [JsonProperty("Director")]
        public string Director { get; set; }

        [JsonProperty("Actors")]
        public string Actors { get; set; }

        [JsonProperty("Plot")]
        public string Plot { get; set; }

        [JsonProperty("Poster")]
        public string Poster { get; set; }

        [JsonProperty("imdbRating")]
        public string ImdbRating { get; set; }

        [JsonProperty("imdbID")]
        public string ImdbId { get; set; }

        [JsonProperty("BoxOffice")]
        public string BoxOffice { get; set; }

        [JsonProperty("Response")]
        public string Response { get; set; }

        [JsonProperty("Error")]
        public string Error { get; set; }
    }
}
