using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace XslFO.Samples.MovieSearchService
{
    public class MovieSearchResponse
    {
        //NOTE: Flag to denote if we want the Templating engine to enable Fonet Compatibility mode
        //          because the syntax for generating some FO has changed, with tighter restrictions in ApacheFOP.
        //NOTE: We default to True for compatibility with prior functionality.
        public bool FonetCompatibilityEnabled { get; set; } = true;

        [JsonProperty("Search")]
        [JsonPropertyName("Search")]
        public List<SearchResult> SearchResults { get; set; }
        public string TotalResults { get; set; }
        public string Response { get; set; }
        public string SearchTitle { get; set; }
        public DateTime SearchDateTime { get; set; }
    }

    public class SearchResult
    {
        string _poster = string.Empty;

        public string Title { get; set; }
        public string Year { get; set; }
        public string ImdbID { get; set; }
        public string Type { get; set; }

        //NOTE: Custom Property Logic here makes the Templating Logic much easier and improves separation of presentation from data!
        public string Poster
        {
            get => _poster;
            set => _poster = value.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? value : string.Empty;
        }
    }

}