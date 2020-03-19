using Fonet.Render.Pdf;

namespace Fonet.Layout.Inline
{
    internal class InlineSpace : Space
    {
        private int size;
        private bool resizeable = true;
        private bool eatable = false;
        protected bool underlined = false;
        protected bool overlined = false;
        protected bool lineThrough = false;

        public InlineSpace(int amount)
        {
            this.size = amount;
        }

        public InlineSpace(int amount, bool resizeable)
        {
            this.resizeable = resizeable;
            this.size = amount;
        }

        public void setUnderlined(bool ul)
        {
            this.underlined = ul;
        }

        public bool getUnderlined()
        {
            return this.underlined;
        }

        public void setOverlined(bool ol)
        {
            this.overlined = ol;
        }

        public bool getOverlined()
        {
            return this.overlined;
        }

        public void setLineThrough(bool lt)
        {
            this.lineThrough = lt;
        }

        public bool getLineThrough()
        {
            return this.lineThrough;
        }

        public int getSize()
        {
            return size;
        }

        public void setSize(int amount)
        {
            this.size = amount;
        }

        public bool getResizeable()
        {
            return resizeable;
        }

        public void setResizeable(bool resizeable)
        {
            this.resizeable = resizeable;
        }

        public void setEatable(bool eatable)
        {
            this.eatable = eatable;
        }

        public bool isEatable()
        {
            return eatable;
        }

        public override void render(PdfRenderer renderer)
        {
            renderer.RenderInlineSpace(this);
        }

    }
}