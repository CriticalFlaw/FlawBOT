using Newtonsoft.Json;

namespace FlawBOT.Models.Imgur
{
    public class ImgurGalleryResponse
    {
        [JsonProperty("data")]
        public List<ImgurGalleryItem> Data { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public class ImgurGalleryItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("nsfw")]
        public bool Nsfw { get; set; }

        [JsonProperty("is_album")]
        public bool IsAlbum { get; set; }

        [JsonProperty("cover")]
        public string Cover { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        ///     Direct image link. Albums link to their gallery page, so use the cover image instead.
        /// </summary>
        public string GetImageLink()
        {
            if (!IsAlbum) return Link;
            return string.IsNullOrWhiteSpace(Cover) ? null : $"https://i.imgur.com/{Cover}.jpg";
        }
    }
}
