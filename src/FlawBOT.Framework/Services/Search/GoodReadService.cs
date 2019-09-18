using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FlawBOT.Framework.Common;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Goodreads;
using Goodreads.Http;
using Goodreads.Models.Response;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Services.Search
{
    public class GoodReadService : HttpHandler
    {
        public static async Task<GoodReadsResponse> GetBookDataAsync(string query)
        {
            var _serializer = new XmlSerializer(typeof(GoodReadsResponse));
            var results = await _http.GetStreamAsync(Resources.API_GoodReads + "?key=" + TokenHandler.Tokens.GoodReadsKey + "&q=" + WebUtility.UrlEncode(query));
            return (GoodReadsResponse)_serializer.Deserialize(results);
        }
    }
}