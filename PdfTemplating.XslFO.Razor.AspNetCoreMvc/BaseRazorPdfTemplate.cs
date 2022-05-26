using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PdfTemplating.XslFO.Razor.AspNetCoreMvc
{
    public abstract class BaseRazorPdfTemplate
    {
        /// <summary>
        /// The Path or VirtualPath to the Razor View.
        /// </summary>
        public string RazorViewPath { get; protected set; }

        protected BaseRazorPdfTemplate(string razorViewPath)
        {
            if (string.IsNullOrWhiteSpace(razorViewPath))
                throw new ArgumentNullException(nameof(razorViewPath), "The virtual path to the Razor is null/empty; a valid virtual path must be specified.");

            //NOTE: The Local FileInfo for the Razor View template/file will have it's Directory used
            //  as the BaseDirectory for resolving locally referenced files/images within the XSL-FO processing.
            this.RazorViewPath = razorViewPath;
        }
    }
}