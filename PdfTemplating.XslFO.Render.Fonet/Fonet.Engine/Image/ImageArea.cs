namespace Fonet.Image
{
    using Fonet.Layout;
    using Fonet.Layout.Inline;
    using Fonet.Render.Pdf;

    internal class ImageArea : InlineArea
    {
        protected int xOffset = 0;
        protected int align;
        protected int valign;
        protected FonetImage image;

        public ImageArea(FontState fontState, FonetImage img, int AllocationWidth,
                         int width, int height, int startIndent, int endIndent,
                         int align)
            : base(fontState, width, 0, 0, 0)
        {
            this.currentHeight = height;
            this.contentRectangleWidth = width;
            this.height = height;
            this.image = img;
            this.align = align;
        }

        public override int getXOffset()
        {
            return this.xOffset;
        }

        public FonetImage getImage()
        {
            return this.image;
        }

        public override void render(PdfRenderer renderer)
        {
            renderer.RenderImageArea(this);
        }

        public int getImageHeight()
        {
            return currentHeight;
        }

        public void setAlign(int align)
        {
            this.align = align;
        }

        public int getAlign()
        {
            return this.align;
        }

        public override void setVerticalAlign(int align)
        {
            this.valign = align;
        }

        public override int getVerticalAlign()
        {
            return this.valign;
        }

        public void setStartIndent(int startIndent)
        {
            xOffset = startIndent;
        }

    }
}