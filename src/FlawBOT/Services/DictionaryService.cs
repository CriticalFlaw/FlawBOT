using DSharpPlus;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Dictionary;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class DictionaryService : HttpHandler
    {
        public static async Task<DiscordEmbed> GetDictionaryDefinitionAsync(string query)
        {
            try
            {
                query = string.IsNullOrWhiteSpace(query) ? Resources.URL_Dictionary_Random : string.Format(Resources.URL_Dictionary, WebUtility.UrlEncode(query.Trim()));
                var response = await Http.GetStringAsync(query).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<UrbanDictionaryList>(response);
                if (result.ResultType == "no_results" || result.List.Count == 0) return null;
                var results = result.List[random.Next(result.List.Count)];

                // TODO: Add pagination when supported for slash commands.
                var output = new DiscordEmbedBuilder()
                    .WithTitle(Formatter.Bold(results.Word))
                    .WithDescription(Formatter.Italic(results.Example) ?? string.Empty)
                    .AddField("Definition", results.Definition.Length < 500 ? results.Definition : results.Definition.Take(500) + "...")
                    .AddField(":thumbsup:", results.ThumbsUp.ToString(), true)
                    .AddField(":thumbsdown:", results.ThumbsDown.ToString(), true)
                    .WithUrl(results.Permalink)
                    .WithFooter(string.IsNullOrWhiteSpace(results.Author) ? string.Empty : "Submitted by: " + results.Author + " on " + results.WrittenOn.Split('T').First())
                    .WithColor(new DiscordColor("#1F2439"));
                return output.Build();
            }
            catch
            {
                return null;
            }
        }
    }
}