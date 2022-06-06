/*
Copyright 2012 Brandon Bernard

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using PdfTemplating;
using PdfTemplating.AspNetMvc.Reports.PdfRenderers;
using XslFO.Samples.MovieSearchService;

namespace AspNetCoreMvc.Controllers
{
    [RoutePrefix("movies/pdf")]
    public class MoviePdfController : Controller
    {
        [Route]
        public async Task<ActionResult> Index(String title = "Star Wars", bool useRazor = true)
        {
            if (useRazor)
                return await PdfWithRazor(title);
            else
                return await PdfWithXslt(title);
        }

        [Route("razor")]
        public async Task<ActionResult> PdfWithRazor(String title = "Star Wars")
        {
            try 
            { 
                var searchResponse = await ExecuteMovieSearchHelperAsync(title);

                //*******************************************
                // RAZOR + Fonet (synchronous; embedded code)
                //*******************************************
                var pdfRenderer = new RazorMoviePdfRenderer(ControllerContext);
                var pdfBytes = pdfRenderer.RenderPdf(searchResponse);

                //Create the File Content Result from the Pdf byte data
                return new FileContentResult(pdfBytes, WebContentType.Pdf);
            }
            catch (Exception exc)
            {
                //Since the Apache FOP Service provides helpful errors on syntax issues, we want to let
                //  those details bubble up to the caller for troubleshooting...
                return CreateJsonExceptionResult(exc);
            }

        }

        [Route("razor/apache-fop")]
        public async Task<ActionResult> PdfWithRazorAndApacheFOP(String title = "Star Wars")
        {
            try
            {
                var searchResponse = await ExecuteMovieSearchHelperAsync(title);

                //*******************************************
                // RAZOR + Apace FOP (async I/O request)
                //*******************************************
                var pdfRenderer = new RazorMoviePdfRenderer(ControllerContext);
                var pdfBytes = await pdfRenderer.RenderPdfAsync(searchResponse).ConfigureAwait(false);

                //Create the File Content Result from the Pdf byte data
                return new FileContentResult(pdfBytes, WebContentType.Pdf);
            }
            catch (Exception exc)
            {
                //Bubble up the Error as Json for additional Details
                return CreateJsonExceptionResult(exc);
            }        
        }

        [Route("xslt")]
        public async Task<ActionResult> PdfWithXslt(String title = "Star Wars")
        {
            try
            {
                var searchResponse = await ExecuteMovieSearchHelperAsync(title);

                //*******************************************
                // XSLT + Fonet (synchronous; embedded code)
                //*******************************************
                var pdfRenderer = new XsltMoviePdfRenderer();
                var pdfBytes = pdfRenderer.RenderPdf(searchResponse);

                //Create the File Content Result from the Pdf byte data
                return new FileContentResult(pdfBytes, WebContentType.Pdf);
            }
            catch (Exception exc)
            {
                //Bubble up the Error as Json for additional Details
                return CreateJsonExceptionResult(exc);
            }

        }

        [Route("xslt/apache-fop")]
        public async Task<ActionResult> PdfWithXsltAndApacheFOP(String title = "Star Wars")
        {
            try
            {
                var searchResponse = await ExecuteMovieSearchHelperAsync(title);

                //*******************************************
                // XSLT + Apache FOP (async I/O request)
                //*******************************************
                var pdfRenderer = new XsltMoviePdfRenderer();
                var pdfBytes = await pdfRenderer.RenderPdfAsync(searchResponse).ConfigureAwait(false);

                //Create the File Content Result from the Pdf byte data
                return new FileContentResult(pdfBytes, WebContentType.Pdf);
            }
            catch (Exception exc)
            {
                //Bubble up the Error as Json for additional Details
                return CreateJsonExceptionResult(exc);
            }
        }

        #region Helpers

        private async Task<MovieSearchResponse> ExecuteMovieSearchHelperAsync(String title)
        {
            //Execute the Move Search to load data . . . 
            //NOTE: This can come from any source, and can be from converted JSON or Xml, etc.
            //NOTE: As an interesting use case here, we load the results dynamically from a Movie Search
            //      Database REST call for JSON results, and convert to Xml dynamically to use efficiently
            //      with our templates.
            var movieSearchService = new MovieSearchService();
            var searchResponse = await movieSearchService.SearchAsync(title).ConfigureAwait(false);
            return searchResponse;
        }

        private ContentResult CreateJsonExceptionResult(Exception exc)
        {
            var exceptionJson = JsonConvert.SerializeObject(exc);
            var resultContent = Content(exceptionJson, ContentTypes.Json);
            return resultContent;
        }

        #endregion
    }


}