using System;

namespace PdfTemplating
{
    public class TemplatedRenderResult
    {
        public TemplatedRenderResult(string renderOutput)
        {
            this.RenderOutput = renderOutput;
        }

        public string RenderOutput { get; protected set; }
    }
}
