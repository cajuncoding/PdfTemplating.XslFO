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
using MVC.Templating;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace PdfTemplating.XslFO.Razor
{
    public abstract class BaseMvcRazorViewPdfTemplatingRenderer<TViewModel>: IPdfTemplatingRenderer<TViewModel>
    {

        #region Abstract Constructor that all implementations must provide

        protected BaseMvcRazorViewPdfTemplatingRenderer(string razorViewPath, ControllerContext controllerContext)
        {
            this.RazorViewPath = razorViewPath;
            this.ControllerContext = controllerContext;
            
            //Load the Local FileInfo for the View Report so that we can use it's Directory
            //  as the BaseDirectory for resolving locally referenced files/images within the XSL-FO processing.
            this.RazorViewFileInfo = new FileInfo(this.RazorViewPath.StartsWith("~")
                                                    ? HttpContext.Current.Server.MapPath(this.RazorViewPath)
                                                    : this.RazorViewPath);
        }

        /// <summary>
        /// The ViewPath to the Razor View to be used for Templating the XSL-FO Output for 
        /// the Pdf Report; this is an abstract method that must be implemented by inheriting classes.
        /// </summary>
        public string RazorViewPath { get; protected set; }

        /// <summary>
        /// The FileInfo to the Razor View file
        /// </summary>
        public FileInfo RazorViewFileInfo { get; protected set; }

        /// <summary>
        /// Returns a valid ControllerContext to use when executing the MVC Razor View; this is an 
        /// abstract method that must be implemented by inheriting classes.
        /// </summary>
        /// <returns></returns>
        public ControllerContext ControllerContext { get; protected set; }

        #endregion

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
            //Render the XSL-FO output from the Razor Template and the View Model
            var xslFODoc = this.RenderXslFOXml(templateModel);

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
                Author = "BBernard",
                Title = "Xsl-FO Test Application",
                Subject = $"Dynamic Razor Template Generated Xsl-FO Pdf Document [{DateTime.Now}]",
                //SET the Base Directory for XslFO Images, Xslt Imports, etc.
                BaseDirectory = this.RazorViewFileInfo.Directory,
                EnableAdd = false,
                EnableCopy = true,
                EnableModify = false,
                EnablePrinting = true,
            };

            return pdfOptions;
        }

        /// <summary>
        /// Helper method to render the XSL FO output
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        protected virtual XDocument RenderXslFOXml(TViewModel viewModel)
        {
            if (this.ControllerContext == null) throw new ArgumentNullException(nameof(this.ControllerContext));

            //***********************************************************
            //Execute the Razor View to generate the XSL-FO output
            //***********************************************************
            var razorViewRenderer = new MvcRazorViewRenderer(this.ControllerContext);
            var xslFOTextOutput = razorViewRenderer.RenderViewToString(this.RazorViewPath, viewModel);

            //Load the XSL-FO output into a fully validated XDocument.
            var xslFODoc = XDocument.Parse(xslFOTextOutput);
            return xslFODoc;
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

        //TODO: Implement XSLFO rendering here???...
        public byte[] RenderPdfBytes(XDocument xslFODoc, XslFOPdfOptions xslFOPdfOptions)
        {
            throw new NotImplementedException();
        }
    }
}