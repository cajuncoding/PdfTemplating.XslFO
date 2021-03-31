using System;

namespace PdfTemplating.XslFO.Razor.AspNetMvc
{
    public class RazorViewRenderResult
    {
        public RazorViewRenderResult(string renderOutput)
        {
            this.RenderOutput = renderOutput;
        }

        public string RenderOutput { get; protected set; }
    }
}
