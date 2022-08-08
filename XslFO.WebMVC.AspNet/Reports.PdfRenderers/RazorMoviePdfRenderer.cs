using System;
using System.Configuration;
using System.Reflection;
using PdfTemplating.XslFO.Razor.AspNetMvc;
using System.Web.Mvc;
using System.Xml.Linq;
using PdfTemplating.XslFO;
using System.Threading.Tasks;
using XslFO.Samples.MovieSearchService;

namespace PdfTemplating.AspNetMvc.Reports.PdfRenderers
{
    /// <summary>
    /// This class implements both the sync and sync interfaces so that it can illustrate side-by-side the legacy Fonet (sync),
    /// and teh new ApacheFOP.Serverless (async) approaches.
    /// </summary>
    public class RazorMoviePdfRenderer : BaseRazorPdfTemplate, IPdfTemplatingRenderer<MovieSearchResponse>, IAsyncPdfTemplatingRenderer<MovieSearchResponse>
    {
        public RazorMoviePdfRenderer(ControllerContext controllerContext)
            : base("~/Reports.Razor/MoviePdfReport/MoviesReport.cshtml", controllerContext)
        {}

        /// <summary>
        /// Implements the IPdfTemplatingRenderer interface with FO.NET in-memory rendering engine for illustration purposes!
        /// NOTE: This method orchestrates all logic to create the view model, execute the view template,
        ///         and render the XSL-FO output, and then convert that XSL-FO output to a valid Pdf
        ///         in one and only place and greatly simplifies all Razor View Renderers to keep
        ///         code very DRY.
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        public virtual byte[] RenderPdf(MovieSearchResponse templateModel)
        {
            //***********************************************************
            //Execute the Razor View to generate the XSL-FO output
            //***********************************************************
            var razorViewRenderer = new MvcRazorViewRenderer(this.ControllerContext);
            var renderResult = razorViewRenderer.RenderView(this.RazorViewVirtualPath, templateModel);
            var xslfoContent = renderResult.RenderOutput;

            //Create the Pdf Options for the XSL-FO Rendering engine to use
            var pdfOptions = new XslFOPdfOptions()
            {
                Author = Assembly.GetExecutingAssembly()?.GetName()?.Name ?? "PdfTemplating Renderer",
                Title = $"Xsl-FO Pdf Templating Renderer [{this.GetType().Name}]",
                Subject = $"Dynamic Razor Template Generated Xsl-FO Pdf Document [{DateTime.Now}]",
                //SET the Base Directory for XslFO Images, Xslt Imports, etc.
                //BaseDirectory = this.RazorViewFileInfo.Directory,
                BaseDirectory = this.RazorViewFileInfo.Directory,
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
        ///         in one and only place and greatly simplifies all Razor View Renderers to keep
        ///         code very DRY.
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        public virtual async Task<byte[]> RenderPdfAsync(MovieSearchResponse templateModel)
        {
            //Ensure that compatibility Mode is Disabled for proper rendering of our Model
            templateModel.FonetCompatibilityEnabled = false;

            //***********************************************************
            //Execute the Razor View to generate the XSL-FO output
            //***********************************************************
            var razorViewRenderer = new MvcRazorViewRenderer(this.ControllerContext);
            var renderResult = razorViewRenderer.RenderView(this.RazorViewVirtualPath, templateModel);

            //***********************************************************
            //OPTIONALLY validate the Output by Loading the XSL-FO output into a fully validated XDocument...
            //***********************************************************
            //Load the XSL-FO output into a fully validated XDocument.
            //NOTE: This template must generate valid Xsl-FO output to be rendered as a Pdf Binary! 
            //      This is optional, but parsing the output into XML will validate that it is well-formed!
            //var xslFODoc = XDocument.Parse(renderResult.RenderOutput);

            //******************************************************************************************
            //Execute the Transformation of the XSL-FO source to Binary Pdf via Apache FOP Service...
            //******************************************************************************************
            //var pdfBytes = await ApacheFOPServerlessHelper.RenderXslFOToPdfAsync(xslFODoc).ConfigureAwait(false);
            var pdfBytes = await ApacheFOPServerlessHelper.RenderXslFOToPdfAsync(renderResult.RenderOutput).ConfigureAwait(false);
            return pdfBytes;
        }

    }
}