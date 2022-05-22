using System;
using System.Web.Mvc;

namespace PdfTemplating.XslFO.Razor.AspNetMvc
{
    public class AspNetMvcRazorViewRenderer : MvcRazorViewRenderer
    {
        public AspNetMvcRazorViewRenderer(ControllerContext controllerContext = null)
            : base(controllerContext)
        { }
    }
}
