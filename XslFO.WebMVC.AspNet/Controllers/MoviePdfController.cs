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
using PdfTemplating.WebMVC.MovieSearch;
using PdfTemplating.XslFO;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using XslFO.WebMVC.Reports.PdfRenderers.Fonet;
using XslFO.WebMVC.Reports.PdfRenderers.ApacheFOP;
using XslFO.WebMVC.Reports.PdfRenderers.ApacheFOP.Serverless;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using RestSharp.CustomExtensions;

namespace PdfTemplating.WebMVC.Controllers
{
    [RoutePrefix("movies/pdf")]
    public class MoviePdfController : Controller
    {
        private const string MIME_TYPE_PDF = "application/pdf";

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
                var searchResponse = await ExecuteMovieSearchHelper(title);

                //*******************************************
                // RAZOR + Fonet (synchronous; embedded code)
                //*******************************************
                var pdfRenderer = new RazorMoviePdfRenderer(ControllerContext);
                var pdfBytes = pdfRenderer.RenderPdf(searchResponse);

                //Create the File Content Result from the Pdf byte data
                return new FileContentResult(pdfBytes, MIME_TYPE_PDF);
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
                var searchResponse = await ExecuteMovieSearchHelper(title);

                //*******************************************
                // RAZOR + Apace FOP (async I/O request)
                //*******************************************
                var pdfRenderer = new RazorMoviePdfRenderer(ControllerContext);
                var pdfBytes = await pdfRenderer.RenderPdfAsync(searchResponse);

                //Create the File Content Result from the Pdf byte data
                return new FileContentResult(pdfBytes, MIME_TYPE_PDF);
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
                var searchResponse = await ExecuteMovieSearchHelper(title);

                //*******************************************
                // XSLT + Fonet (synchronous; embedded code)
                //*******************************************
                var pdfRenderer = new XsltMoviePdfRenderer();
                var pdfBytes = pdfRenderer.RenderPdf(searchResponse);

                //Create the File Content Result from the Pdf byte data
                return new FileContentResult(pdfBytes, MIME_TYPE_PDF);
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
                var searchResponse = await ExecuteMovieSearchHelper(title);

                //*******************************************
                // XSLT + Apache FOP (async I/O request)
                //*******************************************
                var pdfRenderer = new XsltMoviePdfRenderer();
                var pdfBytes = await pdfRenderer.RenderPdfAsync(searchResponse);

                //Create the File Content Result from the Pdf byte data
                return new FileContentResult(pdfBytes, MIME_TYPE_PDF);
            }
            catch (Exception exc)
            {
                //Bubble up the Error as Json for additional Details
                return CreateJsonExceptionResult(exc);
            }
        }

        #region Helpers

        private async Task<MovieSearchResponse> ExecuteMovieSearchHelper(String title)
        {
            //Execute the Move Search to load data . . . 
            //NOTE: This can come from any source, and can be from converted JSON or Xml, etc.
            //NOTE: As an interesting use case here, we load the results dynamically from a Movie Search
            //      Database REST call for JSON results, and convert to Xml dynamically to use efficiently
            //      with our templates.
            var movieSearchService = new MovieSearchService();
            var searchResponse = await movieSearchService.SearchAsync(title);
            return searchResponse;
        }

        private ContentResult CreateJsonExceptionResult(Exception exc)
        {
            var exceptionJson = JsonConvert.SerializeObject(exc);
            var resultContent = Content(exceptionJson, ContentType.Json);
            return resultContent;
        }

        #endregion
    }


}