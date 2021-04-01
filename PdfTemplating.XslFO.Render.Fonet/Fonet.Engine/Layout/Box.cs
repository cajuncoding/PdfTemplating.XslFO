using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal abstract class Box
    {
        protected internal Area parent;

        protected internal AreaTree areaTree = null;

        public abstract void render(PdfRenderer renderer);
    }

}