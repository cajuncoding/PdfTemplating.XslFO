using Flurl.Http;
using PdfTemplating.XslFO.Render.ApacheFOP.Serverless;
using System;
using System.Collections.Generic;
using PdfTemplating.SystemCustomExtensions;
using PdfTemplating.SystemIOCustomExtensions;
using PdfTemplating.SystemLinqCustomExtensions;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl;

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
        public ApacheFOPServerlessXslFORenderOptions Options { get; protected set; }

        public ApacheFOPServerlessPdfRenderService(ApacheFOPServerlessXslFORenderOptions apacheFopServerlessXslFoPdfRenderOptions)
        {
            this.Options = apacheFopServerlessXslFoPdfRenderOptions.AssertArgumentIsNotNull(nameof(apacheFopServerlessXslFoPdfRenderOptions), "XSL-FO Render options must be specified.");
        }

        public virtual async Task<ApacheFOPServerlessResponse> RenderPdfAsync(string xslfoContent, CancellationToken cancellationToken = default)
        {
            xslfoContent.AssertArgumentIsNotNullOrBlank(nameof(xslfoContent), "Valid XSL-FO Xml source document must be specified.");

            //***********************************************************
            //Render the Xsl-FO source into a Pdf binary output
            //***********************************************************
            //Initialize the Xsl-FO micro-service via configuration...
            var restRequest = CreateRestRequest();

            //Execute the request to the service, validate, and retrieve the Raw Binary response...
            var httpContent = await CreatePayloadAsync(xslfoContent).ConfigureAwait(false);
            IFlurlResponse restResponse;
            try
            {
                restResponse = await restRequest.PostAsync(httpContent, cancellationToken).ConfigureAwait(false);
            }
            catch (FlurlHttpException flurlHttpException)
            {
                var apacheFopServerlessApiException = await ApacheFOPServerlessApiException.FromFlurlHttpExceptionAsync(flurlHttpException, xslfoContent);
                throw apacheFopServerlessApiException;
            }

            //Read Response Headers to return...
            var headersDictionary = await GetResponseHeadersDictionaryAsync(restResponse).ConfigureAwait(false);

            //Determine if the Response was GZIP Encoded and handle properly to get the valid binary Pdf Byte Array...
            bool isResponseGzipEncoded = headersDictionary.TryGetValue(ApacheFOPServerlessHeaders.ContentEncoding, out string contentEncoding)
                                             && contentEncoding.IndexOf(ApacheFOPServerlessEncodings.GzipEncoding, StringComparison.OrdinalIgnoreCase) >= 0;

            //Read teh Pdf Bytes (decompressing if necessary) and return the Apache FOP Serverless response...
            var rawResponseBytes = await restResponse.GetBytesAsync().ConfigureAwait(false);
            var pdfBytes = isResponseGzipEncoded
                ? await rawResponseBytes.GzipDecompressAsync().ConfigureAwait(false)
                : rawResponseBytes;

            var apacheServerlessResponse = await ApacheFOPServerlessResponse.CreateAsync(pdfBytes, headersDictionary);
            return apacheServerlessResponse;            
        }

        /// <summary>
        /// Implements the simplified interface for only rendering and returning only the binary Pdf Byte array.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<byte[]> RenderPdfBytesAsync(string xslfoContent)
        {
            var response = await RenderPdfAsync(xslfoContent).ConfigureAwait(false);
            return response.PdfBytes;
        }

        protected virtual IFlurlRequest CreateRestRequest()
        {
            var restRequest = Options.ApacheFOPServiceHost
                .AppendPathSegment(Options.GetApacheFopApiPath())
                //Always add the custom header denoting the content type of the payload for processing (e.g. Always Xml)
                //NOTE: We provide this as as a fallback for reference if ever needed because ApacheFop.Serverless Azure Function now
                //      supports both GZIP & String payloads, and therefore the normal ContentType header may be set to Binary Octet-Stream
                //      for proper handling of GZIP requests by Azure Functions (or exceptions are thrown).
                .WithHeader(ApacheFOPServerlessHeaders.ApacheFopServerlessContentType, ContentTypes.Xml);

            //If specified handle GZIP Compression for the Request, otherwise just get the Raw Byte String...
            if (Options.EnableGzipCompressionForRequests)
            {
                restRequest
                    //When sending GZip payload we must supply the proper Content-Encoding header to denote the compressed payload...
                    // More info here: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Encoding
                    .WithHeader(ApacheFOPServerlessHeaders.ContentEncoding, ApacheFOPServerlessEncodings.GzipEncoding)
                    //Because ApacheFop.Serverless now supports both GZIP and String payloads the Azure Function will only receive
                    //  the raw bytes for GZIP requests and therefore we must specify the the ContentType for binary payload to avoid exceptions
                    //  from Azure Functions request body byte[] binding...
                    //NOTE: That's why we also supply the custom header (above) for ApacheFopXslFoContentType (as a fallback for reference if ever needed).
                    //NOTE: Binary Request Body MUST specify the proper Content-Type of Application/Octet-Stream
                    //      More info. on resolving this for Java Azure Functions here:
                    //      https://github.com/Azure/azure-functions-java-worker/issues/44#issuecomment-358068266
                    .WithHeader(ApacheFOPServerlessHeaders.ContentType, ContentTypes.BinaryOctetStream);
            }

            if (Options.RequestWaitTimeoutMillis.HasValue)
                restRequest.WithTimeout(TimeSpan.FromMilliseconds(Options.RequestWaitTimeoutMillis.Value));

            //Append Request Headers if defined...
            foreach (var item in this.Options.RequestHeaders)
                restRequest.WithHeader(item.Key, item.Value);

            //Manually add special case Gzip Compression Header for Responses if not already defined (e.g. we Accept GZIP Encoding as a response)...
            if (Options.EnableGzipCompressionForResponses && !this.Options.RequestHeaders.ContainsKey(ApacheFOPServerlessHeaders.AcceptEncoding))
                restRequest.WithHeader(ApacheFOPServerlessHeaders.AcceptEncoding, ApacheFOPServerlessEncodings.GzipEncoding);

            //Append Request Querystring Params if defined...
            foreach (var item in this.Options.QuerystringParams)
                restRequest.SetQueryParam(item.Key, item.Value);

            return restRequest;
        }

        protected virtual async Task<HttpContent> CreatePayloadAsync(string xslFoSource)
        {
            HttpContent httpContent;

            //If specified handle GZIP Compression for the Request, otherwise just get the Raw Byte String...
            if (Options.EnableGzipCompressionForRequests)
            {
                byte[] xslFoPayloadBytes = await xslFoSource.GzipCompressAsync().ConfigureAwait(false);
                httpContent = new ByteArrayContent(xslFoPayloadBytes);
            }
            else
            {
                //Otherwise we supply the raw Xml source text...
                httpContent = new StringContent(xslFoSource, Encoding.UTF8, MediaTypeNames.Text.Xml);
            }

            return httpContent;
        }

        protected virtual async Task<Dictionary<string, string>> GetResponseHeadersDictionaryAsync(IFlurlResponse restResponse)
        {
            var headers = restResponse.Headers;
            var headersDictionary = headers.ToDictionarySafely(h => h.Name, h=> h.Value);

            //NOW we post-process some special case handling of headers that might be GZIP compressed...
            if (headersDictionary.TryGetValue(ApacheFOPServerlessHeaders.ApacheFopServerlessEventLogEncoding, out var encoding)
                && encoding.EqualsIgnoreCase(ApacheFOPServerlessEncodings.GzipEncoding))
            {
                headersDictionary[ApacheFOPServerlessHeaders.ApacheFopServerlessEventLogEncoding] = ApacheFOPServerlessEncodings.IdentityEncoding;

                if (headersDictionary.TryGetValue(ApacheFOPServerlessHeaders.ApacheFopServerlessEventLog, out var eventLogHeaderValue)
                    && !string.IsNullOrEmpty(eventLogHeaderValue))
                {
                    var eventLogText = await eventLogHeaderValue.GzipDecompressBase64Async().ConfigureAwait(false);
                    //Overwrite the Compressed value with the Uncompressed value for consuming code to use...
                    headersDictionary[ApacheFOPServerlessHeaders.ApacheFopServerlessEventLog] = eventLogText;
                }
            }

            return headersDictionary;
        }
    }
}
