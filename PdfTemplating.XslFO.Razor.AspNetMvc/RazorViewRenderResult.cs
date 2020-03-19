using System;

namespace PdfTemplating.XslFO.Razor.AspNet
{
    public class RazorViewRenderResult
    {
        public RazorViewRenderResult(string renderOutput)
        {
            this.RenderOutput = renderOutput;
        }

        public String RenderOutput { get; protected set; }
    }
}
