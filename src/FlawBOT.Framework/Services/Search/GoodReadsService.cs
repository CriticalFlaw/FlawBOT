using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlawBOT.Framework.Services.Search
{
    public class GoodReadsService : HttpHandler
    {
        public static async Task<GoodreadsResponse> GetBookDataAsync(string query)
        {
            var _serializer = new XmlSerializer(typeof(GoodreadsResponse));
            var results = await _http.GetStreamAsync(Resources.API_GoodReads + "?key=" + TokenHandler.Tokens.GoodReadsToken + "&q=" + WebUtility.UrlEncode(query)).ConfigureAwait(false);
            return (GoodreadsResponse)_serializer.Deserialize(results);
        }

        /// <summary>
        /// Retrieve and format the book's publication date.
        /// </summary>
        /// <param name="book">Good Reads book to retrieve publication date from.</param>
        public static string GetPublicationDate(Work book)
        {
            try
            {
                var results = new StringBuilder();
                // TODO: Add the day and month
                results.Append($"{book.PublicationMonth.Text ?? "01"}-{book.PublicationDay.Text ?? "01"}-{book.PublicationYear.Text}");
                return results.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}