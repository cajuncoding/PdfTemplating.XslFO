using System;
using System.CustomExtensions;
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
        /// NOTE: This method is strongly discouraged and is flagged as Obsolete so that consumers will use the truly Async version instead!
        ///
        /// Implements the IPdfTemplatingRenderer interface with FO.NET in-memory rendering engine for illustration purposes!
        /// NOTE: This method orchestrates all logic to create the view model, execute the view template,
        ///         and render the XSL-FO output, and then convert that XSL-FO output to a valid Pdf
        ///         in one and only place and greatly simplifies all Razor View Renderer implementations to keep
        ///         code very DRY.
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        [Obsolete("In AspNetCore all I/O tasks are truly Async so the RenderPdfAsync() method should be used instead!")]
        public virtual byte[] RenderPdf(MovieSearchResponse templateModel)
        {
            //Provide a synchronous (blocking) implementation by blocking the Async task; as all I/O is truly Async now in AspNetCore!
            var syncTaskRunner = Task.Run(() =>RenderPdfAsync(templateModel));
            var pdfBytes = syncTaskRunner.GetAwaiter().GetResult();
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