using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace PdfTemplating.XslFO.Razor.AspNetMvc
{
    public abstract class BaseRazorPdfTemplate
    {
        protected BaseRazorPdfTemplate(String razorViewVirtualPath, ControllerContext controllerContext = null)
        {
            if(string.IsNullOrWhiteSpace(razorViewVirtualPath))
                throw new ArgumentNullException(nameof(razorViewVirtualPath), "The virtual path to the Razor is null/empty; a valid virtual path must be specified.");

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
        public string RazorViewVirtualPath { get; protected set; }

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
    }
}