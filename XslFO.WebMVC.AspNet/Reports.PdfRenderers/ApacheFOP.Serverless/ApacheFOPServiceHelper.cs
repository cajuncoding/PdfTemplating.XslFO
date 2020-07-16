using PdfTemplating.XslFO.ApacheFOP.Serverless;
using System.CustomExtensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace XslFO.WebMVC.Reports.PdfRenderers.ApacheFOP.Serverless
{
    public class ApacheFOPServiceHelper
    {
        public static async Task<byte[]> RenderXslFOToPdfAsync(XDocument xslFODoc)
        {
            //****************************************************************************
            //Execute the Trasnformation of the XSL-FO source to Binary Pdf via Fonet
            //****************************************************************************
            var apacheFOPServiceHostString = ConfigurationManager
                                                .AppSettings["XslFO.ApacheFOP.Service.Host"]
                                                .AssertArgumentIsNotNullOrBlank("XslFO.ApacheFOP.Service.Host", "Configuration value for ApacheFOP Service Host is missing or undefined.");

            var xslFOPdfRenderer = new XslFOPdfRenderService(xslFODoc, new Uri(apacheFOPServiceHostString));
            var pdfBytes = await xslFOPdfRenderer.RenderPdfBytesAsync();
            return pdfBytes;
        }

    }
}