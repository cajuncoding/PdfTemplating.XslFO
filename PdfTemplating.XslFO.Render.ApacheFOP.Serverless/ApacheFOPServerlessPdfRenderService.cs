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
    public class ApacheFOPServerlessPdfRenderService : IAsyncXslFOPdfRenderer
    {
        public XDocument XslFODocument { get; set; }
        public ApacheFOPServerlessXslFORenderOptions ApacheFopServerlessXslFoPdfRenderOptions { get; set; }

        public ApacheFOPServerlessPdfRenderService(XDocument xslFODoc, ApacheFOPServerlessXslFORenderOptions apacheFopServerlessXslFoPdfRenderOptions)
        {
            this.XslFODocument = xslFODoc.AssertArgumentIsNotNull(nameof(xslFODoc), "Valid XSL-FO Xml source document must be specified.");
            this.ApacheFopServerlessXslFoPdfRenderOptions = apacheFopServerlessXslFoPdfRenderOptions.AssertArgumentIsNotNull(nameof(apacheFopServerlessXslFoPdfRenderOptions), "XSL-FO Render options must be specified.");
        }

        public async Task<byte[]> RenderPdfBytesAsync()
        {
            //***********************************************************
            //Render the Xsl-FO source into a Pdf binary output
            //***********************************************************
            //Initialize the Xsl-FO micro-service via configuration...
            var restClient = new RestClient(ApacheFopServerlessXslFoPdfRenderOptions.ApacheFOPServiceHost);

            //Get the Raw Xml Source for our Xsl-FO to be transformed into Pdf binary...
            var xslFoSource = this.XslFODocument.ToString();

            //Create the REST request for the Apache FOP micro-service...
            var restRequest = new RestRequest(ApacheFopServerlessXslFoPdfRenderOptions.ApacheFOPApi, Method.POST);
            restRequest.AddRawTextBody(xslFoSource, ContentType.Xml);

            //Append Request Headers if defined...
            foreach (var item in this.ApacheFopServerlessXslFoPdfRenderOptions.RequestHeaders)
            {
                restRequest.AddHeader(item.Key, item.Value);
            }

            //Append Request Querystring Params if defined...
            foreach (var item in this.ApacheFopServerlessXslFoPdfRenderOptions.QuerystringParams)
            {
                restRequest.AddQueryParameter(item.Key, item.Value);
            }

            //Execute the request to the service, validate, and retrieve the Raw Binary response...
            var restResponse = await restClient.ExecuteWithExceptionHandlingAsync(restRequest);

            var pdfBytes = restResponse.RawBytes;
            return pdfBytes;            
        }
    }
}
