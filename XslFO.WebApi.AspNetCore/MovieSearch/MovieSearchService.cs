using Flurl;
using Flurl.Http;

namespace PdfTemplating.AspNetCoreMvc.MovieSearch
{
    public class MovieSearchService
    {
        public String ApiKey { get; protected set; } = "e915adab";

        public async Task<MovieSearchResponse> SearchAsync(String title)
        {
            var searchResponse = await ExecuteMovieQueryRESTUri(title).ConfigureAwait(false);
            
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
        protected async Task<MovieSearchResponse> ExecuteMovieQueryRESTUri(String movieTitle)
        {
            var movieResponse = await "http://www.omdbapi.com/"
                .SetQueryParam("apikey", this.ApiKey)
                .SetQueryParam("r", "json")
                .SetQueryParam("s", movieTitle)
                .GetJsonAsync<MovieSearchResponse>()
                .ConfigureAwait(false);

            return movieResponse;
        }
    }
}