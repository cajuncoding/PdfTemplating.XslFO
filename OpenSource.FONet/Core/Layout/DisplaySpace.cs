using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal class DisplaySpace : Space
    {
        private int size;

        public DisplaySpace(int size)
        {
            this.size = size;
        }

        public int getSize()
        {
            return size;
        }

        public override void render(PdfRenderer renderer)
        {
            renderer.RenderDisplaySpace(this);
        }

    }
}