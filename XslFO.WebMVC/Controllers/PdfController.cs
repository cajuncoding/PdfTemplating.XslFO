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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.IO;
using System.IO.CustomExtensions;
using System.Diagnostics;

using System.Xml.Linq;
using System.Xml.Linq.CustomExtensions;
using System.Xml.Linq.XslFO.CustomExtensions;

using Fonet;
using RestSharp;
using RestSharp.CustomExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XslFO.WebMVC.Controllers
{
    public class PdfController : Controller
    {
        public async Task<ActionResult> Index(String id)
        {
            return await ByTitle(id ?? "Star Wars");
        }

        public async Task<ActionResult> ByTitle(String id)
        {
            try
            {
                //Load the Xml Data
                //NOTE: This can come from any source, and even be JSON converted to Xml.
                //NOTE: As an interesting use case here, we load the results dynamically from a Movie Search
                //      Database REST call for JSON results, and convert to Xml dynamically to use efficiently
                //      with our Xslt Report.
                var xmlDataDoc = await GetXmlAsync(id);

                //Load the Xslt Report to process the Xml data
                //NOTE: This report must generate valid Xsl-FO (well formed xml) output to be rendered as a Pdf Binary!
                //NOTE: WE must map the path from the Application Root "~/" to find the Local Files deployed with our MVC app!
                var xsltFileInfo = new FileInfo(Server.MapPath(@"~/XslFO.Reports/MoviePdfReport/MoviesReport.xsl"));

                using (var xslFORenderOutput = RenderXslFOPdf(xmlDataDoc, xsltFileInfo))
                {
                    //Get the MemoryStream reference from the Rendered Output
                    var memoryStream = xslFORenderOutput.PdfStream;
                    //Read the Binary Byte array from the Memory Stream using the Custom Extesions on MemoryStream
                    var pdfBytes = memoryStream.ReadBinary();

                    //Creat the MVC File Content Result from the Pdf Byte data
                    var fileContent = new FileContentResult(pdfBytes, "application/pdf");
                    return fileContent;
                }

                //return Json(json, JsonRequestBehavior.AllowGet);
                //return Content(xmlDataDoc.ToString(), "text/xml");
            }
            catch(HttpException exc)
            {
                return Json(exc);
            }
        }

        protected XslFORenderStreamOutput RenderXslFOPdf(XDocument xmlDoc, FileInfo xsltFileInfo)
        {
            //***********************************************************
            //Initialize and Compile the XsltTransformer
            //***********************************************************
            var pdfOptions = new XslFOPdfOptions()
            {
                Author = "BBernard",
                Title = "Xsl-FO Test Application",
                Subject = $"Dynamically Xslt Generated Xsl-FO Pdf Document [{DateTime.Now}]",
                //SET the Base Directory for XslFO Images, Xslt Imports, etc.
                BaseDirectory = xsltFileInfo.Directory, 
                EnableAdd = false,
                EnableCopy = true,
                EnableModify = false,
                EnablePrinting = true,
            };

            //Load the Xslt Source Code (e.g. Must be valid Xml)
            var xsltDoc = XDocument.Load(xsltFileInfo.FullName);

            //Create the Xslt Transform Engine helper with Custom Extension methods on XDocument
            //NOTE: We use the enhanced Xml Url Resolver to support importing Xslt Common libraries
            var xmlResolver = new XmlUrlExtendedResolver(pdfOptions.BaseDirectory);
            XslTransformEngine xslTransformer = xsltDoc.CreateXslTransformEngine(new XslTransformEngineOptions()
            {
                XsltDocumentResolver = xmlResolver,
                XsltLoadResolver = xmlResolver
            });

            //***********************************************************
            //Execute the Xslt Transformation to generate Xsl-FO Source
            //***********************************************************
            var xslFODoc = xslTransformer.TransformToXDocument(xmlDoc);

            //***********************************************************
            //Render the Xsl-FO results into a Pdf binary output
            //***********************************************************
            //Initialize an Event Handler to process events raised by FONET; we use the same handler for all events.
            var fnFonetEventHandler = new EventHandler<FonetEventArgs>((sender, e) =>
            {
                Debug.WriteLine($"[XslFORenderer] {e.GetMessage()}");
            });

            //Initialize the render options for the FONET process and Execute the Render 
            //  using the Custom Extensions on XDocument
            var xslFORenderedOutput = xslFODoc.RenderXslFOToPdf(new XslFORenderOptions()
            {
                PdfOptions = pdfOptions,
                RenderErrorHandler = fnFonetEventHandler,
                RenderEventHandler = fnFonetEventHandler
            });

            return xslFORenderedOutput;
        }


        protected async Task<XDocument> GetXmlAsync(String id)
        {
            //Get the JSON Content from REST Response
            var content = await ExecuteMovieQueryRESTUri(id);

            //Dynamically convert directly from JSON String to Xml
            //NOTE: This DOES not use intermediate String serialization/deserialization so it will be performant!
            var xdocFromJson = JsonConvert.DeserializeXNode(content, "Results");
            xdocFromJson.Root.SetAttributeValue("id", id);
            xdocFromJson.Root.SetAttributeValue("dateTime", DateTime.Now);

            //Return the final Xml result
            return xdocFromJson;
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
                .AddQueryParameter("apikey", "e915adab")
                .AddQueryParameter("r", "json")
                .AddQueryParameter("s", movieTitle);

            var response = await client.ExecuteWithExceptionHandlingAsync(request);
            return response.Content;
        }

    }
}