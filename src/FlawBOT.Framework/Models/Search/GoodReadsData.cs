using System.Collections.Generic;
using System.Xml.Serialization;

namespace FlawBOT.Framework.Models
{
    [XmlRoot(ElementName = "GoodreadsResponse")]
    public class GoodreadsResponse
    {
        [XmlElement("search")]
        public GoodreadsSearchInfo Search { get; set; }
    }

    public class GoodreadsSearchInfo
    {
        [XmlElement("total-results")]
        public int ResultCount { get; set; }

        [XmlArray("results"), XmlArrayItem("work")]
        public List<Work> Results { get; set; }
    }

    [XmlRoot(ElementName = "work")]
    public class Work
    {
        [XmlElement(ElementName = "best_book")]
        public Book Book { get; set; }

        [XmlElement(ElementName = "original_publication_day")]
        public PublicationDay PublicationDay { get; set; }

        [XmlElement(ElementName = "original_publication_month")]
        public PublicationMonth PublicationMonth { get; set; }

        [XmlElement(ElementName = "original_publication_year")]
        public PublicationYear PublicationYear { get; set; }

        [XmlElement(ElementName = "average_rating")]
        public string RatingAverage { get; set; }

        [XmlElement(ElementName = "ratings_count")]
        public RatingsCount RatingCount { get; set; }
    }

    [XmlRoot(ElementName = "best_book")]
    public class Book
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "author")]
        public Author Author { get; set; }

        [XmlElement(ElementName = "image_url")]
        public string ImageUrl { get; set; }

        [XmlElement(ElementName = "small_image_url")]
        public string ImageUrlSmall { get; set; }
    }

    [XmlRoot(ElementName = "author")]
    public class Author
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "original_publication_day")]
    public class PublicationDay
    {
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "original_publication_month")]
    public class PublicationMonth
    {
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "original_publication_year")]
    public class PublicationYear
    {
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ratings_count")]
    public class RatingsCount
    {
        [XmlText]
        public string Text { get; set; }
    }
}