using PdfTemplating.XslFO.ApacheFOP.Serverless;
using System.CustomExtensions;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PdfTemplating.AspNetMvc.Reports.PdfRenderers
{
    public class ApacheFOPServerlessHelper
    {
        public static Task<byte[]> RenderXslFOToPdfAsync(XDocument xslFODoc)
            => RenderXslFOToPdfAsync(xslFODoc.ToString());

        public static async Task<byte[]> RenderXslFOToPdfAsync(string xslfoContent)
        {
            //************************************************************************************************
            //Execute the Transformation of the XSL-FO source to Binary Pdf via ApacheFOP Serverless Rendering
            //************************************************************************************************
            var apacheFOPServiceHostString = ConfigurationManager
                .AppSettings["XslFO.ApacheFOP.Serverless.Host"]
                .AssertArgumentIsNotNullOrBlank("XslFO.ApacheFOP.Serverless.Host", "Configuration value for ApacheFOP Service Host is missing or undefined.");

            var gzipRequestsEnabled = ConfigurationManager
                .AppSettings["XslFO.ApacheFOP.Serverless.GzipRequestsEnabled"]
                ?.EqualsIgnoreCase(bool.TrueString) ?? false;

            var gzipResponsesEnabled = ConfigurationManager
                .AppSettings["XslFO.ApacheFOP.Serverless.GzipResponsesEnabled"]
                ?.EqualsIgnoreCase(bool.TrueString) ?? false;

            //Construct the REST request options and append the Security Token (as QuerystringParam):
            var options = new ApacheFOPServerlessXslFORenderOptions(new Uri(apacheFOPServiceHostString))
            {
                EnableGzipCompressionForRequests = gzipRequestsEnabled,
                EnableGzipCompressionForResponses = gzipResponsesEnabled
            };
 
            var xslFOPdfRenderer = new ApacheFOPServerlessPdfRenderService(xslfoContent, options);
            var pdfBytes = await xslFOPdfRenderer.RenderPdfBytesAsync().ConfigureAwait(false);
            
            return pdfBytes;
        }

    }
}