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
using System.IO.CustomExtensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PdfTemplating.XslFO.Render.ApacheFOP.Serverless;

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
        public ApacheFOPServerlessXslFORenderOptions Options { get; set; }

        public ApacheFOPServerlessPdfRenderService(XDocument xslFODoc, ApacheFOPServerlessXslFORenderOptions apacheFopServerlessXslFoPdfRenderOptions)
        {
            this.XslFODocument = xslFODoc.AssertArgumentIsNotNull(nameof(xslFODoc), "Valid XSL-FO Xml source document must be specified.");
            this.Options = apacheFopServerlessXslFoPdfRenderOptions.AssertArgumentIsNotNull(nameof(apacheFopServerlessXslFoPdfRenderOptions), "XSL-FO Render options must be specified.");
        }

        public virtual async Task<ApacheFOPServerlessResponse> RenderPdfAsync()
        {
            //***********************************************************
            //Render the Xsl-FO source into a Pdf binary output
            //***********************************************************
            //Initialize the Xsl-FO micro-service via configuration...
            var restClient = new RestClient(Options.ApacheFOPServiceHost);

            //Get the Raw Xml Source for our Xsl-FO to be transformed into Pdf binary...
            var xslFoSource = this.XslFODocument.ToString();

            //Create the RESTSharp Request...
            var restRequest = await CreateRestRequest(xslFoSource).ConfigureAwait(false);

            //Execute the request to the service, validate, and retrieve the Raw Binary response...
            var restResponse = await restClient.ExecuteWithExceptionHandlingAsync(restRequest).ConfigureAwait(false);

            //Read Response Headers to return...
            var headersDictionary = restResponse.GetHeadersAsDictionary();

            //Determine if the Response was GZIP Encoded and handle properly to get the valid binary Pdf Byte Array...
            bool isResponseGzipEncoded = headersDictionary.TryGetValue(ApacheFOPServerlessHeaders.ContentEncoding, out string contentEncoding)
                                             && contentEncoding.IndexOf(ApacheFOPServerlessEncodings.GzipEncoding, StringComparison.OrdinalIgnoreCase) >= 0;

            //Read teh Pdf Bytes and return the Apache FOP Serverless response...
            var pdfBytes = isResponseGzipEncoded
                ? await restResponse.RawBytes.GzipDecompressAsync()
                : restResponse.RawBytes;

            var apacheServerlessResponse = new ApacheFOPServerlessResponse(pdfBytes, headersDictionary);
            return apacheServerlessResponse;            
        }

        public virtual async Task<IRestRequest> CreateRestRequest(string xslFoSource)
        {
            //Create the REST request for the Apache FOP micro-service...
            var restRequest = new RestRequest(Options.ApacheFOPApi, Method.POST);

            //Always add the custom header denoting the content type of the payload for processing (e.g. Always Xml)
            //NOTE: We provide this as as a fallback for reference if ever needed because ApacheFop.Serverless Azure Function will now
            //      only receive raw bytes (lowest common denominator format for supporting both GZIP & String payloads), and therefore the normal 
            //      ContentType header must be set to Binary Octet-Stream for proper handling by Azure Functions (or exceptions are thrown).
            restRequest.AddHeader(ApacheFOPServerlessHeaders.ApacheFopServerlessContentType, ContentType.Xml);

            //If specified handle GZIP Compression for the Request, otherwise just get the Raw Byte String...
            byte[] xslFoPayloadBytes;
            if (Options.EnableGzipCompressionForRequests)
            {
                //When sending GZip payload we must supply the proper Content-Encoding header to denote the compressed payload...
                // More info here: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Encoding
                xslFoPayloadBytes = await xslFoSource.GzipCompressAsync().ConfigureAwait(false);
                restRequest.AddHeader(ApacheFOPServerlessHeaders.ContentEncoding, ApacheFOPServerlessEncodings.GzipEncoding);
            }
            else
            {
                //Otherwise we supply the raw Bytes for the Xml source text...
                xslFoPayloadBytes = xslFoSource.GetBytesUtf8();
            }

            //Because ApacheFop.Serverless now supports both GZIP and String payloads the Azure Function will only recieve
            //  the raw bytes (lowest common denominator format), and therefore we must specify the the ContentType for binary payload
            //NOTE: That's why we also supply the custom header (above) for ApacheFopXslFoContentType (as a fallback for reference if ever needed).
            //NOTE: Binary Request Body MUST specify the proper Content-Type of Application/Octet-Stream
            //      More info. on resolving this for Java Azure Functions here:
            //      https://github.com/Azure/azure-functions-java-worker/issues/44#issuecomment-358068266
            restRequest.AddRawBytesBody(xslFoPayloadBytes, ContentType.BinaryOctetStream);

            //Append Request Headers if defined...
            foreach (var item in this.Options.RequestHeaders)
            {
                restRequest.AddHeader(item.Key, item.Value);
            }

            //Manually add special case Gzip Compression Header for Responses if not already defined (e.g. we Accept GZIP Encoding as a response)...
            if (Options.EnableGzipCompressionForResponses && !this.Options.RequestHeaders.ContainsKey(ApacheFOPServerlessHeaders.AcceptEncoding))
            {
                restRequest.AddHeader(ApacheFOPServerlessHeaders.AcceptEncoding, ApacheFOPServerlessEncodings.GzipEncoding);
            }

            //Append Request Querystring Params if defined...
            foreach (var item in this.Options.QuerystringParams)
            {
                restRequest.AddQueryParameter(item.Key, item.Value);
            }

            return restRequest;
        }

        /// <summary>
        /// Implements the simplified interface for only rendering and returning only the binary Pdf Byte array.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<byte[]> RenderPdfBytesAsync()
        {
            var response = await RenderPdfAsync().ConfigureAwait(false);
            return response.PdfBytes;
        }
    }
}
