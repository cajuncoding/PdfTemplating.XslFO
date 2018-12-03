using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace XslFO.WebMVC.MovieSearch
{
    public class SearchResult
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string ImdbID { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }
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