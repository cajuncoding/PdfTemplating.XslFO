using System.Web.Mvc;

namespace PdfTemplating.XslFO.Razor
{
    public class RazorPdfTemplatingRenderer<T> : BaseMvcRazorViewPdfTemplatingRenderer<T> where T:class
    {
        public RazorPdfTemplatingRenderer(string razorViewPath, ControllerContext controllerContext)
            : base(razorViewPath, controllerContext)
        {
        }
    }
}