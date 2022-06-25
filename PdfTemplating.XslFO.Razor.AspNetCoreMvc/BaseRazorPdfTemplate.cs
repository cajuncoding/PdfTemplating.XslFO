using System;
using Microsoft.AspNetCore.Mvc;

namespace PdfTemplating.XslFO.Razor.AspNetCoreMvc
{
    public abstract class BaseRazorPdfTemplate
    {
        protected BaseRazorPdfTemplate(String razorViewPath, Controller mvcController)
        {
            if (string.IsNullOrWhiteSpace(razorViewPath))
                throw new ArgumentNullException(nameof(razorViewPath), "The virtual path to the Razor is null/empty; a valid virtual path must be specified.");

            //NOTE: The Local FileInfo for the Razor View template/file will have it's Directory used
            //  as the BaseDirectory for resolving locally referenced files/images within the XSL-FO processing.
            this.RazorViewPath = razorViewPath;
            this.MvcController = mvcController;
        }

        /// <summary>
        /// The original VirtualPath to the Razor View File.
        /// NOTE: REQUIRED for underlying MVC ViewEngine to work as expected!
        /// </summary>
        public string RazorViewPath { get; protected set; }

        /// <summary>
        /// Returns a valid ControllerContext to use when executing the MVC Razor View; this is an 
        /// abstract method that must be implemented by inheriting classes.
        /// </summary>
        /// <returns></returns>
        public Controller MvcController { get; protected set; }
    }
}