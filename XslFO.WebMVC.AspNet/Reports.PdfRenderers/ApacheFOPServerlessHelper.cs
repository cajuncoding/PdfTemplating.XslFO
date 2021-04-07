using PdfTemplating.XslFO.ApacheFOP.Serverless;
using System.CustomExtensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace XslFO.WebMvc.Reports.PdfRenderers
{
    public class ApacheFOPServerlessHelper
    {
        public static async Task<byte[]> RenderXslFOToPdfAsync(XDocument xslFODoc)
        {
            //************************************************************************************************
            //Execute the Transformation of the XSL-FO source to Binary Pdf via ApacheFOP Serverless Rendering
            //************************************************************************************************
            var apacheFOPServiceHostString = ConfigurationManager
                .AppSettings["XslFO.ApacheFOP.Serverless.Host"]
                .AssertArgumentIsNotNullOrBlank("XslFO.ApacheFOP.Serverless.Host", "Configuration value for ApacheFOP Service Host is missing or undefined.");

            var gzipRequestsEnabled = ConfigurationManager
                .AppSettings["XslFO.ApacheFOP.Serverless.GzipRequestsEnabled"]?.EqualsIgnoreCase(bool.TrueString) ?? false;

            var gzipResponsesEnabled = ConfigurationManager
                .AppSettings["XslFO.ApacheFOP.Serverless.GzipResponsesEnabled"]?.EqualsIgnoreCase(bool.TrueString) ?? false;

            //Construct the REST request options and append the Security Token (as QuerystringParam):
            var options = new ApacheFOPServerlessXslFORenderOptions(new Uri(apacheFOPServiceHostString))
            {
                EnableGzipCompressionForRequests = gzipRequestsEnabled,
                EnableGzipCompressionForResponses = gzipResponsesEnabled
            };
 
            var xslFOPdfRenderer = new ApacheFOPServerlessPdfRenderService(xslFODoc, options);
            var pdfBytes = await xslFOPdfRenderer.RenderPdfBytesAsync().ConfigureAwait(false);
            
            return pdfBytes;
        }

    }
}