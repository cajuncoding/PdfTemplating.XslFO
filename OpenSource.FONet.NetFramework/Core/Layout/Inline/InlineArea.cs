namespace Fonet.Layout.Inline
{
    internal abstract class InlineArea : Area
    {
        private int yOffset = 0;
        private int xOffset = 0;
        protected int height = 0;
        private int verticalAlign = 0;
        protected string pageNumberId = null;
        private float red, green, blue;
        protected bool underlined = false;
        protected bool overlined = false;
        protected bool lineThrough = false;

        public InlineArea(
            FontState fontState, int width, float red,
            float green, float blue)
            : base(fontState)
        {
            this.contentRectangleWidth = width;
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        public float getBlue()
        {
            return this.blue;
        }

        public float getGreen()
        {
            return this.green;
        }

        public float getRed()
        {
            return this.red;
        }

        public override void SetHeight(int height)
        {
            this.height = height;
        }

        public override int GetHeight()
        {
            return this.height;
        }

        public virtual void setVerticalAlign(int align)
        {
            this.verticalAlign = align;
        }

        public virtual int getVerticalAlign()
        {
            return this.verticalAlign;
        }

        public void setYOffset(int yOffset)
        {
            this.yOffset = yOffset;
        }

        public int getYOffset()
        {
            return this.yOffset;
        }

        public void setXOffset(int xOffset)
        {
            this.xOffset = xOffset;
        }

        public virtual int getXOffset()
        {
            return this.xOffset;
        }

        public string getPageNumberID()
        {
            return pageNumberId;
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

    }
}