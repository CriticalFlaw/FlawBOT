using DSharpPlus;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Models.Dictionary;
using FlawBOT.Properties;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Services
{
    public class DictionaryService : HttpHandler
    {
        public static async Task<DiscordEmbed> GetDictionaryDefinitionAsync(string query)
        {
            try
            {
                var url = string.IsNullOrWhiteSpace(query) ? Resources.URL_Dictionary_Random : string.Format(Resources.URL_Dictionary, Uri.EscapeDataString(query.Trim()));
                var response = await Http.GetStringAsync(url).ConfigureAwait(false);
                return BuildDefinitionEmbed(JsonConvert.DeserializeObject<UrbanDictionaryList>(response));
            }
            catch
            {
                return null;
            }
        }

        public static DiscordEmbed BuildDefinitionEmbed(UrbanDictionaryList result)
        {
            if (result?.List is null || result.ResultType == "no_results" || result.List.Count == 0) return null;
            var results = result.List[Random.Next(result.List.Count)];
            var output = new DiscordEmbedBuilder()
                .WithTitle(results.Word.Truncate(EmbedExtensions.TitleLimit))
                .AddSafeField("Definition", results.Definition.Truncate(500))
                .AddSafeField(":thumbsup:", results.ThumbsUp.ToString(), true)
                .AddSafeField(":thumbsdown:", results.ThumbsDown.ToString(), true)
                .WithColor(new DiscordColor("#1F2439"));
            if (!string.IsNullOrWhiteSpace(results.Example))
                output.WithDescription(Formatter.Italic(results.Example.Trim().Truncate(1000)));
            if (Uri.IsWellFormedUriString(results.Permalink, UriKind.Absolute))
                output.WithUrl(results.Permalink);
            if (!string.IsNullOrWhiteSpace(results.Author))
                output.WithFooter($"Submitted by: {results.Author} on {results.WrittenOn?.Split('T')[0]}");
            return output.Build();
        }
    }
}
