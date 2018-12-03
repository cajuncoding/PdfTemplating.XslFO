using MVC.Templating;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.Linq.XslFO.CustomExtensions;
using XslFO.WebMVC.MovieSearch;

namespace XslFO.WebMVC.PdfRenderers
{
    public class RazorMoviePdfRenderer : BaseReportPdfRenderer, IMoviePdfRenderer
    {
        protected MvcRazorViewRenderer razorViewRenderer;

        public RazorMoviePdfRenderer(ControllerContext controllerContext)
        {
            this.razorViewRenderer = new MvcRazorViewRenderer(controllerContext);
        }

        public byte[] RenderPdf(MovieSearchResponse searchResponse)
        {
            //Load the Razor View Template to process the Model data
            //NOTE: This Template must generate valid Xsl-FO (well formed xml) output to be rendered as a Pdf Binary!
            //NOTE: WE must map the path from the Application Root "~/" to find the Local Files deployed with our MVC app!
            var razorViewPath = "~/Reports.Razor/MoviePdfReport/MoviesReport.cshtml";
            var razorViewFileInfo = new FileInfo(HttpContext.Current.Server.MapPath(razorViewPath));

            //Execute the Razor Template to generate Xsl-FO Source
            var xslFOTextOutput = this.razorViewRenderer.RenderViewToString(razorViewPath, searchResponse);
            var xslFODoc = XDocument.Parse(xslFOTextOutput);

            //Initialize the Pdf rendering options for the XSL-FO Pdf Engine
            var pdfOptions = new XslFOPdfOptions()
            {
                Author = "BBernard",
                Title = "Xsl-FO Test Application",
                Subject = $"Dynamic Razor Template Generated Xsl-FO Pdf Document [{DateTime.Now}]",
                //SET the Base Directory for XslFO Images, Xslt Imports, etc.
                BaseDirectory = razorViewFileInfo.Directory,
                EnableAdd = false,
                EnableCopy = true,
                EnableModify = false,
                EnablePrinting = true,
            };

            //***********************************************************
            //Execute the XSL-FO Engine to generate PDF Output
            //***********************************************************
            var pdfBytes = this.RenderXslFOPdfBytes(xslFODoc, pdfOptions);
            return pdfBytes;
        }
    }
}