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
using XslFO.WebMVC.MovieSearch;
using XslFO.WebMVC.PdfRenderers;

namespace XslFO.WebMVC.Controllers
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
            //Execute the Move Search to load data . . . 
            //NOTE: This can come from any source, and can be from converted JSON or Xml, etc.
            //NOTE: As an interesting use case here, we load the results dynamically from a Movie Search
            //      Database REST call for JSON results, and convert to Xml dynamically to use efficiently
            //      with our templates.
            var movieSearchService = new MovieSearchService();
            var searchResponse = await movieSearchService.SearchAsync(title);

            //Initialize the appropriate Renderer based on the Parameter.
            // and execute the Pdf Renderer to generate the Pdf Document byte data
            IPdfRenderer<MovieSearchResponse> pdfRenderer = new RazorMoviePdfRenderer(ControllerContext);

            var pdfBytes = pdfRenderer.RenderPdf(searchResponse);

            //Creat the MVC File Content Result from the Pdf byte data
            var fileContent = new FileContentResult(pdfBytes, "application/pdf");
            return fileContent;
        }

        [Route("xslt")]
        public async Task<ActionResult> PdfWithXslt(String title = "Star Wars")
        {
            //Execute the Move Search to load data . . . 
            //NOTE: This can come from any source, and can be from converted JSON or Xml, etc.
            //NOTE: As an interesting use case here, we load the results dynamically from a Movie Search
            //      Database REST call for JSON results, and convert to Xml dynamically to use efficiently
            //      with our templates.
            var movieSearchService = new MovieSearchService();
            var searchResponse = await movieSearchService.SearchAsync(title);

            //Initialize the appropriate Renderer based on the Parameter.
            // and execute the Pdf Renderer to generate the Pdf Document byte data
            IPdfRenderer<MovieSearchResponse> pdfRenderer = new XsltMoviePdfRenderer();
            var pdfBytes = pdfRenderer.RenderPdf(searchResponse);

            //Creat the MVC File Content Result from the Pdf byte data
            var fileContent = new FileContentResult(pdfBytes, "application/pdf");
            return fileContent;
        }
    }


}