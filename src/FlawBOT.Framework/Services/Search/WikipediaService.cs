﻿using System;
using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Services
{
    public class WikipediaService : HttpHandler
    {
        public static async Task<WikipediaData.WikipediaQuery.WikipediaPage> GetWikipediaDataAsync(string query)
        {
            var results = await Http.GetStringAsync(Resources.API_Wikipedia + "&titles=" + Uri.EscapeDataString(query))
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<WikipediaData>(results).Query.Pages[0];
        }
    }
}