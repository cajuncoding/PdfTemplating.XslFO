using System;
using System.Xml.Linq;
using PdfTemplating.XslFO;
using Microsoft.AspNetCore.Mvc;
using PdfTemplating.XslFO.Razor.AspNetCoreMvc;
using XslFO.Samples.MovieSearchService;

namespace PdfTemplating.AspNetCoreMvc.Reports.PdfRenderers
{
    /// <summary>
    /// This class implements both the sync and sync interfaces so that it can illustrate side-by-side the legacy Fonet (sync),
    /// and teh new ApacheFOP.Serverless (async) approaches.
    /// </summary>
    public class RazorMoviePdfRenderer : BaseRazorPdfTemplate, IAsyncPdfTemplatingRenderer<MovieSearchResponse>
    {
        public Controller MvcController { get; set; }

        public RazorMoviePdfRenderer(Controller mvcController)
            : base("~/Reports.Razor/MoviePdfReport/MoviesReport.cshtml")
        {
            this.MvcController = mvcController;
        }

        /// <summary>
        /// Implements the IRazorPdfRenderer interface and delegate the specific logic to the abstract
        /// methods to simplify the implementations of all inheriting Razor View Renderers.
        /// NOTE: This method orchestrates all logic to create the view model, execute the view template,
        ///         and render the XSL-FO output, and then convert that XSL-FO output to a valid Pdf
        ///         in one and only place and greatly simplifies all Razor View Renderers to keep
        ///         code very DRY.
        /// </summary>
        /// <param name="templateModel"></param>
        /// <param name="mvcController"></param>
        /// <returns></returns>
        public virtual async Task<byte[]> RenderPdfAsync(MovieSearchResponse templateModel)
        {
            //Ensure that compatibility Mode is Disabled for proper rendering of our Model
            templateModel.FonetCompatibilityEnabled = false;

            //***********************************************************
            //Execute the Razor View to generate the XSL-FO output
            //***********************************************************
            //***********************************************************
            //Execute the Razor View to generate the XSL-FO output
            //***********************************************************
            var razorViewRenderer = new MvcRazorViewRenderer();
            var renderResult = await razorViewRenderer.RenderViewAsync(this.RazorViewPath, this.MvcController, templateModel).ConfigureAwait(false);

            //***********************************************************
            //OPTIONALLY validate the Output by Loading the XSL-FO output into a fully validated XDocument...
            //***********************************************************
            //Load the XSL-FO output into a fully validated XDocument.
            //NOTE: This template must generate valid Xsl-FO output -- via the well-formed xml we load into the XDocument return value -- to be rendered as a Pdf Binary!
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