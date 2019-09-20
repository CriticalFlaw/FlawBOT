using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;

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
    }
}