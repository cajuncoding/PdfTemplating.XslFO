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
    public class ApacheFOPServiceHelper
    {
        public static async Task<byte[]> RenderXslFOToPdfAsync(XDocument xslFODoc)
        {
            //************************************************************************************************
            //Execute the Transformation of the XSL-FO source to Binary Pdf via ApacheFOP Serverless Rendering
            //************************************************************************************************
            var apacheFOPServiceHostString = ConfigurationManager
                .AppSettings["XslFO.ApacheFOP.Serverless.Host"]
                .AssertArgumentIsNotNullOrBlank("XslFO.ApacheFOP.Serverless.Host", "Configuration value for ApacheFOP Service Host is missing or undefined.");

            var apacheFOPServiceToken = ConfigurationManager
                .AppSettings["XslFO.ApacheFOP.Serverless.Token"]
                .AssertArgumentIsNotNullOrBlank("XslFO.ApacheFOP.Serverless.Token", "Configuration value for ApacheFOP Service Host is missing or undefined.");


            //Construct the REST request options and append the Security Token (as QuerystringParam):
            var options = new ApacheFOPServerlessXslFORenderOptions(new Uri(apacheFOPServiceHostString));
            options.QuerystringParams["code"] = apacheFOPServiceToken;

            var xslFOPdfRenderer = new ApacheFOPServerlessPdfRenderService(xslFODoc, options);

            var pdfBytes = await xslFOPdfRenderer.RenderPdfBytesAsync();
            
            return pdfBytes;
        }

    }
}