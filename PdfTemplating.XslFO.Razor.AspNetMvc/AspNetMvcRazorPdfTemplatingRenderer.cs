/*
Copyright 2012 Brandon Bernard

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace PdfTemplating.XslFO.Razor.AspNet
{
    public class AspNetMvcRazorPdfTemplatingRenderer<TViewModel>: IPdfTemplatingRenderer<TViewModel>
    {
        protected AspNetMvcRazorPdfTemplatingRenderer()
        {}

        protected AspNetMvcRazorPdfTemplatingRenderer(String razorViewVirtualPath, ControllerContext controllerContext = null)
        {
            if(String.IsNullOrWhiteSpace(razorViewVirtualPath))
                throw new ArgumentNullException(nameof(razorViewVirtualPath), "The virtual path to the Razor is null/empty; a valid virtual path must be specified.");

            this.InitializeBase(razorViewVirtualPath, controllerContext);
        }

        protected void InitializeBase(String razorViewVirtualPath, ControllerContext controllerContext = null)
        {
            //NOTE: The Local FileInfo for the Razor View template/file will have it's Directory used
            //  as the BaseDirectory for resolving locally referenced files/images within the XSL-FO processing.
            this.RazorViewVirtualPath = razorViewVirtualPath;

            var razorViewFilePath = HttpContext.Current.Server.MapPath(razorViewVirtualPath);
            this.RazorViewFileInfo = new FileInfo(razorViewFilePath);

            this.ControllerContext = controllerContext;
        }

        /// <summary>
        /// The original VirtualPath to the Razor View File.
        /// NOTE: REQUIRED for underlying MVC ViewEngine to work as expected!
        /// </summary>
        public String RazorViewVirtualPath { get; protected set; }

        /// <summary>
        /// The FileInfo to the Razor View file
        /// </summary>
        public FileInfo RazorViewFileInfo { get; set; }


        /// <summary>
        /// Returns a valid ControllerContext to use when executing the MVC Razor View; this is an 
        /// abstract method that must be implemented by inheriting classes.
        /// </summary>
        /// <returns></returns>
        public ControllerContext ControllerContext { get; protected set; }

        #region IPdfRenderer implementation

        /// <summary>
        /// Implements the IRazorPdfRenderer interface and delegate the specific logic to the abstract
        /// methods to simplify the implementations of all inheriting Razor View Renderers.
        /// NOTE: This method orchestrates all logic to create the view model, execute the view template,
        ///         and render the XSL-FO output, and then convert that XSL-FO output to a valid Pdf
        ///         in one and only place and greatly simplifies all Razor View Renderers to keep
        ///         code very DRY.
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        public virtual byte[] RenderPdf(TViewModel templateModel)
        {
            //***********************************************************
            //Execute the Razor View to generate the XSL-FO output
            //***********************************************************
            var razorViewRenderer = new AspNetMvcRazorViewRenderer(this.ControllerContext);
            var renderResult = razorViewRenderer.RenderView(this.RazorViewVirtualPath, templateModel);

            //Load the XSL-FO output into a fully validated XDocument.
            //NOTE: This template must generate valid Xsl-FO output -- via the well-formed xml we load into the XDocument return value -- to be rendered as a Pdf Binary!
            var xslFODoc = XDocument.Parse(renderResult.RenderOutput);
            
            //Create the Pdf Options for the XSL-FO Rendering engine to use
            var pdfOptions = this.CreatePdfOptions();

            //Finally conver the XSL-FO XDocument into valid Pdf Binary data
            var pdfBytes = this.RenderXslFOPdfBytes(xslFODoc, pdfOptions);
            return pdfBytes;
        }

        #endregion

        #region Helper Methods (each can be individually Overridden as needed)

        /// <summary>
        /// Helper method to Create the PdfOptions for the XSL-FO Rendering engine to use.
        /// </summary>
        /// <returns></returns>
        protected virtual XslFOPdfOptions CreatePdfOptions()
        {
            //Initialize the Pdf rendering options for the XSL-FO Pdf Engine
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

            return pdfOptions;
        }

        /// <summary>
        /// Helper method to convert the XSL-FO into a valid Pdf
        /// </summary>
        /// <param name="xslFODoc"></param>
        /// <param name="xslFOPdfOptions"></param>
        /// <returns></returns>
        protected virtual byte[] RenderXslFOPdfBytes(XDocument xslFODoc, XslFOPdfOptions xslFOPdfOptions)
        {
            //***********************************************************
            //Render the Xsl-FO results into a Pdf binary output
            //***********************************************************
            var xslFOPdfRenderer = new FONetXslFOPdfRenderer(xslFODoc, xslFOPdfOptions);
            var pdfBytes = xslFOPdfRenderer.RenderPdfBytes();
            return pdfBytes;
        }

        #endregion
    }
}