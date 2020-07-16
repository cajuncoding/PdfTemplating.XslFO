using PdfTemplating.WebMVC.MovieSearch;
using PdfTemplating.XslFO.Xslt;
using PdfTemplating.XslFO;
using System.IO;
using System.Web;

namespace XslFO.WebMVC.Reports.PdfRenderers.Fonet
{
    public class XsltMoviePdfRendererViaFonet : XsltPdfTemplatingRenderer<MovieSearchResponse>, IPdfTemplatingRenderer<MovieSearchResponse>
    {
        public XsltMoviePdfRendererViaFonet()
        {
            //Initialize the fully qualified path to the Razor View that is linked to this specific Pdf Templating Renderer!
            var fullyQualifiedPath = HttpContext.Current.Server.MapPath("~/Reports.Xslt/MoviePdfReport/MoviesReport.xsl");
            var fileInfo = new FileInfo(fullyQualifiedPath);

            //Initialize the Generic Base class...
            base.InitializeBase(fileInfo);
        }

        /// <summary>
        /// Implements the IRazorPdfRenderer interface and delegate the specific logic to the abstract
        /// methods to simplify the implementations of all inheriting Razor View Renderer implementations.
        /// NOTE: This method orchestrates all logic to create the view model, execute the view template,
        ///         and render the XSL-FO output, and then convert that XSL-FO output to a valid Pdf
        ///         in one and only place and greatly simplifies all Razor View Renderer implementations to keep
        ///         code very DRY.
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        public virtual byte[] RenderPdf(MovieSearchResponse templateModel)
        {
            //***********************************************************
            //Execute the XSLT Transform to generate the XSL-FO output
            //***********************************************************
            //Render the XSL-FO output from the Razor Template and the View Model
            var xslFODoc = this.RenderXslFOXml(templateModel);

            //Create the Pdf Options for the XSL-FO Rendering engine to use
            var pdfOptions = this.CreatePdfOptions();

            //****************************************************************************
            //Execute the Trasnformation of the XSL-FO source to Binary Pdf via Fonet
            //****************************************************************************
            var xslFOPdfRenderer = new FONetXslFOPdfRenderer(xslFODoc, pdfOptions);
            var pdfBytes = xslFOPdfRenderer.RenderPdfBytes();
            return pdfBytes;
        }
    }
}