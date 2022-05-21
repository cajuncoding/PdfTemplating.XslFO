using Newtonsoft.Json;
using RestSharp;
using RestSharp.CustomExtensions;
using System;
using System.Threading.Tasks;
using PdfTemplating.WebMvc.MovieSearch;

namespace AspNetCoreMvc.MovieSearch
{
    public class MovieSearchService
    {
        public String ApiKey { get; protected set; } = "e915adab";

        public async Task<MovieSearchResponse> SearchAsync(String title)
        {
            var jsonText = await ExecuteMovieQueryRESTUri(title);
            var searchResponse = JsonConvert.DeserializeObject<MovieSearchResponse>(jsonText);
            
            //Update other properties
            searchResponse.SearchTitle = title;
            searchResponse.SearchDateTime = DateTime.Now;

            return searchResponse;
        }

        /// <summary>
        /// BBernard - 02/21/2018
        /// Helper method to better isolate/separate/re-use the logic required to execute the Request from the business logic making the request.
        /// </summary>
        /// <param name="uriPath"></param>
        /// <returns></returns>
        protected async Task<String> ExecuteMovieQueryRESTUri(String movieTitle)
        {
            var client = new RestClient("http://www.omdbapi.com/");
            var request = new RestRequest("/", Method.GET)
                .AddQueryParameter("apikey", this.ApiKey)
                .AddQueryParameter("r", "json")
                .AddQueryParameter("s", movieTitle);

            var response = await client.ExecuteWithExceptionHandlingAsync(request);
            return response.Content;
        }
    }
}