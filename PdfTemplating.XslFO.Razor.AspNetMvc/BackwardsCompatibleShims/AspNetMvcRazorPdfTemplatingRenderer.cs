using System;
using System.Web.Mvc;

namespace PdfTemplating.XslFO.Razor.AspNetMvc.BackwardsCompatibleShims
{
    /// <summary>
    /// Provide for backwards compatibility with previous versions but the name and functionality has
    /// been simplified and streamlined in the MvcRazorViewRenderer class.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    [Obsolete("Recommended to use MvcRazorViewRenderer instead.")]
    public class AspNetMvcRazorPdfTemplatingRenderer: BaseRazorPdfTemplate
    {
        public AspNetMvcRazorPdfTemplatingRenderer(string razorViewVirtualPath, ControllerContext controllerContext = null)
            : base(razorViewVirtualPath, controllerContext)
        {
        }
    }
}
