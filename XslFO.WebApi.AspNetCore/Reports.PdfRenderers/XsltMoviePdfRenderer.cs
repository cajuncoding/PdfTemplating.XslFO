using System;
using PdfTemplating.SystemCustomExtensions;
using PdfTemplating.XslFO.Xslt;
using PdfTemplating.XslFO;
using System.Reflection;
using XslFO.Samples.MovieSearchService;

namespace PdfTemplating.AspNetCoreMvc.Reports.PdfRenderers
{
    /// <summary>
    /// This class implements both the sync and sync interfaces so that it can illustrate side-by-side the legacy Fonet (sync),
    /// and teh new ApacheFOP.Serverless (async) approaches.
    /// </summary>
    public class XsltMoviePdfRenderer : BaseXsltPdfRenderingTemplate<MovieSearchResponse>, IPdfTemplatingRenderer<MovieSearchResponse>, IAsyncPdfTemplatingRenderer<MovieSearchResponse>
    {
        private readonly ApacheFOPServerlessHelperClient _apacheFopHelperClient;

        public XsltMoviePdfRenderer(ApacheFOPServerlessHelperClient apacheFopHelperClient)
        {
            _apacheFopHelperClient = apacheFopHelperClient.AssertArgumentIsNotNull(nameof(apacheFopHelperClient));

            //Initialize the fully qualified path to the Razor View that is linked to this specific Pdf Templating Renderer!
            //var fullyQualifiedPath = HttpContext.Current.Server.MapPath("~/Reports.Xslt/MoviePdfReport/MoviesReport.xsl");
            var fullyQualifiedPath = Path.GetFullPath("./Reports.Xslt/MoviePdfReport/MoviesReport.xsl");
            var fileInfo = new FileInfo(fullyQualifiedPath);

            //Initialize the Generic Base class...
            base.InitializeBase(fileInfo);
        }

        /// <summary>
        /// Implements the IPdfTemplatingRenderer interface with FO.NET in-memory rendering engine for illustration purposes!
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
            var xslfoContent = this.RenderXslFOContent(templateModel);

            //Create the Pdf Options for the XSL-FO Rendering engine to use
            var pdfOptions = new XslFOPdfOptions()
            {
                Author = Assembly.GetExecutingAssembly()?.GetName()?.Name ?? "PdfTemplating Renderer",
                Title = $"Xsl-FO Pdf Templating Renderer [{this.GetType().Name}]",
                Subject = $"Dynamic Xslt Generated Xsl-FO Pdf Document [{DateTime.Now}]",
                //SET the Base Directory for XslFO Images, Xslt Imports, etc.
                BaseDirectory = this.XsltFileInfo.Directory,
                EnableAdd = false,
                EnableCopy = true,
                EnableModify = false,
                EnablePrinting = true,
            };

            //****************************************************************************
            //Execute the Transformation of the XSL-FO source to Binary Pdf via Fonet
            //****************************************************************************
            var xslFOPdfRenderer = new FONetXslFOPdfRenderer(pdfOptions);
            var pdfBytes = xslFOPdfRenderer.RenderPdfBytes(xslfoContent);
            return pdfBytes;
        }

        /// <summary>
        /// Implements the IAsyncPdfTemplatingRenderer interface with ApacheFOP.Serverless (pdf-as-a-service) rendering
        /// engine for illustration purposes!
        /// NOTE: This method orchestrates all logic to create the view model, execute the view template,
        ///         and render the XSL-FO output, and then convert that XSL-FO output to a valid Pdf
        ///         in one and only place and greatly simplifies all Razor View Renderer implementations to keep
        ///         code very DRY.
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        public virtual async Task<byte[]> RenderPdfAsync(MovieSearchResponse templateModel)
        {
            //Ensure that compatibility Mode is Disabled for proper rendering of our Model
            templateModel.FonetCompatibilityEnabled = false;

            //***********************************************************
            //Execute the XSLT Transform to generate the XSL-FO output
            //***********************************************************
            //Render the XSL-FO output from the Razor Template and the View Model
            var xslFODoc = this.RenderXslFOContent(templateModel);

            //******************************************************************************************
            //Execute the Transformation of the XSL-FO source to Binary Pdf via Apache FOP Service...
            //******************************************************************************************
            var pdfBytes = await _apacheFopHelperClient.RenderXslFOToPdfAsync(xslFODoc).ConfigureAwait(false);
            return pdfBytes;
        }

    }
}