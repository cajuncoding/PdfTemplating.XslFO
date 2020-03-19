using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PdfTemplating.WebMVC.MovieSearch
{
    public class SearchResult
    {
        string _poster = string.Empty;

        public string Title { get; set; }
        public string Year { get; set; }
        public string ImdbID { get; set; }
        public string Type { get; set; }
        
        //NOTE: Custom Property Logic here makes the Templating Logic much easier and improves separation of presentation from data!
        public string Poster { 
            get => _poster;
            set => _poster = value.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? value : string.Empty;
        }
    }

    public class MovieSearchResponse
    {
        [JsonProperty("Search")]
        public List<SearchResult> SearchResults { get; set; }
        public string TotalResults { get; set; }
        public string Response { get; set; }
        public string SearchTitle { get; set; }
        public DateTime SearchDateTime { get; set; }
    }
}