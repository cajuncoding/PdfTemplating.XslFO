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

using RestSharp;
using RestSharp.CustomExtensions;
using System;
using System.CustomExtensions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PdfTemplating.XslFO.ApacheFOP.Serverless
{
    /// <summary>
    /// BBernard
    /// Class implementation of IXslFOPdfRenderer to abstract the Extension Method use when Interface 
    /// Usage pattern is desired.
    /// NOTE: This may be more suitable, than direct Custom Extension use, for code patterns that use 
    ///         Dependency Injection, etc.
    /// </summary>
    public class XslFOPdfRenderService : IAsyncXslFOPdfRenderer
    {
        public XDocument XslFODocument { get; set; }
        public XslFORenderOptions XslFOPdfRenderOptions { get; set; }

        public XslFOPdfRenderService(XDocument xslFODoc, Uri apacheFOPServiceHostUri, string apacheFOPServiceApiPath = "")
        {
            this.XslFODocument = xslFODoc.AssertArgumentIsNotNull(nameof(xslFODoc), "Valid XSL-FO Xml source document must be specified.");
            
            //NOTE: The Render Options class will validate it's own parameters.
            this.XslFOPdfRenderOptions = new XslFORenderOptions(apacheFOPServiceHostUri);

        }


        public XslFOPdfRenderService(XDocument xslFODoc, XslFORenderOptions xslFOPdfRenderOptions)
        {
            this.XslFODocument = xslFODoc.AssertArgumentIsNotNull(nameof(xslFODoc), "Valid XSL-FO Xml source document must be specified.");
            this.XslFOPdfRenderOptions = xslFOPdfRenderOptions.AssertArgumentIsNotNull(nameof(xslFOPdfRenderOptions), "XSL-FO Render options must be specified.");
        }

        public async Task<byte[]> RenderPdfBytesAsync()
        {
            //***********************************************************
            //Render the Xsl-FO source into a Pdf binary output
            //***********************************************************
            //Initialize the Xsl-FO microservice via configuration...
            var restClient = new RestClient(XslFOPdfRenderOptions.ApacheFOPServiceHost);

            //Get the Raw Xml Source for our Xsl-FO to be tansformed into Pdf binary...
            var xslFoSource = this.XslFODocument.ToString();


            //Create the REST request for the Apache FOP micro-service...
            var restRequest = new RestRequest(XslFOPdfRenderOptions.ApacheFOPApi, Method.POST);
            restRequest.AddRawTextBody(xslFoSource, ContentType.Xml);

            //Execute the request to the service, validate, and retrieve the Raw Binary resposne...
            var restResponse = await restClient.ExecuteWithExceptionHandlingAsync(restRequest);

            var pdfBytes = restResponse.RawBytes;
            return pdfBytes;            
        }
    }
}
